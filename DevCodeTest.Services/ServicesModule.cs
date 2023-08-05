using DevCodeTest.Services.Interfaces;
using DevCodeTest.Services.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace DevCodeTest.Services
{
    public static class ServicesModule
    {
        public static void AddInternalServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStoriesService, StoriesService>();
        }
    }
}
