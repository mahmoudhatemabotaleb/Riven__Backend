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
            await app.RunAsync();
        }
    }
}
