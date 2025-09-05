using System.Threading.Tasks;
using lib.Coworkee.Application.Requests.Mail;

namespace Coworkee.Application.Contracts.Services;

public interface IMailService
{
    Task SendAsync(MailRequest request);
}