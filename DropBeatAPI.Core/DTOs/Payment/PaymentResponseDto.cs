namespace DropBeatAPI.Core.DTOs.Payment;

public class PaymentResponseDto
{
    public string PaymentUrl { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
}