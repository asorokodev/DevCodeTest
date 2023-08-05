using DevCodeTest.DataProviders.Interfaces;
using DevCodeTest.DataProviders.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using Polly.Wrap;
using System.Text.Json;

namespace DevCodeTest.DataProviders.Providers
{
    internal sealed class StoriesDataProvider : IStoriesDataProvider, IDisposable
    {
        private const string ServiceUnavailableErrorCode = "503";
        private const int WaitBeforeRetryMs = 1000;
        private const int TimeoutSec = 3;
        private readonly AsyncPolicyWrap _policyWrap;

        private readonly ILogger<StoriesDataProvider> _logger;
        private readonly DataSourceOptions _dataSourceOptions;
        private readonly HttpClient _httpClient;

        public StoriesDataProvider(
            ILogger<StoriesDataProvider> logger,
            IOptions<DataSourceOptions> dataSourceOptions
            )
        {
            _logger = logger;
            _dataSourceOptions = dataSourceOptions.Value;

            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_dataSourceOptions.BaseUrl),
            };

            var retryPolicy = Policy
                .Handle<HttpRequestException>(ex => ex.Message.Contains(ServiceUnavailableErrorCode))
                .WaitAndRetryAsync(
                    _dataSourceOptions.RetryNumber,
                    _ => TimeSpan.FromMilliseconds(WaitBeforeRetryMs),
                    (result, timespan, retryNo, context) =>
                    {
                        _logger.LogWarning($"{context.OperationKey}: Retry number {retryNo} within " +
                            $"{timespan.TotalMilliseconds}ms. Original status code: 503");
                    }
                );

            var timeoutPolicy = Policy.TimeoutAsync(TimeoutSec, TimeoutStrategy.Pessimistic,
                (context, timeout, _, _) =>
                {
                    _logger.LogWarning($"{context.OperationKey}: Timeout {timeout.Milliseconds} ms.");
                    return Task.CompletedTask;
                });

            var rateLimit = Policy.RateLimitAsync(_dataSourceOptions.RateLimit, TimeSpan.FromSeconds(1), 10);

            _policyWrap = Policy.WrapAsync(retryPolicy, rateLimit, timeoutPolicy);
        }

        public async Task<IEnumerable<int>?> GetBestStoriyIdsAsync(CancellationToken cancellationToken)
        {
            var response = await _policyWrap.ExecuteAsync(async ctx =>
            {
                var response = await _httpClient.GetAsync(_dataSourceOptions.BestStoriesIdsRoute, cancellationToken);
                response.EnsureSuccessStatusCode();
                return response;
            }, cancellationToken);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var ids = JsonSerializer.Deserialize<int[]>(jsonResponse);

            return ids;
        }

        public async Task<string> GetStoryAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _policyWrap.ExecuteAsync(async ctx =>
            {
                var response = await _httpClient.GetAsync(string.Format(_dataSourceOptions.ItemDetailsRoute, id), cancellationToken);
                response.EnsureSuccessStatusCode();
                return response;
            }, cancellationToken);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}