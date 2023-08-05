using DevCodeTest.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DevCodeTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BestStoriesController : ControllerBase
    {
        private readonly IStoriesService _storiesService;

        public BestStoriesController(
            IStoriesService newsDataProvider
            )
        {
            _storiesService = newsDataProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get([Required] [Range(1, 1000)] int number)
        {
            var data = await _storiesService.GetBestStoriesAsync(number);
            return data;
        }
    }
}