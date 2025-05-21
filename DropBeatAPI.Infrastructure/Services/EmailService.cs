using DropBeatAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DropBeatAPI.Core.DTOs.Payment;
using MimeKit;
using MailKit.Net.Smtp;

namespace DropBeatAPI.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost = "smtp.mail.ru";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "ivan-grebenyukov@mail.ru";
        private readonly string _smtpPass = "KaLitzAetFUD8VMJzj5E";

        public async Task SendConfirmationEmail(string email, string code)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DropBeat", _smtpUser));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Код подтверждения для DropBeat";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                <h2>Добро пожаловать в DropBeat!</h2>
                <p>Ваш код подтверждения: <strong>{code}</strong></p>
                <p>Код действителен в течение 15 минут.</p>
                <br>
                <p>С уважением,<br>Команда DropBeat</p>"
            };

            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки письма: {ex.Message}");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
        
        public async Task SendPurchaseEmail(PurchaseEmailDto dto)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DropBeat", _smtpUser));
            message.To.Add(MailboxAddress.Parse(dto.Email));
            message.Subject = "Ваши покупки в DropBeat";

            var builder = new BodyBuilder
            {
                HtmlBody = @"
                <h2>Спасибо за покупку!</h2>
                <p>Ваши биты и лицензии во вложениях.</p>
                <br>
                <p>С уважением,<br>Команда DropBeat</p>"
            };

            // Чек в формате PDF
            builder.Attachments.Add("receipt.pdf", dto.ReceiptPdf, new ContentType("application", "pdf"));

            // Добавляем биты
            foreach (var (beatTitle, beatFile) in dto.Beats)
            {
                builder.Attachments.Add($"{beatTitle}.mp3", beatFile, new ContentType("audio", "mpeg"));
            }

            // Добавляем лицензии
            foreach (var (licenseTitle, licenseFile) in dto.Licenses)
            {
                builder.Attachments.Add($"{licenseTitle}.pdf", licenseFile, new ContentType("application", "pdf"));
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
