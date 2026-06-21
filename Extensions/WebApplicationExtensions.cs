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
            await app.ApplyMigrationsAsync();
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

        private static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                var isPostgres = db.Database.IsNpgsql();
                var isSqlServer = db.Database.IsSqlServer();

                logger.LogInformation("Applying migrations for {Provider}...",
                    isPostgres ? "PostgreSQL" : "SQL Server");

                await db.Database.MigrateAsync();
                await DbSeeder.SeedAsync(db);

                logger.LogInformation("Database migrations and seed data applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to apply database migrations.");
                throw;
            }
        }
    }
}