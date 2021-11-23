using System.Threading.Tasks;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Requests.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CleanArchitectureBase.Infrastructure.Services
{
    [RegisterAs(typeof(IMailService))]
    public class SMTPMailService : IMailService
    {
        private readonly ServerConfiguration _config;
        private readonly ILogger<SMTPMailService> _logger;

        public SMTPMailService(IOptions<ServerConfiguration> config, ILogger<SMTPMailService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var email = new MimeMessage
                {
                    To = { new MailboxAddress(request.RecipientName, request.To) },
                    Sender = new MailboxAddress(_config.MailConfiguration.DisplayName, request.From ?? _config.MailConfiguration.From),
                    Subject = request.Subject,
                    Body = new BodyBuilder
                    {
                        HtmlBody = request.Body
                    }.ToMessageBody()
                };
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config.MailConfiguration.Host, _config.MailConfiguration.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.MailConfiguration.UserName, _config.MailConfiguration.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}