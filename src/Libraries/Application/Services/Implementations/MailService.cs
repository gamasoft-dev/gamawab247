using Application.DTOs;
using Application.Services.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IEmailTemplateService _emailTemplateService;


        public MailService(IOptions<MailSettings> mailSettings, IEmailTemplateService emailTemplateService)
        {
            _mailSettings = mailSettings.Value;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<bool> SendSingleMail(string reciepientAddress, string message, 
            string subject)
        {
            try
            {
                var x = Encoding.UTF8;
                {
                    
                };
                var email = new MimeMessage()
                {

                    Subject = subject,
                    Body = new TextPart("html",message)
                };
                
                email.To.Add(MailboxAddress.Parse(reciepientAddress));
                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
           

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);

                    await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);

                    await client.SendAsync(email);

                    client.Disconnect(true);
                }
                return true;
            }
            catch(SmtpException)
            {
                throw;
            }
        }
    }
}
