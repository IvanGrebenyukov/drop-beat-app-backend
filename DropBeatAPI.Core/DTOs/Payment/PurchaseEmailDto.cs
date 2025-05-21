namespace DropBeatAPI.Core.DTOs.Payment;

public class PurchaseEmailDto
{
    public string Email { get; set; } = string.Empty;
    public List<(string, byte[])> Beats { get; set; } = new();
    public List<(string, byte[])> Licenses { get; set; } = new();
    public byte[] ReceiptPdf { get; set; } = null!;
}
