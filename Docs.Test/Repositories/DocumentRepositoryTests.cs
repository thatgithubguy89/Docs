using Docs.Api.Interfaces;
using Docs.Api.Models;
using Docs.Api.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;

namespace Docs.Test.Repositories
{
    public class DocumentRepositoryTests
    {
        IDocumentRepository _documentRepository;
        Mock<ICachingService<Document>> _mockCachingService;
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
            _mockCachingService = new Mock<ICachingService<Document>>();

            _documentRepository = new DocumentRepository(_configuration, _mockCachingService.Object);

            _cosmosClient = new CosmosClient(connectionString: _configuration["CosmosConnection"]);
        }

        [TearDown]
        public async Task Teardown()
        {
            _mockCachingService.Setup(c => c.GetItems(It.IsAny<string>()));

            var documents = await _documentRepository.GetAllDocumentsAsync();

            foreach (var document in documents)
            {
                await _documentRepository.DeleteDocumentAsync(document.Id);
            }
        }

        [Test]
        public async Task AddDocumentAsync()
        {
            _mockCachingService.Setup(c => c.DeleteItems(It.IsAny<string>()));
            var id = await _documentRepository.AddDocumentAsync(_mockDocument);
            var result = await _documentRepository.GetDocumentByIdAsync(id);

            result.ShouldBeOfType<Document>();
            result.Id.ShouldBe(id);
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
            _mockCachingService.Setup(c => c.DeleteItems(It.IsAny<string>()));
            var id = await _documentRepository.AddDocumentAsync(_mockDocument);
            var document = await _documentRepository.GetDocumentByIdAsync(id);
            document.ShouldNotBeNull();

            await _documentRepository.DeleteDocumentAsync(id);
            var result = await _documentRepository.GetDocumentByIdAsync(id);

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
            _mockCachingService.Setup(c => c.SetItems(It.IsAny<string>(), It.IsAny<List<Document>>()));
            foreach (var document in _mockDocuments)
            {
                await _container.CreateItemAsync(document, new PartitionKey(document.Id));
            }

            var result = await _documentRepository.GetAllDocumentsAsync();

            result.ShouldBeOfType<List<Document>>();
            result.Count.ShouldBe(_mockDocuments.Count);
        }

        [Test]
        public async Task GetAllDocumentsAsync_CachedItemsFound()
        {
            _mockCachingService.Setup(c => c.GetItems(It.IsAny<string>())).Returns(_mockDocuments);

            var result = await _documentRepository.GetAllDocumentsAsync();

            result.ShouldBeOfType<List<Document>>();
            result.Count.ShouldBe(_mockDocuments.Count);
        }

        [Test]
        public async Task GetDocumentByIdAsync()
        {
            _mockCachingService.Setup(c => c.SetItem(It.IsAny<string>(), It.IsAny<Document>()));
            var id = await _documentRepository.AddDocumentAsync(_mockDocument);

            var result = await _documentRepository.GetDocumentByIdAsync(id);

            result.ShouldBeOfType<Document>();
            result.Id.ShouldBe(id);
            result.Title.ShouldBe(_mockDocument.Title);
            result.Content.ShouldBe(_mockDocument.Content);
        }

        [Test]
        public async Task GetDocumentByIdAsync_CacheItemFound()
        {
            _mockCachingService.Setup(c => c.GetItem(It.IsAny<string>())).Returns(_mockDocument);

            var result = await _documentRepository.GetDocumentByIdAsync(_mockDocument.Id);

            result.ShouldBeOfType<Document>();
            result.Id.ShouldBe(_mockDocument.Id);
            result.Title.ShouldBe(_mockDocument.Title);
            result.Content.ShouldBe(_mockDocument.Content);
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
            _mockCachingService.Setup(c => c.DeleteItems(It.IsAny<string>()));
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
