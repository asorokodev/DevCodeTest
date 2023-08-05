
namespace DevCodeTest.Services.Interfaces
{
    public interface IStoriesService
    {
        Task<IReadOnlyCollection<string>> GetBestStoriesAsync(int number, CancellationToken cancellationToken = default);
    }
}
