using Docs.UI.Models;

namespace Docs.UI.Interfaces
{
    public interface IDocumentService
    {
        Task AddDocumentAsync(Document document);
        Task DeleteDocumentAsync(string id);
        Task<List<Document>> GetAllDocumentsAsync();
        Task<Document> GetDocumentByIdAsync(string id);
        Task UpdateDocumentAsync(string id, Document document);
    }
}
