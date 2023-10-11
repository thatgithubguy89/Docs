namespace Docs.Api.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> CreatePdfByteArray(string id);
    }
}
