using Docs.Api.Functions;
using Docs.Api.Interfaces;
using Docs.Test.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Docs.Test.Functions
{
    public class PdfApiTests
    {
        Mock<ILogger<PdfApi>> _mockLogger;
        Mock<IPdfService> _mockPdfService;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<PdfApi>>();
            _mockPdfService = new Mock<IPdfService>();
        }

        [Test]
        public async Task GetPdf()
        {
            _mockPdfService.Setup(p => p.CreatePdfByteArray(It.IsAny<string>())).Returns(Task.FromResult(new byte[1]));
            var _pdfApi = new PdfApi(_mockLogger.Object, _mockPdfService.Object);
            var request = Generator.GenerateMockRequest();

            var actionResult = await _pdfApi.GetPdf(request, "1");
            var result = actionResult as OkObjectResult;

            result.Value.ShouldBeOfType<byte[]>();
            result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

        [TestCase("")]
        [TestCase(" ")]
        public async Task GetPdf_GivenInvalidId_ShouldReturn_BadRequestResult(string id)
        {
            var _pdfApi = new PdfApi(_mockLogger.Object, _mockPdfService.Object);
            var request = Generator.GenerateMockRequest();

            var actionResult = await _pdfApi.GetPdf(request, id);
            var result = actionResult as BadRequestResult;

            result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Test]
        public async Task GetPdf_GivenIdForDocumentWithNullContent_ShouldReturn_BadRequestResult()
        {
            _mockPdfService.Setup(p => p.CreatePdfByteArray(It.IsAny<string>())).Returns(Task.FromResult(Array.Empty<byte>()));
            var _pdfApi = new PdfApi(_mockLogger.Object, _mockPdfService.Object);
            var request = Generator.GenerateMockRequest();

            var actionResult = await _pdfApi.GetPdf(request, "1");
            var result = actionResult as BadRequestResult;

            result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }
    }
}
