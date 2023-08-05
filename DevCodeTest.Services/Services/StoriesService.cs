using DevCodeTest.DataProviders.Interfaces;
using DevCodeTest.Services.Interfaces;
using DevCodeTest.Services.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevCodeTest.Services.Services
{
    public sealed class StoriesService : IStoriesService
    {
        private readonly ILogger<StoriesService> _logger;
        private readonly IStoriesDataProvider _storiesDataProvider;
        private readonly RequestProcessingOptions _requestProcessingOptions;

        private IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(1))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(1);

        public StoriesService(
            IMemoryCache cache,
            ILogger<StoriesService> logger,
            IOptions<RequestProcessingOptions> requestProcessingOptions,
            IStoriesDataProvider storiesDataProvider)
        {
            _cache = cache;
            _logger = logger;
            _requestProcessingOptions = requestProcessingOptions.Value;
            _storiesDataProvider = storiesDataProvider;
        }

        public async Task<IReadOnlyCollection<string>> GetBestStoriesAsync(int number, CancellationToken cancellationToken = default)
        {
            var storyIds = await _storiesDataProvider.GetBestStoriyIdsAsync(cancellationToken);
            if (storyIds == null)
                return new List<string>();

            var requestedStoryIds = storyIds.Take(number);

            var semaphoreSlim = new SemaphoreSlim(_requestProcessingOptions.MaxParallelRequest, _requestProcessingOptions.MaxParallelRequest);
            
            var tasks = new List<Task<string?>>();
            foreach (var id in requestedStoryIds)
            {
                tasks.Add(GetStoryAsync(id, semaphoreSlim, _cache, cancellationToken));
            }

            var result = (await Task.WhenAll(tasks)).Where(x => !string.IsNullOrEmpty(x)).Select(x => x!).ToList();
            return result;
        }

        private async Task<string?> GetStoryAsync(int id, SemaphoreSlim semaphoreSlim, IMemoryCache cache, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(id, out string? story))
                return story;

            try
            {
                await semaphoreSlim.WaitAsync();
                var receivedStory = await _storiesDataProvider.GetStoryAsync(id, cancellationToken);
                if (!string.IsNullOrEmpty(receivedStory))
                {
                    _cache.Set(id, receivedStory!, _cacheEntryOptions);
                    return receivedStory;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                semaphoreSlim.Release();
            }

            return null;
        }
    }
}