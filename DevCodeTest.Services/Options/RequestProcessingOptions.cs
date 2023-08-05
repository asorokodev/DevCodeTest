using System.ComponentModel.DataAnnotations;

namespace DevCodeTest.Services.Options
{
    public sealed class RequestProcessingOptions
    {
        public const string SectionName = "RequestProcessing";

        [Required]
        [Range(1, 1000)]
        public int MaxParallelRequest { get; set; } = 1;
    }
}
