using DropBeatAPI.Core.DTOs.Sale;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;
    private readonly UserManager<User> _userManager;

    public SalesController(ISaleService saleService, UserManager<User> userManager)
    {
        _saleService = saleService;
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet("seller")]
    public async Task<ActionResult<List<SaleDto>>> GetSellerSales()
    {
        var sellerId = GetUserId();
        var sales = await _saleService.GetSellerSalesAsync(sellerId);
        return Ok(sales);
    }

    [Authorize]
    [HttpGet("seller/period")]
    public async Task<ActionResult<List<SaleDto>>> GetSellerSalesByPeriod([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var sellerId = GetUserId();
        
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        end = DateTime.SpecifyKind(end, DateTimeKind.Utc);
        
        var sales = await _saleService.GetSellerSalesByPeriodAsync(sellerId, start, end);
        return Ok(sales);
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<ActionResult<List<SaleDto>>> GetUserPurchases()
    {
        var userId = GetUserId();
        var purchases = await _saleService.GetUserPurchasesAsync(userId);
        return Ok(purchases);
    }
    
    private Guid GetUserId()
    {
        return Guid.Parse(_userManager.GetUserId(User));
    }
}