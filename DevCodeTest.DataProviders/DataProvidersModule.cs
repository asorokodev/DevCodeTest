using DevCodeTest.DataProviders.Interfaces;
using DevCodeTest.DataProviders.Providers;
using Microsoft.Extensions.DependencyInjection;


namespace DevCodeTest.DataProviders
{
    public static class DataProvidersModule
    {
        public static void AddDataProviders(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IStoriesDataProvider, StoriesDataProvider>();
        }
    }
}
