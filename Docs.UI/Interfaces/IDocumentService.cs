using Docs.UI.Models;
using Docs.UI.Models.Responses;

namespace Docs.UI.Interfaces
{
    public interface IDocumentService
    {
        Task AddDocumentAsync(Document document);
        Task DeleteDocumentAsync(string id);
        Task<PageResponse<Document>> GetAllDocumentsAsync(int page);
        Task<Document> GetDocumentByIdAsync(string id);
        Task UpdateDocumentAsync(string id, Document document);
    }
}
