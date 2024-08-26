using System;
using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Application.Services.Implementations
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IConfiguration _configuration;
        public EmailTemplateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConfirmEmailTemplate(string otp, string email, string firstName, string title)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "ComfirmEmail.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{otp}", otp).
                Replace("{email}", email)
                .Replace("{first_name}", firstName)
                .Replace("{title}", title);

            return msgBody;
        }

        public string GetReceiptBroadcastEmailTemplate(string fullName, string message)
        {
            var body = "Dear Bill Payer {full_name}, " + Environment.NewLine + "";
            var folderName = Path.Combine("wwwroot", "SendReceiptTemplate.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName).Replace("\\", "/");
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);

            var msgBody = body.Replace("{full_name}", fullName) + Environment.NewLine + message;

            return msgBody;
        }
    }
}
