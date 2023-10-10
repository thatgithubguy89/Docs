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

        public async Task<List<Document>> GetAllDocumentsAsync()
        {
            var documents = new List<Document>();

            var response = await _http.GetAsync("http://localhost:7020/api/alldocuments");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<OkObjectResultMultipleValues<Document>>(json);
                documents = data?.Value ?? new List<Document>();
            }

            return documents;
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
    }
}
