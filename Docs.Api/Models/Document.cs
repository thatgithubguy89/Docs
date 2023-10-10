using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Docs.Api.Models
{
    public class Document : TableEntity
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? LastEditTime { get; set; }
    }
}
