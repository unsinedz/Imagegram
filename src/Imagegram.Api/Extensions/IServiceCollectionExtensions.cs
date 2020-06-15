using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Imagegram.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Imagegram.Api.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var mapper = new MapperConfiguration(MapperConfigurator.ConfigureMappings).CreateMapper();
            services.AddSingleton<IMapper>(mapper);
            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(Constants.Api.Version, new OpenApiInfo
                {
                    Title = Constants.Api.Name,
                    Version = Constants.Api.Version
                });

                var headerName = Constants.Authentication.HeaderName;
                x.AddSecurityDefinition(Constants.Authentication.SecuritySchemeName, new OpenApiSecurityScheme
                {
                    Description = $"{headerName} header that contains user ID. Example: \"00000000-0000-0000-0000-000000000000\"",
                    In = ParameterLocation.Header,
                    Name = headerName,
                    Type = SecuritySchemeType.ApiKey
                });
                x.OperationFilter<SecurityRequirementsOperationFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });
            return services;
        }
    }
}