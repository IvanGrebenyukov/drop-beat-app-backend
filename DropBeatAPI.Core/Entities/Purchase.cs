using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{

    public class Purchase
    {
        public Guid Id { get; set; }

        // Связи
        public Guid BuyerId { get; set; }
        public User Buyer { get; set; }

        public Guid BeatId { get; set; }
        public Beat Beat { get; set; }

        public Guid TransactionId { get; set; }
        public Transaction Transactions { get; set; }

        // Дополнительные поля
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public string Email { get; set; } = string.Empty;
    }

    //public class Purchase
    //{
    //    public Guid Id { get; set; }
    //    public Guid BuyerId { get; set; }
    //    public User Buyer { get; set; } = null!;
    //    public Guid SellerId { get; set; }
    //    public User Seller { get; set; } = null!;
    //    public Guid BeatId { get; set; }
    //    public Beat Beat { get; set; } = null!;
    //    public LicenseType LicenseType { get; set; }
    //    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    //    public string Email { get; set; } = string.Empty;
    //}
}
