namespace Docs.Api.Interfaces
{
    public interface ICachingService<T> where T : class
    {
        void DeleteItems(string key);
        T GetItem(string key);
        List<T> GetItems(string key);
        void SetItem(string key, T item);
        void SetItems(string key, List<T> items);
    }
}
