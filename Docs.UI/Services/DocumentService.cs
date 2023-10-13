using Docs.UI.Interfaces;
using Docs.UI.Models;
using Docs.UI.Models.Responses;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Docs.UI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly HttpClient _http;

        public DocumentService(HttpClient http)
        {
            _http = http;
        }

        public async Task AddDocumentAsync(Document document)
        {
            await _http.PostAsJsonAsync("http://localhost:7020/api/createdocument", document);
        }

        public async Task DeleteDocumentAsync(string id)
        {
            await _http.DeleteAsync($"http://localhost:7020/api/{id}");
        }

        public async Task<PageResponse<Document>> GetAllDocumentsAsync(int page)
        {
            var pageResponse = new PageResponse<Document>();

            var response = await _http.GetAsync($"http://localhost:7020/api/alldocuments/{page}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<OkObjectResultSingleValue<PageResponse<Document>>>(json);
                pageResponse = data?.Value;
            }

            return pageResponse;
        }

        public async Task<Document> GetDocumentByIdAsync(string id)
        {
            var document = new Document();

            var response = await _http.GetAsync($"http://localhost:7020/api/singledocument/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<OkObjectResultSingleValue<Document>>(json);
                document = data?.Value ?? new Document();
            }

            return document;
        }

        public async Task UpdateDocumentAsync(string id, Document document)
        {
            await _http.PutAsJsonAsync($"http://localhost:7020/api/{id}", document);
        }
    }
}
