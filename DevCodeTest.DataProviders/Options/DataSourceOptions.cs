using System.ComponentModel.DataAnnotations;

namespace DevCodeTest.DataProviders.Options
{
    public sealed class DataSourceOptions
    {
        public const string SectionName = "DataSource";

        [Required]
        [Url]
        public string NewsBaseUrl { get; set; } = string.Empty;

        [Required]
        public string BestStoriesIdsRoute { get; set; } = string.Empty;

        [Required]
        public string BestStoriesItemRoute { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int RetryNumber { get; set; } = 1;

        [Required]
        [Range(1, 10000)]
        public int RateLimit { get; set; } = 50;
    }
}
