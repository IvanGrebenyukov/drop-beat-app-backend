using DropBeatAPI.Core.DTOs.Documents;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace DropBeatAPI.Infrastructure.Services;

public class LicenseService : ILicenseService
{
    public async Task<Stream> GenerateLicensePdfAsync(LicenseDocumentDto licenseDto)
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        var graphics = XGraphics.FromPdfPage(page);
        var fontTitle = new XFont("Arial", 20, XFontStyle.Bold);
        var fontText = new XFont("Arial", 12, XFontStyle.Regular);
        var fontSmallText = new XFont("Arial", 10, XFontStyle.Regular);

        var title = $"Лицензия на использование бита: {licenseDto.Title}";
        var licenseText = GetLicenseText(licenseDto.LicenseType);

        // Рисуем заголовок
        graphics.DrawString(title, fontTitle, XBrushes.Black, new XRect(0, 40, page.Width, 30), XStringFormats.TopCenter);
    
        // Рисуем информацию о продавце
        graphics.DrawString($"Продавец: {licenseDto.SellerName}", fontText, XBrushes.Black, new XRect(40, 100, page.Width - 80, 20), XStringFormats.TopLeft);
    
        // Создаем прямоугольник для текста лицензии
        var licenseRect = new XRect(40, 130, page.Width - 80, page.Height - 150);
    
        // Рисуем основной текст лицензии
        graphics.DrawString(licenseText, fontText, XBrushes.Black, licenseRect, XStringFormats.TopLeft);
    
        // Альтернативный способ измерения высоты текста
        var textHeight = EstimateTextHeight(licenseText, fontText, page.Width - 80);
        var yPosition = 130 + textHeight + 20;
    
        // Рисуем footer с датой
        var footerText = 
            "Данный документ подтверждает право на использование музыкального произведения в соответствии с указанной лицензией.\n" +
            $"Дата создания: {licenseDto.CreatedAt:dd.MM.yyyy}\n\n" +
            "© DropBeat, все права защищены.";
    
        graphics.DrawString(footerText, fontSmallText, XBrushes.Black, 
            new XRect(40, yPosition, page.Width - 80, page.Height - yPosition), 
            XStringFormats.TopLeft);

        using var stream = new MemoryStream();
        document.Save(stream, false);
        stream.Position = 0;
        return await Task.FromResult(stream);
    }

    private string GetLicenseText(LicenseType licenseType) => licenseType switch
    {
        LicenseType.Free => 
            "Данная лицензия предоставляет право на использование бита только для личных целей.\n" +
            "Бит не может быть размещен на стриминговых платформах или использоваться в коммерческих проектах.",
        
        LicenseType.NonExclusive => 
            "Данная лицензия позволяет использовать бит на стриминговых платформах (Spotify, Apple Music и др.).\n" +
            "Лицензия является неэксклюзивной — данный бит могут купить несколько пользователей.",

        LicenseType.Exclusive => 
            "Данная лицензия предоставляет исключительные права на использование бита.\n" +
            "Покупатель получает полные права и может размещать его на любых платформах.\n" +
            "Бит больше не будет доступен для покупки после завершения транзакции.",
        
        _ => "Неизвестный тип лицензии."
    };
    
    // Вспомогательный метод для оценки высоты текста
    private double EstimateTextHeight(string text, XFont font, double availableWidth)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        // Разбиваем текст на строки
        var lines = text.Split(new[] { '\n' }, StringSplitOptions.None);
        var lineHeight = font.GetHeight();
        var totalHeight = 0.0;

        foreach (var line in lines)
        {
            // Оцениваем количество переносов строк
            var lineLength = line.Length;
            var approxLineCount = Math.Ceiling(lineLength * font.Size / availableWidth * 0.6);
            totalHeight += lineHeight * Math.Max(1, approxLineCount);
        }

        return totalHeight;
    }
}