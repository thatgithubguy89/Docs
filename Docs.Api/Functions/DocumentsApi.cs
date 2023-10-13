using System.Net;
using System.Web.Http;
using AutoMapper;
using Docs.Api.Interfaces;
using Docs.Api.Models;
using Docs.Api.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Docs.Api.Functions
{
    public class DocumentsApi
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger<DocumentsApi> _logger;
        private readonly IMapper _mapper;
        private readonly IPagingService<Document, DocumentDto> _pageService;

        public DocumentsApi(IDocumentRepository documentRepository, ILogger<DocumentsApi> logger, IMapper mapper, IPagingService<Document, DocumentDto> pageService)
        {
            _documentRepository = documentRepository;
            _logger = logger;
            _mapper = mapper;
            _pageService = pageService;
        }

        [OpenApiOperation(operationId: "GetAllDocuments", tags: new[] { "Documents" })]
        [OpenApiParameter(name: "page")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.None)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<DocumentDto>), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(BadRequestResult), Description = "The Bad Request response")]
        [Function("GetAllDocuments")]
        public async Task<IActionResult> GetAllDocuments([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "alldocuments/{page:int?}")] HttpRequestData req, int page)
        {
            _logger.LogInformation("Getting all documents.");

            try
            {
                var documents = await _documentRepository.GetAllDocumentsAsync();

                var pageResponse = _pageService.Page(documents, page);

                return new OkObjectResult(pageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to get all documents: {}", ex.Message);

                return new InternalServerErrorResult();
            }
        }

        [OpenApiOperation(operationId: "GetSingleDocument", tags: new[] { "Documents" })]
        [OpenApiParameter(name: "id")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.None)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentDto), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(BadRequestResult), Description = "The Bad Request response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(NotFoundResult), Description = "The Not Found response")]
        [Function("GetSingleDocument")]
        public async Task<IActionResult> GetSingleDocument([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "singledocument/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Getting document with the id of {}", id);

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BadRequestResult();

                var document = await _documentRepository.GetDocumentByIdAsync(id);

                if (document == null)
                    return new NotFoundResult();

                return new OkObjectResult(_mapper.Map<DocumentDto>(document));
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to get document with the id of {}: {}", id, ex.Message);

                return new InternalServerErrorResult();
            }
        }

        [OpenApiOperation(operationId: "CreateDocument", tags: new[] { "Documents" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DocumentDto))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.None)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(NoContentResult), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(BadRequestResult), Description = "The Bad Request response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(NotFoundResult), Description = "The Not Found response")]
        [Function("CreateDocument")]
        public async Task<IActionResult> CreateDocument([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "")] HttpRequestData req)
        {
            _logger.LogInformation("Creating document");

            try
            {
                using StreamReader streamReader = new StreamReader(req.Body);
                var body = await streamReader.ReadToEndAsync();
                var document = JsonConvert.DeserializeObject<DocumentDto>(body);

                if (document == null)
                    return new BadRequestResult();

                await _documentRepository.AddDocumentAsync(_mapper.Map<Document>(document));

                return new NoContentResult();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to create document: {}", ex.Message);

                return new InternalServerErrorResult();
            }
        }

        [OpenApiOperation(operationId: "UpdateDocument", tags: new[] { "Documents" })]
        [OpenApiParameter(name: "id")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DocumentDto))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.None)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(NoContentResult), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(BadRequestResult), Description = "The Bad Request response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(NotFoundResult), Description = "The Not Found response")]
        [Function("UpdateDocument")]
        public async Task<IActionResult> UpdateDocument([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Updating document with the id of {}", id);

            try
            {
                using StreamReader streamReader = new StreamReader(req.Body);
                var body = await streamReader.ReadToEndAsync();
                var updatedDocument = JsonConvert.DeserializeObject<DocumentDto>(body);

                if (string.IsNullOrWhiteSpace(id))
                    return new BadRequestResult();

                var document = await _documentRepository.GetDocumentByIdAsync(id);
                if (document == null)
                    return new NotFoundResult();

                await _documentRepository.UpdateDocumentAsync(_mapper.Map<Document>(updatedDocument));

                return new NoContentResult();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to update document with the id of {}: {}", id, ex.Message);

                return new InternalServerErrorResult();
            }
        }

        [OpenApiOperation(operationId: "DeleteDocument", tags: new[] { "Documents" })]
        [OpenApiParameter(name: "id")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.None)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(NoContentResult), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(BadRequestResult), Description = "The Bad Request response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(NotFoundResult), Description = "The Not Found response")]
        [Function("DeleteDocument")]
        public async Task<IActionResult> DeleteDocument([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Deleting document with the id of {}", id);

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new BadRequestResult();

                var document = await _documentRepository.GetDocumentByIdAsync(id);
                if (document == null)
                    return new NotFoundResult();

                await _documentRepository.DeleteDocumentAsync(id);

                return new NoContentResult();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to delete document with the id of {}: {}", id, ex.Message);

                return new InternalServerErrorResult();
            }
        }
    }
}
