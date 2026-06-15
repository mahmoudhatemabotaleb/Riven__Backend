using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RivenBackend.Data;
using RivenBackend.Repositories;
using RivenBackend.Security;
using RivenBackend.Services;
using System.Text;
using System.Text.Json;

namespace RivenBackend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRivenServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSignalR();
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .SelectMany(e => e.Value!.Errors.Select(err => err.ErrorMessage))
                            .ToList();

                        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                            new Models.ApiResponse { Success = false, Message = string.Join("; ", errors) });
                    };
                });
            services.AddEndpointsApiExplorer();
            services.AddHttpClient();
            services.AddSwaggerWithJwt();
            services.AddRivenDatabase(configuration);
            services.AddRivenAuthentication(configuration);
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICaseRepository, CaseRepository>();
            services.AddScoped<IAiReportRepository, AiReportRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICaseAccessService, CaseAccessService>();
            services.AddScoped<FileUploadValidator>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICaseWorkflowService, CaseWorkflowService>();
            services.AddScoped<ITransportService, TransportService>();
            services.AddScoped<IFinalReviewService, FinalReviewService>();
            services.AddScoped<IAiReportService, AiReportService>();
            services.AddScoped<IRealtimeTrackingService, RealtimeTrackingService>();
            services.AddScoped<IOtpRateLimitService, OtpRateLimitService>();
            services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>("database");

            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:4200", "http://127.0.0.1:4200"];

            services.AddCors(options =>
            {
                options.AddPolicy("AngularApp", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

        private static void AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "1) Run POST /api/auth/login  2) Copy data.token from response  3) Click Authorize and paste token only (no 'Bearer' prefix)."
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        private static void AddRivenDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = ResolveConnectionString(configuration);

                if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
                    || connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(connectionString);
                    var userInfo = uri.UserInfo.Split(':');
                    var pgConnection =
                        $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
                    options.UseNpgsql(pgConnection);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }
            });
        }

        private static void AddRivenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                                context.Token = accessToken;
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            var hasAuthHeader = !string.IsNullOrEmpty(context.Request.Headers.Authorization);
                            var message = hasAuthHeader
                                ? "Invalid or expired token. Login again via POST /api/auth/login."
                                : "Authorization required. Login via POST /api/auth/login and send header: Authorization: Bearer {token}";

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            var payload = JsonSerializer.Serialize(new { success = false, message });
                            return context.Response.WriteAsync(payload);
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        private static string ResolveConnectionString(IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "No database connection string configured. Set DATABASE_URL or ConnectionStrings:DefaultConnection.");
            }

            return connectionString;
        }
    }
}
