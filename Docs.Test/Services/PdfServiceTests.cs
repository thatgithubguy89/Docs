using Docs.Api.Interfaces;
using Docs.Api.Models;
using Docs.Api.Services;
using Moq;
using Shouldly;

namespace Docs.Test.Services
{
    public class PdfServiceTests
    {
        IPdfService _pdfService;
        Mock<IDocumentRepository> _mockDocumentRepository;

        private static Document _mockDocument = new Document { Id = "1", Title = "test", Content = "test" };

        [SetUp]
        public void Setup()
        {
            _mockDocumentRepository = new Mock<IDocumentRepository>();

            _pdfService = new PdfService(_mockDocumentRepository.Object);
        }

        [Test]
        public async Task CreatePdfByteArray()
        {
            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(_mockDocument));

            var result = await _pdfService.CreatePdfByteArray(_mockDocument.Id);

            result.ShouldBeOfType<byte[]>();
            result.ShouldNotBeEmpty();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task CreatePdfByteArray_GivenInvalidId_ShouldThrow_ArgumentNullException(string id)
        {
            await Should.ThrowAsync<ArgumentNullException>(async () => await _pdfService.CreatePdfByteArray(id));
        }

        [Test]
        public async Task CreatePdfByteArray_GivenIdForDocumentThatDoesNotExist_ShouldThrow_NullReferenceException()
        {
            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult<Document>(null));

            await Should.ThrowAsync<NullReferenceException>(async () => await _pdfService.CreatePdfByteArray("1"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task CreatePdfByteArray_GivenIdForDocumentWithInvalidContent_ShouldReturn_EmptyByteArray(string content)
        {
            var document = new Document { Id = "1", Title = "test", Content = content };
            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(document));

            var result = await _pdfService.CreatePdfByteArray("1");

            result.ShouldBeOfType<byte[]>();
            result.ShouldBeEmpty();
        }
    }
}
