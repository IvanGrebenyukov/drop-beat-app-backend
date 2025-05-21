using DropBeatAPI.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Infrastructure.Data
{
    public class BeatsDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public BeatsDbContext(DbContextOptions<BeatsDbContext> options) : base(options) { }

        public DbSet<UserSocialLink> UserSocialLinks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<UserGenre> UserGenres { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Beat> Beats { get; set; }
        public DbSet<BeatGenre> BeatGenres { get; set; }
        public DbSet<BeatMood> BeatMoods { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BeatTag> BeatTags { get; set; }
        public DbSet<BeatLike> BeatLikes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Report> Reports { get; set; }

        public DbSet<EmailConfirmationCode> EmailConfirmationCodes { get; set; }
        public DbSet<SellerRequest> SellerRequests { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserChat> UserChats { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация User
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Email).IsUnique();
                b.HasIndex(u => u.UserName).IsUnique();
                b.HasIndex(u => u.StageName).IsUnique();

                // Явно указываем навигационное свойство для корзин
                b.HasMany(u => u.Carts)
                    .WithOne(c => c.User)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Или Restrict в зависимости от требований
            });

            // UserSocialLink
            modelBuilder.Entity<UserSocialLink>()
                .HasIndex(us => new { us.UserId, us.Platform }).IsUnique();

            modelBuilder.Entity<UserSocialLink>()
                .HasOne(us => us.User)
                .WithMany(u => u.SocialLinks)
                .HasForeignKey(us => us.UserId);

            // Follow
            modelBuilder.Entity<Follow>()
                .HasKey(f => new { f.FollowerId, f.FollowingId });

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Beat
            modelBuilder.Entity<Beat>(b =>
            {
                b.HasIndex(beat => beat.CreatedAt);
                b.Property(beat => beat.LicenseType)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                b.Property(beat => beat.Price).HasColumnType("decimal(10,2)");
            });

            // BeatGenre
            modelBuilder.Entity<BeatGenre>()
                .HasKey(bg => new { bg.BeatId, bg.GenreId });

            // BeatMood
            modelBuilder.Entity<BeatMood>()
                .HasKey(bm => new { bm.BeatId, bm.MoodId });

            // Tag
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name).IsUnique();

            // BeatTag
            modelBuilder.Entity<BeatTag>()
                .HasKey(bt => new { bt.BeatId, bt.TagId });


            // Для BeatLike
            modelBuilder.Entity<BeatLike>(b =>
            {
                // Определяем составной первичный ключ
                b.HasKey(bl => new { bl.UserId, bl.BeatId });

                // Конфигурация связей
                b.HasOne(bl => bl.User)
                    .WithMany(u => u.LikedBeats)
                    .HasForeignKey(bl => bl.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(bl => bl.Beat)
                    .WithMany(b => b.Likes)
                    .HasForeignKey(bl => bl.BeatId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BeatLike>()
                    .HasIndex(bl => new { bl.UserId, bl.BeatId })
                    .IsUnique();

            //UserGenre
            modelBuilder.Entity<UserGenre>(b =>
            {
                // Определяем составной первичный ключ
                b.HasKey(ug => new { ug.UserId, ug.GenreId });

                // Конфигурация связей
                b.HasOne(ug => ug.User)
                    .WithMany(u => u.UserGenres)
                    .HasForeignKey(ug => ug.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(ug => ug.Genre)
                    .WithMany(g => g.UserGenres)
                    .HasForeignKey(ug => ug.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // Cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Cart>()
                .HasIndex(c => new { c.UserId })
                .IsUnique(); // Если 1 корзина на пользователя

            // CartItem
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.BeatId });

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Beat)
                .WithMany(b => b.CartItems)  // Убедитесь, что в Beat есть ICollection<CartItem> CartItems
                .HasForeignKey(ci => ci.BeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // Конфигурация Purchase
            modelBuilder.Entity<Purchase>(p =>
            {
                // Связь с покупателем
                p.HasOne(p => p.Buyer)
                    .WithMany(u => u.Purchases)
                    .HasForeignKey(p => p.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь с битом
                p.HasOne(p => p.Beat)
                    .WithMany(b => b.Purchases)
                    .HasForeignKey(p => p.BeatId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь с транзакцией (1:1)
                p.HasOne(p => p.Transactions)
                    .WithOne(t => t.Purchase)
                    .HasForeignKey<Transaction>(t => t.PurchaseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Purchase>()
                .HasIndex(p => p.PurchaseDate);

            // Report
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Reporter)
                .WithMany(u => u.ReportedBy)
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.ReportedUser)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.ReportedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasIndex(r => r.CreatedAt);


            modelBuilder.Entity<Transaction>(t =>
            {
                // Связь с пользователем
                t.HasOne(t => t.User)
                    .WithMany(u => u.Transactions)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь с покупкой (опционально)
                t.HasOne(t => t.Purchase)
                        .WithOne(p => p.Transactions)  // Была `.WithMany()`, исправляем на `.WithOne()`
                        .HasForeignKey<Purchase>(p => p.TransactionId)  // Указываем связь по `TransactionId`
                        .OnDelete(DeleteBehavior.SetNull);
            });
            
            // Chat
            modelBuilder.Entity<Chat>(c =>
            {
                c.HasKey(x => x.Id);

                c.Property(x => x.Type)
                    .HasConversion<string>() // Для читаемости в базе
                    .HasMaxLength(20);

                c.HasOne(c => c.Genre)
                    .WithMany()
                    .HasForeignKey(c => c.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

// UserChat (связь между пользователями и чатами)
            modelBuilder.Entity<UserChat>()
                .HasKey(uc => new { uc.ChatId, uc.UserId });

            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.Chat)
                .WithMany(c => c.Participants)
                .HasForeignKey(uc => uc.ChatId);

            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChats)
                .HasForeignKey(uc => uc.UserId);

// Message
            modelBuilder.Entity<Message>(m =>
            {
                m.HasKey(x => x.Id);

                m.HasOne(m => m.Chat)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                m.HasOne(m => m.Sender)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                m.Property(m => m.Text)
                    .IsRequired()
                    .HasMaxLength(2000);
            });

// UserBlock
            modelBuilder.Entity<UserBlock>()
                .HasKey(b => new { b.BlockerId, b.BlockedId });

            modelBuilder.Entity<UserBlock>()
                .HasOne(b => b.Blocker)
                .WithMany(u => u.BlockedUsers)
                .HasForeignKey(b => b.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserBlock>()
                .HasOne(b => b.Blocked)
                .WithMany(u => u.BlockedByUsers)
                .HasForeignKey(b => b.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);

            
            
        }
    }


}
