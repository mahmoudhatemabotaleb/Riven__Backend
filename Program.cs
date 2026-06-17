using RivenBackend.Extensions;
namespace RivenBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRivenServices(builder.Configuration);
            var app = builder.Build();
            await app.UseRivenPipelineAsync();

            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            await app.RunAsync($"http://0.0.0.0:{port}");
        }
    }
}