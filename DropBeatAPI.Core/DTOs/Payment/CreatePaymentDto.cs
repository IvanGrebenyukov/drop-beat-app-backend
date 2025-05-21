namespace DropBeatAPI.Core.DTOs.Payment;

public class CreatePaymentDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
}
