namespace DropBeatAPI.Core.DTOs.Payment;

public class ProcessPaymentDto
{
    public Guid UserId { get; set; }
    public string PaymentId { get; set; }
}
