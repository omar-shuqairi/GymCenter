using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace GymCenter.Services
{

    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            using (var client = new SmtpClient(emailSettings["Host"], int.Parse(emailSettings["Port"])))
            {
                client.Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["FromEmail"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body, byte[] attachmentData, string attachmentName)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            using (var client = new SmtpClient(emailSettings["Host"], int.Parse(emailSettings["Port"])))
            {
                client.Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["FromEmail"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);


                using (var memoryStream = new MemoryStream(attachmentData))
                {
                    var attachment = new Attachment(memoryStream, attachmentName, "application/pdf");
                    mailMessage.Attachments.Add(attachment);

                    await client.SendMailAsync(mailMessage);
                }
            }
        }

    }

}