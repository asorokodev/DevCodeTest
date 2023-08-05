namespace DevCodeTest.DataProviders.Interfaces
{
    public interface IStoriesDataProvider : IDisposable
    {
        Task<IEnumerable<int>?> GetBestStoriyIdsAsync(CancellationToken cancellationToken);
        Task<string> GetStoryAsync(int id, CancellationToken cancellationToken);
    }
}
