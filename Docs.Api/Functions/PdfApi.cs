using System.Net;
using Docs.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Docs.Api.Functions
{
    public class PdfApi
    {
        private readonly ILogger<PdfApi> _logger;
        private readonly IPdfService _pdfService;

        public PdfApi(ILogger<PdfApi> logger, IPdfService pdfService)
        {
            _logger = logger;
            _pdfService = pdfService;
        }

        [OpenApiOperation(operationId: "GetPdf", tags: new[] { "Pdf" })]
        [OpenApiParameter(name: "id")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.None)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(byte[]), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(BadRequestResult), Description = "The Bad Request response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(NotFoundResult), Description = "The Not Found response")]
        [Function("GetPdf")]
        public async Task<IActionResult> GetPdf([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdf/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Getting pdf for document with the id of {}.", id);

            if (string.IsNullOrWhiteSpace(id))
                return new BadRequestResult();

            var pdfByteArray = await _pdfService.CreatePdfByteArray(id);
            if (!pdfByteArray.Any())
                return new BadRequestResult();

            return new OkObjectResult(pdfByteArray);
        }
    }
}
