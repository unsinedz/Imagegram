using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using ApiModels = Imagegram.Api.Models.Api;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<ApiModels.AccountInput, EntityModels.Account>();
                config.CreateMap<EntityModels.Account, ApiModels.Account>();
            }).CreateMapper();
            services.AddSingleton<IMapper>(mapper);
            return services;
        }
    }
}