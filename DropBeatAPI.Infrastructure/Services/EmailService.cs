using DropBeatAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            var message = new MailMessage
            {
                From = new MailAddress(_smtpUser, "DropBeat"),
                Subject = "Код подтверждения для DropBeat",
                Body = $@"
            <h2>Добро пожаловать в DropBeat!</h2>
            <p>Ваш код подтверждения: <strong>{code}</strong></p>
            <p>Код действителен в течение 15 минут.</p>
            <br>
            <p>С уважением,<br>Команда DropBeat</p>",
                IsBodyHtml = true,
            };
            message.To.Add(email);

            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}
