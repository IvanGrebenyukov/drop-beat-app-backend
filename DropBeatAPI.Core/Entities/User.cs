using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? StageName { get; set; }  // Псевдоним битмейкера
        public string? AvatarUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }

        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
        public bool IsSeller { get; set; }
        public decimal Balance { get; set; } = 0;
        public SellerRequestStatus SellerRequestStatus { get; set; } = SellerRequestStatus.None;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public ICollection<UserSocialLink> SocialLinks { get; set; } = new List<UserSocialLink>();
        public ICollection<Beat> Beats { get; set; } = new List<Beat>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Purchase> Sales { get; set; } = new List<Purchase>();
        public ICollection<BeatLike> LikedBeats { get; set; } = new List<BeatLike>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<Report> ReportedBy { get; set; } = new List<Report>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public ICollection<UserGenre> UserGenres { get; set; } = new List<UserGenre>();
        public ICollection<SellerRequest> SellerRequests { get; set; } = new List<SellerRequest>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<UserBlock> BlockedUsers { get; set; } = new List<UserBlock>();
        public ICollection<UserBlock> BlockedByUsers { get; set; } = new List<UserBlock>();

    }

    public enum SellerRequestStatus
    {
        None,       // Не подавал заявку
        Pending,    // Заявка на рассмотрении
        Approved,   // Одобрено
        Rejected    // Отклонено
    }
}
