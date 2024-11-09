using DM7._1.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DM7._1.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IWebHostEnvironment _env;
        public EmailService(IOptions<EmailSettings> emailSettings, IWebHostEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            _env = env;
        }
        public async Task<string> GetTemplateContentAsync(string templateFileName,
                Dictionary<string, string> placeholders)
        {
            var templatePath = Path.Combine(_env.WebRootPath, "templates", templateFileName);
            // Check if the file exists
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Template file not found.", templatePath);
            }
            // Đọc file template
            string templateContent = await System.IO.File.ReadAllTextAsync(templatePath);

            // Thay thế các placeholders bằng các giá trị từ dictionary
            foreach (var placeholder in placeholders)
            {
                templateContent = templateContent.Replace("{{" + placeholder.Key + "}}", placeholder.Value);
            }

            return templateContent;
        }
        public async Task SendMailAsync(EmailRequest emailRequest)
        {
            var fromAddress = new MailAddress(_emailSettings.UserName);
            var toAddress = new MailAddress("dangminhquan9320@gmail.com");
            var smtp = new SmtpClient
            {
                Host = _emailSettings.Host,
                //25 khong ma hoa
                //465 (SSL) va 587 (TLS)
                Port = _emailSettings.Port,
                EnableSsl = _emailSettings.EnableSsl,
                //gui thong qua mang
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
            };
            // Các giá trị động cần thay thế
            var placeholders = new Dictionary<string, string>
                {
                    { "TITLE",  emailRequest.Subject},
                    { "CONTENT", emailRequest.HtmlContent },
                };
            string emailContent = await GetTemplateContentAsync("mail.html", placeholders);
            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = emailRequest.Subject,
                Body = emailContent,
                IsBodyHtml = true
            };
            await smtp.SendMailAsync(message);
        }
    }
}
