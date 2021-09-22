using CleanArchitectureBase.Application.Requests.Mail;
using System.Threading.Tasks;

namespace CleanArchitectureBase.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}