using Docs.UI.Interfaces;
using Docs.UI.Models;
using Newtonsoft.Json;

namespace Docs.UI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly HttpClient _http;

        public DocumentService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Document>> GetAllDocumentsAsync()
        {
            var documents = new List<Document>();

            var response = await _http.GetAsync("http://localhost:7020/api/alldocuments");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<OkObjectResult>(json);
                documents = data?.Value ?? new List<Document>();
            }

            return documents;
        }
    }
}
