//using AutoMapper;
//using Docs.Api.Functions;
//using Docs.Api.Interfaces;
//using Docs.Api.Models;
//using Docs.Api.Models.Dtos;
//using Docs.Api.Profiles;
//using Docs.Test.Utility;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Shouldly;
//using System.Web.Http;

//namespace Docs.Test.Functions
//{
//    public class DocumentsApiTests
//    {
//        private readonly Mock<ILogger<DocumentsApi>> _mockLogger = new Mock<ILogger<DocumentsApi>>();
//        Mock<IDocumentRepository> _mockDocumentRepository;
//        IMapper _mapper;

//        [SetUp]
//        public void Setup()
//        {
//            var config = new MapperConfiguration(c => c.AddProfile(typeof(MappingProfile)));
//            _mapper = new Mapper(config);

//            _mockDocumentRepository = new Mock<IDocumentRepository>();
//        }

//        [Test]
//        public async Task GetAllDocuments()
//        {
//            _mockDocumentRepository.Setup(d => d.GetAllDocumentsAsync()).Returns(Task.FromResult(new List<Document>()));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.GetAllDocuments(request);
//            var result = actionResult as OkObjectResult;

//            result.Value.ShouldBeOfType<List<DocumentDto>>();
//            result.StatusCode.ShouldBe(StatusCodes.Status200OK);
//        }

//        [Test]
//        public async Task GetAllDocuments_Failure_ShouldReturn_BadRequestResult()
//        {
//            _mockDocumentRepository.Setup(d => d.GetAllDocumentsAsync()).Throws(new Exception());
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.GetAllDocuments(request);
//            var result = actionResult as InternalServerErrorResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
//        }

//        [Test]
//        public async Task GetSingleDocument()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Document()));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.GetSingleDocument(request, "123");
//            var result = actionResult as OkObjectResult;

//            result.Value.ShouldBeOfType<DocumentDto>();
//            result.StatusCode.ShouldBe(StatusCodes.Status200OK);
//        }

//        [TestCase(null)]
//        [TestCase("")]
//        [TestCase(" ")]
//        public async Task GetSingleDocument_GivenInvalidId_ShouldReturn_BadRequestResult(string id)
//        {
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.GetSingleDocument(request, id);
//            var result = actionResult as BadRequestResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
//        }

//        [Test]
//        public async Task GetSingleDocument_GivenIdForDocumentThatDoesNotExist_ShouldReturn_NotFoundResult()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult<Document>(null));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.GetSingleDocument(request, "123");
//            var result = actionResult as NotFoundResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
//        }

//        [Test]
//        public async Task GetSingleDocument_Failure_ShouldReturn_BadRequestResult()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Throws(new Exception());
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.GetSingleDocument(request, "123");
//            var result = actionResult as InternalServerErrorResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
//        }

//        [Test]
//        public async Task CreateDocument()
//        {
//            _mockDocumentRepository.Setup(d => d.AddDocumentAsync(It.IsAny<Document>()));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequestWithBody(new DocumentDto());

//            var actionResult = await documentsApi.CreateDocument(request);
//            var result = actionResult as NoContentResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status204NoContent);
//        }

//        [Test]
//        public async Task CreateDocument_GivenInvalidDocument_ShouldReturn_BadRequestResult()
//        {
//            _mockDocumentRepository.Setup(d => d.AddDocumentAsync(It.IsAny<Document>()));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequestWithBody(null);

//            var actionResult = await documentsApi.CreateDocument(request);
//            var result = actionResult as BadRequestResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
//        }

//        [Test]
//        public async Task CreateDocument_Failure_ShouldReturn_BadRequestResult()
//        {
//            _mockDocumentRepository.Setup(d => d.AddDocumentAsync(It.IsAny<Document>())).Throws(new Exception());
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequestWithBody(new DocumentDto());

//            var actionResult = await documentsApi.CreateDocument(request);
//            var result = actionResult as InternalServerErrorResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
//        }

//        [Test]
//        public async Task UpdateDocument()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Document()));
//            _mockDocumentRepository.Setup(d => d.UpdateDocumentAsync(It.IsAny<Document>()));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequestWithBody(new DocumentDto());

//            var actionResult = await documentsApi.UpdateDocument(request, "1");
//            var result = actionResult as NoContentResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status204NoContent);
//        }

//        [TestCase(null)]
//        [TestCase("")]
//        [TestCase(" ")]
//        public async Task UpdateDocument_GivenInvalidId_ShouldReturn_BadRequestResult(string id)
//        {
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequestWithBody(new DocumentDto());

//            var actionResult = await documentsApi.UpdateDocument(request, id);
//            var result = actionResult as BadRequestResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
//        }

//        [Test]
//        public async Task UpdateDocument_GivenIdForDocumentThatDoesNotExist_ShouldReturn_NotFoundResult()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult<Document>(null));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequestWithBody(new DocumentDto());

//            var actionResult = await documentsApi.UpdateDocument(request, "1");
//            var result = actionResult as NotFoundResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
//        }

//        [Test]
//        public async Task UpdateDocument_Failure_ShouldReturn_BadRequestResult()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Throws(new Exception());
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequestWithBody(new DocumentDto());

//            var actionResult = await documentsApi.UpdateDocument(request, "1");
//            var result = actionResult as InternalServerErrorResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
//        }

//        [Test]
//        public async Task DeleteDocument()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Document()));
//            _mockDocumentRepository.Setup(d => d.DeleteDocumentAsync(It.IsAny<string>()));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.DeleteDocument(request, "1");
//            var result = actionResult as NoContentResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status204NoContent);
//        }

//        [TestCase(null)]
//        [TestCase("")]
//        [TestCase(" ")]
//        public async Task DeleteDocument_GivenInvalidId_ShouldReturn_BadRequestResult(string id)
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Document()));
//            _mockDocumentRepository.Setup(d => d.DeleteDocumentAsync(It.IsAny<string>()));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.DeleteDocument(request, id);
//            var result = actionResult as BadRequestResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
//        }

//        [Test]
//        public async Task DeleteDocument_GivenIdForDocumentThatDoesNotExist_ShouldReturn_NotFoundResult()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Returns(Task.FromResult<Document>(null));
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.DeleteDocument(request, "1");
//            var result = actionResult as NotFoundResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
//        }

//        [Test]
//        public async Task DeleteDocument_Failure_ShouldReturn_BadRequestResult()
//        {
//            _mockDocumentRepository.Setup(d => d.GetDocumentByIdAsync(It.IsAny<string>())).Throws(new Exception());
//            var documentsApi = new DocumentsApi(_mockDocumentRepository.Object, _mockLogger.Object, _mapper);
//            var request = Generator.GenerateMockRequest();

//            var actionResult = await documentsApi.DeleteDocument(request, "1");
//            var result = actionResult as InternalServerErrorResult;

//            result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
//        }
//    }
//}
