using Docs.Api.Interfaces;
using Docs.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Docs.Api.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _configuration;

        private Container _container
        {
            get => _cosmosClient.GetDatabase(_configuration["CosmosDatabaseId"]).GetContainer(_configuration["CosmosDocumentsContainerId"]);
        }

        public DocumentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosClient = new CosmosClient(connectionString: _configuration["CosmosConnection"]);
        }

        public async Task<string> AddDocumentAsync(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            document.Id = Guid.NewGuid().ToString();
            document.CreatedBy = "test@gmail.com";
            document.CreateTime = DateTime.Now;
            document.LastEditTime = DateTime.Now;

            await _container.CreateItemAsync(document, new PartitionKey(document.Id));

            return document.Id;
        }

        public async Task DeleteDocumentAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            await _container.DeleteItemAsync<Document>(id, new PartitionKey(id));
        }

        public async Task<List<Document>> GetAllDocumentsAsync()
        {
            var items = _container.GetItemQueryIterator<Document>();

            return (await items.ReadNextAsync()).ToList();
        }

        public async Task<Document> GetDocumentByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            try
            {
                var item = await _container.ReadItemAsync<Document>(id, new PartitionKey(id));

                return item.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            document.CreatedBy = "test@gmail.com";
            document.LastEditTime = DateTime.Now;

            await _container.UpsertItemAsync(document);
        }
    }
}
