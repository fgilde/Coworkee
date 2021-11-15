using System.Threading.Tasks;
using CleanArchitectureBase.Application.Requests.Mail;

namespace CleanArchitectureBase.Application.Contracts.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}