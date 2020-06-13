using System;
using System.IO;
using System.Reflection;
using AutoMapper;
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

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });
            return services;
        }
    }
}