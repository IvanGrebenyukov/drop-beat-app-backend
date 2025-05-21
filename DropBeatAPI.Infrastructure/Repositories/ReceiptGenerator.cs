using DropBeatAPI.Core.Entities;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace DropBeatAPI.Infrastructure.Repositories;

public static class ReceiptGenerator
{
    public static MemoryStream GenerateReceipt(IEnumerable<(string BeatTitle, decimal Price)> items, decimal totalAmount)
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Arial", 14, XFontStyle.Regular);

        int y = 40;
        gfx.DrawString("Чек покупки", font, XBrushes.Black, new XPoint(20, y));
        y += 20;

        foreach (var item in items)
        {
            var line = $"{item.BeatTitle} - {item.Price} руб.";
            gfx.DrawString(line, font, XBrushes.Black, new XPoint(20, y));
            y += 20;
        }

        gfx.DrawString($"Итого: {totalAmount} руб.", font, XBrushes.Black, new XPoint(20, y + 10));

        var stream = new MemoryStream();
        document.Save(stream, false);
        stream.Position = 0;
        return stream;
    }
}