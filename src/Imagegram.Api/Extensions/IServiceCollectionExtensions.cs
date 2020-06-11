using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

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
    }
}