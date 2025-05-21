using DropBeatAPI.Core.DTOs.Payment;

namespace DropBeatAPI.Core.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreatePaymentAsync(Guid userId, string email);
    Task<PaymentResultDto> ProcessPaymentAsync(Guid userId, string paymentId);
}