namespace Docs.UI.Interfaces
{
    public interface IDownloadService
    {
        Task<MemoryStream> GetFileMemoryStream(string id);
    }
}
