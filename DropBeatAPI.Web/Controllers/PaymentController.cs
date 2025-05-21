using DropBeatAPI.Core.DTOs.Payment;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Создание платежа и получение ссылки для оплаты.
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
    {
        try
        {
            var paymentResponse = await _paymentService.CreatePaymentAsync(dto.UserId, dto.Email);
            return Ok(paymentResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = $"Ошибка создания платежа: {ex.Message}" });
        }
    }

    /// <summary>
    /// Обработка завершенного платежа.
    /// </summary>
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto dto)
    {
        try
        {
            var paymentResult = await _paymentService.ProcessPaymentAsync(dto.UserId, dto.PaymentId);
            return Ok(paymentResult);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = $"Ошибка обработки платежа: {ex.Message}" });
        }
    }
}