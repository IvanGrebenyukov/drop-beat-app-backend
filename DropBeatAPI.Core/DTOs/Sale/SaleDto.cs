namespace DropBeatAPI.Core.DTOs.Sale;

public class SaleDto
{
    public Guid PurchaseId { get; set; }
    public Guid BeatId { get; set; }
    public string BeatTitle { get; set; }
    public decimal BeatPrice { get; set; }  
    public DateTime PurchaseDate { get; set; }
    public string BuyerStageName { get; set; }
    public string SellerStageName { get; set; }
}