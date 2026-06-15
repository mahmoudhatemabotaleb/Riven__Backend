using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RivenBackend.Extensions
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorizeOnMethod = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
            var hasAuthorizeOnController = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ?? false;
            var hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if ((hasAuthorizeOnMethod || hasAuthorizeOnController) && !hasAllowAnonymous)
            {
                operation.Security =
                [
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }] = Array.Empty<string>()
                    }
                ];
            }
        }
    }
}
