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
            });
            return services;
        }
    }
}