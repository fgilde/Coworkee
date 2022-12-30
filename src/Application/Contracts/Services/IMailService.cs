using System.Threading.Tasks;
using Coworkee.Application.Requests.Mail;

namespace Coworkee.Application.Contracts.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}