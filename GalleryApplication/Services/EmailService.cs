using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using GalleryApplication.Interfaces;


namespace GalleryApplication.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(email)); 
            mailMessage.From = new MailAddress("galleryweb1@gmail.com");  
            mailMessage.Subject = subject;
            mailMessage.Body = string.Format(body, "Administration", "galleryweb1@gmail.com", message);
            mailMessage.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "galleryweb1@gmail.com",  
                    Password = "bluelamborgini123"  
                };
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mailMessage);
            }
        }
    }
}