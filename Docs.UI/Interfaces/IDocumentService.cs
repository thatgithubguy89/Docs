using Docs.UI.Models;

namespace Docs.UI.Interfaces
{
    public interface IDocumentService
    {
        Task<List<Document>> GetAllDocumentsAsync();
        Task<Document> GetDocumentByIdAsync(string id);
    }
}
