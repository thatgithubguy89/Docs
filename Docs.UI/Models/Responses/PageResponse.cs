namespace Docs.UI.Models.Responses
{
    public class PageResponse<T>
    {
        public List<T> Payload { get; set; }
        public int StartingIndex { get; set; }
        public int PageTotal { get; set; }
    }
}
