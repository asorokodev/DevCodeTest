using DevCodeTest.DataProviders;
using DevCodeTest.DataProviders.Options;
using DevCodeTest.Services;
using DevCodeTest.Services.Options;

namespace DevCodeTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOptions<DataSourceOptions>()
                .BindConfiguration(DataSourceOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<RequestProcessingOptions>()
                .BindConfiguration(RequestProcessingOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddMemoryCache();
            builder.Services.AddDataProviders();
            builder.Services.AddInternalServices();

            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddProblemDetails();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            
            app.MapControllers();

            app.Run();
        }
    }
}