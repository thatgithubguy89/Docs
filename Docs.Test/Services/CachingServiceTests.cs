using Docs.Api.Interfaces;
using Docs.Api.Models;
using Docs.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;

namespace Docs.Test.Services
{
    public class CachingServiceTests
    {
        ICachingService<Document> _cachingService;
        IMemoryCache _cache;
        MemoryCacheOptions _options;

        private static Document _mockDocument = new Document { Id = "1", Title = "test", Content = "test" };

        private static List<Document> _mockDocuments = new List<Document>
        {
            new Document { Id = "1", Title = "test1", Content = "test1" },
            new Document { Id = "2", Title = "test2", Content = "test2" }
        };

        [SetUp]
        public void Setup()
        {
            _options = new MemoryCacheOptions();

            _cache = new MemoryCache(_options);

            _cachingService = new CachingService<Document>(_cache);
        }

        [TearDown]
        public void Teardown()
        {
            _cachingService.DeleteItems("docs-1");
            _cachingService.DeleteItems("docs-2");
            _cachingService.DeleteItems("documents");
        }

        [Test]
        public void DeleteItem()
        {
            _cachingService.SetItems("documents", _mockDocuments);
            var documents = _cachingService.GetItems("documents");
            documents.ShouldNotBeNull();

            _cachingService.DeleteItems("documents");
            var result = _cachingService.GetItems("documents");

            result.ShouldBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void DeleteItem_GivenInvalidKey_ShouldThrow_ArgumentNullException(string key)
        {
            Should.Throw<ArgumentNullException>(() => _cachingService.DeleteItems(key));
        }

        [Test]
        public void GetItem()
        {
            _cachingService.SetItem($"doc-{_mockDocument.Id}", _mockDocument);

            var result = _cachingService.GetItem($"doc-{_mockDocument.Id}");

            result.ShouldNotBeNull();
            result.ShouldBeOfType<Document>();
            result.Title.ShouldBe("test");
            result.Content.ShouldBe("test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetItem_GivenInvalidKey_ShouldThrow_ArgumentNullException(string key)
        {
            Should.Throw<ArgumentNullException>(() => _cachingService.GetItem(key));
        }

        [Test]
        public void GetItems()
        {
            _cachingService.SetItems("documents", _mockDocuments);

            var result = _cachingService.GetItems("documents");

            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<Document>>();
            result.Count.ShouldBe(2);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetItems_GivenInvalidKey_ShouldThrow_ArgumentNullException(string key)
        {
            Should.Throw<ArgumentNullException>(() => _cachingService.GetItems(key));
        }

        [Test]
        public void SetItem()
        {
            _cachingService.SetItem($"doc-{_mockDocument.Id}", _mockDocument);

            var result = _cachingService.GetItem($"doc-{_mockDocument.Id}");

            result.ShouldNotBeNull();
            result.ShouldBeOfType<Document>();
            result.Title.ShouldBe("test");
            result.Content.ShouldBe("test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void SetItem_GivenInvalidKey_ShouldThrow_ArgumentNullException(string key)
        {
            Should.Throw<ArgumentNullException>(() => _cachingService.SetItem(key, _mockDocument));
        }

        [Test]
        public void SetItem_GivenInvalidItem_ShouldThrow_ArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => _cachingService.SetItem("1", null));
        }

        [Test]
        public void SetItems()
        {
            _cachingService.SetItems("documents", _mockDocuments);
            var result = _cachingService.GetItems("documents");

            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<Document>>();
            result.Count.ShouldBe(2);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void SetItems_GivenInvalidKey_ShouldThrow_ArgumentNullException(string key)
        {
            Should.Throw<ArgumentNullException>(() => _cachingService.SetItems(key, _mockDocuments));
        }

        [Test]
        public void SetItems_GivenInvalidItems_ShouldThrow_ArgumentOutOfRangeException()
        {
            Should.Throw<ArgumentOutOfRangeException>(() => _cachingService.SetItems("1", new List<Document>()));
        }
    }
}
