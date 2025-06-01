using DropBeatAPI.Core.DTOs.Sale;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DropBeatAPI.Infrastructure.Services;

public class SaleService : ISaleService
{
    private readonly BeatsDbContext _dbContext;

    public SaleService(BeatsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SaleDto>> GetSellerSalesAsync(Guid sellerId)
    {
        return await _dbContext.Purchases
            .Where(p => p.Beat.SellerId == sellerId)
            .Select(p => new SaleDto
            {
                PurchaseId = p.Id,
                BeatId = p.BeatId,
                BeatTitle = p.Beat.Title,
                BeatPrice = p.Beat.Price,
                PurchaseDate = p.PurchaseDate,
                BuyerStageName = p.Buyer.StageName,
                SellerStageName = p.Beat.Seller.StageName
            })
            .ToListAsync();
    }

    public async Task<List<SaleDto>> GetSellerSalesByPeriodAsync(Guid sellerId, DateTime start, DateTime end)
    {
        return await _dbContext.Purchases
            .Where(p => p.Beat.SellerId == sellerId && p.PurchaseDate >= start && p.PurchaseDate <= end)
            .Select(p => new SaleDto
            {
                PurchaseId = p.Id,
                BeatId = p.BeatId,
                BeatTitle = p.Beat.Title,
                BeatPrice = p.Beat.Price,
                PurchaseDate = p.PurchaseDate,
                BuyerStageName = p.Buyer.StageName,
                SellerStageName = p.Beat.Seller.StageName
            })
            .ToListAsync();
    }

    public async Task<List<SaleDto>> GetUserPurchasesAsync(Guid userId)
    {
        return await _dbContext.Purchases
            .Where(p => p.BuyerId == userId)
            .Select(p => new SaleDto
            {
                PurchaseId = p.Id,
                BeatId = p.BeatId,
                BeatTitle = p.Beat.Title,
                BeatPrice = p.Beat.Price,
                PurchaseDate = p.PurchaseDate,
                BuyerStageName = p.Buyer.StageName,
                SellerStageName = p.Beat.Seller.StageName
            })
            .ToListAsync();
    }
}
