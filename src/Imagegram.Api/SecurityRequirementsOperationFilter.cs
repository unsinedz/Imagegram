using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Imagegram.Api
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var allowAnonymousAttributes = GetDeclaredMethodAndTypeAttributes<AllowAnonymousAttribute>(context.MethodInfo);
            IEnumerable<string> requiredScopes;
            if (allowAnonymousAttributes.Count > 0)
                requiredScopes = new List<string>(0);
            else
            {
                var authorizeAttributes = GetDeclaredMethodAndTypeAttributes<AuthorizeAttribute>(context.MethodInfo);
                requiredScopes = authorizeAttributes
                    .Select(attr => attr.Policy)
                    .Distinct();
            }

            if (requiredScopes.Any())
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = Constants.Authorization.SecutirySchemeName
                    }
                };
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [ securityScheme ] = requiredScopes.ToList()
                    }
                };
            }
        }

        private ICollection<T> GetDeclaredMethodAndTypeAttributes<T>(MethodInfo methodInfo)
        {
            var methodAttributes = methodInfo
                .GetCustomAttributes(true)
                .OfType<T>();
            var typeAttributes = methodInfo.DeclaringType
                .GetCustomAttributes(true)
                .OfType<T>();
            return methodAttributes.Concat(typeAttributes).ToList();
        }
    }
}