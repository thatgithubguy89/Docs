namespace Docs.UI.Models.Responses
{
    public class OkObjectResultMultipleValues<T> where T : class
    {
        public List<T>? Value { get; set; }
    }
}