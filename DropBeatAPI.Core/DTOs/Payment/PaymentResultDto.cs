namespace DropBeatAPI.Core.DTOs.Payment;

public class PaymentResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}