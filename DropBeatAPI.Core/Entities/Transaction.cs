using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DropBeatAPI.Core.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }

        // Связь с пользователем (обязательна для всех типов транзакций)
        public Guid UserId { get; set; }
        public User User { get; set; }

        // Связь с Purchase (только для транзакций типа Purchase)
        public Guid? PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }

        // Данные транзакции
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string? YooKassaPaymentId { get; set; }
        public TransactionStatus Status { get; set; }
    }

    public enum TransactionType
    {
        Purchase,   // Покупка бита
        Withdrawal  // Вывод средств
    }

    public enum TransactionStatus
    {
        Pending,    // В обработке
        Completed,  // Успешно завершено
        Failed      // Ошибка
    }
}
