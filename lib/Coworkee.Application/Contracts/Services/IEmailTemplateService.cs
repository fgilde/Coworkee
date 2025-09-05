using System.Threading.Tasks;
using System.Threading;
using lib.Coworkee.Shared;

namespace lib.Coworkee.Application.Contracts.Services;

public interface IEmailTemplateService
{
    public Task<string> RunAsync(EmailTemplate template, object model, CancellationToken cancellationToken = default);
    public string Run(EmailTemplate template, object model);
}