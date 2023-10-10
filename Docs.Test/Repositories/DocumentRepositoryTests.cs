using Docs.Api.Interfaces;
using Docs.Api.Models;
using Docs.Api.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Shouldly;

namespace Docs.Test.Repositories
{
    public class DocumentRepositoryTests
    {
        IDocumentRepository _documentRepository;
        IConfigurationRoot _configuration = new ConfigurationBuilder()
            .AddUserSecrets("4c4c151e-f5e0-442e-a38c-13941ba553cf")
            .Build();

        private CosmosClient _cosmosClient;

        private Container _container
        {
            get => _cosmosClient.GetDatabase(_configuration["CosmosDatabaseId"]).GetContainer(_configuration["CosmosDocumentsContainerId"]);
        }

        private static Document _mockDocument = new Document { Id = "1", Title = "test", Content = "test" };

        private static List<Document> _mockDocuments = new List<Document>
        {
            new Document { Id = "1", Title = "test1", Content = "test1" },
            new Document { Id = "2", Title = "test2", Content = "test2" }
        };

        [SetUp]
        public void Setup()
        {
            _documentRepository = new DocumentRepository(_configuration);

            _cosmosClient = new CosmosClient(connectionString: _configuration["CosmosConnection"]);
        }

        [TearDown]
        public async Task Teardown()
        {
            var documents = await _documentRepository.GetAllDocumentsAsync();

            foreach (var document in documents)
            {
                await _documentRepository.DeleteDocumentAsync(document.Id);
            }
        }

        [Test]
        public async Task AddDocumentAsync()
        {
            await _documentRepository.AddDocumentAsync(_mockDocument);
            var result = await _documentRepository.GetDocumentByIdAsync("1");

            result.ShouldBeOfType<Document>();
            result.Id.ShouldBe(_mockDocument.Id);
            result.Title.ShouldBe(_mockDocument.Title);
            result.Content.ShouldBe(_mockDocument.Content);
        }

        [Test]
        public async Task AddDocumentAsync_GivenInvalidDocument_ShouldThrow_ArgumentOutOfRangeException()
        {
            await Should.ThrowAsync<ArgumentNullException>(async () => await _documentRepository.AddDocumentAsync(null));
        }

        [Test]
        public async Task DeleteDocumentAsync()
        {
            await _documentRepository.AddDocumentAsync(_mockDocument);
            var document = await _documentRepository.GetDocumentByIdAsync("1");
            document.ShouldNotBeNull();

            await _documentRepository.DeleteDocumentAsync("1");
            var result = await _documentRepository.GetDocumentByIdAsync("1");

            result.ShouldBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task DeleteDocumentAsync_GivenInvalidDocument_ShouldThrow_ArgumentNullException(string id)
        {
            await Should.ThrowAsync<ArgumentNullException>(async () => await _documentRepository.DeleteDocumentAsync(id));
        }

        [Test]
        public async Task GetAllDocumentsAsync()
        {
            foreach (var document in _mockDocuments)
            {
                await _container.CreateItemAsync(document, new PartitionKey(document.Id));
            }
            
            var result = await _documentRepository.GetAllDocumentsAsync();

            result.ShouldBeOfType<List<Document>>();
            result.Count.ShouldBe(_mockDocuments.Count);
        }

        [Test]
        public async Task GetDocumentByIdAsync()
        {
            await _documentRepository.AddDocumentAsync(_mockDocument);

            var result = await _documentRepository.GetDocumentByIdAsync("1");

            result.ShouldBeOfType<Document>();
            result.Id.ShouldBe("1");
            result.Title.ShouldBe("test");
            result.Content.ShouldBe("test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task GetDocumentByIdAsync_GivenInvalidId_ShouldThrow_ArgumentNullException(string id)
        {
            await Should.ThrowAsync<ArgumentNullException>(async () => await _documentRepository.GetDocumentByIdAsync(id));
        }

        [Test]
        public async Task UpdateDocumentAsync()
        {
            Document updatedDocument = new Document { Id = "1", Title = "test2", Content = "test2" };
            await _documentRepository.AddDocumentAsync(_mockDocument);

            await _documentRepository.UpdateDocumentAsync(updatedDocument);
            var result = await _documentRepository.GetDocumentByIdAsync("1");

            result.Id.ShouldBe("1");
            result.Title.ShouldBe("test2");
            result.Content.ShouldBe("test2");
        }

        [Test]
        public async Task UpdateDocumentAsync_GivenInvalidDocument_ShouldThrow_ArgumentOutOfRangeException()
        {
            await Should.ThrowAsync<ArgumentNullException>(async () => await _documentRepository.UpdateDocumentAsync(null));
        }
    }
}
