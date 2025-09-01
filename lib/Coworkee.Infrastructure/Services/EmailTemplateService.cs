using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RazorEngineCore;
using Coworkee.Application.Contracts.Services;
using Coworkee.Shared;
using Coworkee.Infrastructure.Models;
using Nextended.Core.Attributes;

namespace Coworkee.Infrastructure.Services;

[RegisterAs(typeof(IEmailTemplateService))]

public class EmailTemplateService : IEmailTemplateService
{
    private readonly ILogger<EmailTemplateService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EmailTemplateService(ILogger<EmailTemplateService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task<string> RunAsync(EmailTemplate emailTemplate, object model = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Run(emailTemplate, model), cancellationToken);
    }

    public string Run(EmailTemplate emailTemplate, object model = null)
    {
        try
        {
            var razorEngine = new RazorEngine();
            var template = Compile(razorEngine, emailTemplate, Parts(emailTemplate), _serviceProvider);
            return template.Run(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return emailTemplate.Content;
        }
    }

    private EmailTemplateCompiled Compile(IRazorEngine razorEngine, EmailTemplate template, IDictionary<string, string> parts, IServiceProvider serviceProvider)
    {
        return new EmailTemplateCompiled(template, serviceProvider,
            razorEngine.Compile<RazorEngineEmailTemplate>(template.Content),
            parts.ToDictionary(
                k => k.Key,
                v => razorEngine.Compile<RazorEngineEmailTemplate>(v.Value)));
    }


    private IDictionary<string, string> Parts(EmailTemplate template)
    {
        return new Dictionary<string, string>
        {
            {nameof(EmailTemplate.Names.Layout), template.IncludeLayout ? EmailTemplate.Layout(template.Culture).Content : "@RenderBody()"},
            {nameof(EmailTemplate.Names.Footer), template.IncludeLayout ? EmailTemplate.Footer(template.Culture).Content : ""}
        };
    }
}