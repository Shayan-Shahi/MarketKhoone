using MarketKhoone.Services.Contracts.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Text;

namespace MarketKhoone.Services.Services.Identity
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _Configuration;

        public EmailService(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public void SendEmail(string ReceiverEmail, string Subject, string Body)
        {
            string pass = _Configuration["Passwords:GmailPassword"];
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 1000000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("wintekgroup365@gmail.com", pass);
            MailMessage mm = new MailMessage("wintekgroup365@gmail.com", ReceiverEmail, Subject, Body);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            client.Send(mm);
        }
    }
}
