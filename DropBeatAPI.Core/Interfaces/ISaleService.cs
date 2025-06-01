using DropBeatAPI.Core.DTOs.Sale;

namespace DropBeatAPI.Core.Interfaces;

public interface ISaleService
{
    Task<List<SaleDto>> GetSellerSalesAsync(Guid sellerId);
    Task<List<SaleDto>> GetSellerSalesByPeriodAsync(Guid sellerId, DateTime start, DateTime end);
    Task<List<SaleDto>> GetUserPurchasesAsync(Guid userId);
}