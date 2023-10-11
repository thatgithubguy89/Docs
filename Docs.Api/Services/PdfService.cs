using Docs.Api.Interfaces;

namespace Docs.Api.Services
{
    public class PdfService : IPdfService
    {
        private readonly IDocumentRepository _documentRepository;

        public PdfService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        // Gets a document, converts it to a pdf and returns it in a byte array
        public async Task<byte[]> CreatePdfByteArray(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var renderer = new ChromePdfRenderer();
            var document = await _documentRepository.GetDocumentByIdAsync(id);
            if (document == null)
                throw new NullReferenceException(nameof(document));

            if (string.IsNullOrWhiteSpace(document.Content))
                return Array.Empty<byte>();

            var pdf = renderer.RenderHtmlAsPdf(document.Content);

            return pdf.Stream.ToArray();
        }
    }
}
