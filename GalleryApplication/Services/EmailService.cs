using System.Threading.Tasks;
using GalleryApplication.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace GalleryApplication.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
 
            emailMessage.From.Add(new MailboxAddress("Administration", "galleryweb1@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
             
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mailtrap.io", 25, false);
                await client.AuthenticateAsync("565407f7eefd1e", "3335b20c396f8e");
                await client.SendAsync(emailMessage);
 
                await client.DisconnectAsync(true);
            }
        }
    }
}