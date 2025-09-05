using System.Threading.Tasks;

namespace lib.Coworkee.Application.Contracts.Repositories
{
    public interface IDocumentRepository
    {
        Task<bool> IsDocumentTypeUsed(int documentTypeId);
        Task<bool> IsDocumentExtendedAttributeUsed(int documentExtendedAttributeId);
    }
}