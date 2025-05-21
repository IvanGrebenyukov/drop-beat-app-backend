using System.Text;
using DropBeatAPI.Core.DTOs.Payment;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using DropBeatAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace DropBeatAPI.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly BeatsDbContext _context;
    private readonly IYandexStorageService _storageService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentService> _logger;


    public PaymentService(BeatsDbContext context, IYandexStorageService storageService, 
                          IEmailService emailService, IConfiguration configuration, ILogger<PaymentService> logger)
    {
        _context = context;
        _storageService = storageService;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(Guid userId, string email)
{
    var cart = await _context.Carts
        .Include(c => c.Items)
        .ThenInclude(ci => ci.Beat)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    if (cart == null || !cart.Items.Any())
        throw new InvalidOperationException("Корзина пуста.");

    var totalAmount = cart.Items.Sum(ci => ci.Beat.Price);

    var shopId = _configuration["YooMoney:ShopId"];
    var secretKey = _configuration["YooMoney:SecretKey"];
    var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{shopId}:{secretKey}"));

    var client = new RestClient("https://api.yookassa.ru/v3/payments");
    var request = new RestRequest("", Method.Post);

    // 🆕 Добавляем Idempotence-Key для избежания дублирования
    var idempotenceKey = Guid.NewGuid().ToString();
    request.AddHeader("Idempotence-Key", idempotenceKey);

    // Добавляем заголовки
    request.AddHeader("Authorization", $"Basic {authHeader}");
    request.AddHeader("Content-Type", "application/json");

    // Данные платежа
    var paymentData = new
    {
        amount = new { value = totalAmount.ToString("F2"), currency = "RUB" },
        capture = true,
        confirmation = new { type = "redirect", return_url = "https://your-frontend.com/payment-success" },
        description = $"Оплата корзины пользователя {userId}"
    };

    // Добавляем тело запроса
    request.AddJsonBody(paymentData);

    // Выполняем запрос
    var response = await client.ExecuteAsync(request);

    if (!response.IsSuccessful)
        throw new Exception($"Ошибка создания платежа: {response.Content}");

    try
    {
        dynamic paymentResponse = JsonConvert.DeserializeObject(response.Content);

        return new PaymentResponseDto
        {
            PaymentUrl = paymentResponse.confirmation.confirmation_url,
            PaymentId = paymentResponse.id
        };
    }
    catch (Exception ex)
    {
        throw new Exception($"Ошибка обработки ответа от YooKassa: {ex.Message}");
    }
}


    public async Task<PaymentResultDto> ProcessPaymentAsync(Guid userId, string paymentId)
    {
        var shopId = _configuration["YooMoney:ShopId"];
        var secretKey = _configuration["YooMoney:SecretKey"];
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{shopId}:{secretKey}"));

        var client = new RestClient($"https://api.yookassa.ru/v3/payments/{paymentId}");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("Authorization", $"Basic {authHeader}");
        request.AddHeader("Content-Type", "application/json");

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
            throw new Exception($"Не удалось получить информацию о платеже: {response.Content}");

        dynamic paymentInfo = JsonConvert.DeserializeObject(response.Content);
        string status = paymentInfo.status.ToString();

        if (status != "succeeded")
            throw new InvalidOperationException("Платёж не был подтвержден. Покупка невозможна.");
        
        var cart = await _context.Carts
            .Include(c => c.User)
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Beat)
                    .ThenInclude(b => b.Seller)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    
        if (cart == null || cart.User == null)
            throw new InvalidOperationException("Корзина или пользователь не найдены.");
    
        var email = cart.User.Email ?? throw new InvalidOperationException("Email пользователя отсутствует.");
    
        var beats = cart.Items
            .Where(ci => ci.Beat != null)
            .Select(ci => ci.Beat)
            .ToList();
    
        // 🔥 Получаем список всех оригинальных файлов единожды
        var allFiles = await _storageService.ListFilesAsync("audios/original/");
        var fileMap = allFiles.ToDictionary(f => Path.GetFileNameWithoutExtension(f).Split('_').Last(), f => f);
    
        var originalBeatFiles = new List<(string, MemoryStream)>();
        var uniqueLicenses = new HashSet<string>();
    
        foreach (var beat in beats)
        {
            var seller = beat.Seller;
            if (seller == null)
            {
                _logger.LogWarning($"У бита '{beat.Title}' нет продавца.");
                continue;
            }
    
            seller.Balance += beat.Price * 0.9m;
            uniqueLicenses.Add(beat.LicenseType.ToString());
    
            if (beat.LicenseType == LicenseType.Exclusive)
                beat.IsAvailable = false;
    
            try
            {
                if (!fileMap.TryGetValue(beat.Id.ToString(), out var fileName))
                {
                    _logger.LogWarning($"Файл оригинального аудио не найден для бита '{beat.Title}'.");
                    continue;
                }
    
                var stream = await _storageService.GetFileAsync(fileName);
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                originalBeatFiles.Add((beat.Title, memoryStream));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка загрузки файла '{beat.Title}': {ex.Message}");
            }
    
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = beat.Price * 0.9m,
                Type = TransactionType.Purchase,
                Status = TransactionStatus.Completed,
                YooKassaPaymentId = paymentId
            };
            _context.Transactions.Add(transaction);
    
            _context.Purchases.Add(new Purchase
            {
                Id = Guid.NewGuid(),
                BuyerId = userId,
                BeatId = beat.Id,
                TransactionId = transaction.Id,
                Email = email
            });
        }
        
        _context.CartItems.RemoveRange(cart.Items);
    
        await _context.SaveChangesAsync();
    
        // 📄 Генерация чека
        var receipt = ReceiptGenerator.GenerateReceipt(
            beats.Select(b => (b.Title, b.Price)),
            beats.Sum(b => b.Price)
        );
    
        var purchaseEmailDto = new PurchaseEmailDto
        {
            Email = email,
            ReceiptPdf = receipt.ToArray(),
            Beats = originalBeatFiles.Select(b => (b.Item1, b.Item2.ToArray())).ToList(),
            Licenses = uniqueLicenses.Select(l => ($"Лицензия на {l}", Encoding.UTF8.GetBytes($"Лицензия: {l}"))).ToList()
        };
    
        try
        {
            await _emailService.SendPurchaseEmail(purchaseEmailDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка отправки письма: {ex.Message}");
        }
    
        return new PaymentResultDto
        {
            Success = true,
            Message = "Оплата успешно завершена!"
        };
    }
}
