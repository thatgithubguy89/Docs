using Docs.Api.Models;

namespace Docs.Api.Interfaces
{
    public interface IDocumentRepository
    {
        Task<string> AddDocumentAsync(Document document);
        Task DeleteDocumentAsync(string id);
        Task<List<Document>> GetAllDocumentsAsync();
        Task<Document> GetDocumentByIdAsync(string id);
        Task UpdateDocumentAsync(Document document);
    }
}
