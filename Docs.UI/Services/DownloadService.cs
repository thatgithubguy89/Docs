using Docs.UI.Interfaces;
using Docs.UI.Models.Responses;
using Newtonsoft.Json;

namespace Docs.UI.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _http;

        public DownloadService(HttpClient http)
        {
            _http = http;
        }

        public async Task<MemoryStream> GetFileMemoryStream(string id)
        {
            var byteArray = await GetByteArray(id);

            return new MemoryStream(byteArray);
        }

        private async Task<byte[]> GetByteArray(string id)
        {
            byte[] byteArray = Array.Empty<byte>();

            var response = await _http.GetAsync($"http://localhost:7020/api/pdf/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<OkObjectResultSingleValue<byte[]>>(json);
                byteArray = data?.Value ?? Array.Empty<byte>();
            }

            return byteArray;
        }
    }
}
