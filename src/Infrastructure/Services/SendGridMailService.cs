using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using lib.Coworkee.Application.Configurations;
using lib.Coworkee.Application.Contracts.Attributes;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Requests.Mail;

namespace Coworkee.Infrastructure.Services
{
    [RegisterAsIfConfigValueIsNotEmpty(typeof(IMailService), new[] { nameof(MailConfiguration), nameof(MailConfiguration.SendGridApiKey) })]
    public class SendGridMailService : IMailService
    {
        private readonly MailConfiguration _config;
        private readonly ILogger<SendGridMailService> _logger;

        public SendGridMailService(IOptions<ServerConfiguration> config, ILogger<SendGridMailService> logger)
        {
            _config = config.Value.MailConfiguration;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var client = new SendGridClient(_config.SendGridApiKey);
                var from = new EmailAddress(request.From ?? _config.From, request.SenderName ?? _config.DisplayName);
                var subject = request.Subject;
                var to = new EmailAddress(request.To, request.RecipientName);
                var plainTextContent = request.Body; // TODO: Plain it or add field for plain content in MailRequest
                var htmlContent = request.Body;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Body.ReadAsStringAsync();
                    _logger.LogError(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}