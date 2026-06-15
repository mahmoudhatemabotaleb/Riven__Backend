using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.Middleware;
using System.Text.Json;

namespace RivenBackend.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task UseRivenPipelineAsync(this WebApplication app)
        {
            if (app.Environment.IsProduction())
                app.UseHsts();

            await app.ApplyMigrationsInDevelopmentAsync();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<AuditMiddleware>();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/uploads"))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                await next();
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Riven API v1");
                    options.DocumentTitle = "Riven API";
                    options.ConfigObject.PersistAuthorization = true;
                    options.InjectJavascript("/swagger-auth-helper.js");
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AngularApp");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString()
                        })
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });

            app.MapControllers();
            app.MapHub<RivenBackend.Hubs.TransportHub>("/hubs/transport");
        }

        private static async Task ApplyMigrationsInDevelopmentAsync(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
                return;

            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                await db.Database.MigrateAsync();
                await DbSeeder.SeedAsync(db);
                logger.LogInformation("Database migrations and seed data applied successfully.");
            }
            catch (Exception ex)
            {
                var serverHint = db.Database.GetConnectionString()?.Split(';')
                    .FirstOrDefault(p => p.StartsWith("Server=", StringComparison.OrdinalIgnoreCase)
                        || p.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));

                logger.LogCritical(ex,
                    "Failed to connect to SQL Server ({Server}). " +
                    "Update ConnectionStrings:DefaultConnection in appsettings.Development.json. " +
                    "Try: Server=localhost or Server=YOUR_PC_NAME. " +
                    "Run 'sqlcmd -S localhost -E -Q \"SELECT @@SERVERNAME\"' to find your server.",
                    serverHint ?? "unknown");
                throw;
            }
        }
    }
}
