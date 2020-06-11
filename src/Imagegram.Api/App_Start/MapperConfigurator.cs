using AutoMapper;
using ApiModels = Imagegram.Api.Models.Api;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api
{
    public static class MapperConfigurator
    {
        public static void ConfigureMappings(IMapperConfigurationExpression config)
        {
            config.CreateMap<ApiModels.AccountInput, EntityModels.Account>();
            config.CreateMap<EntityModels.Account, ApiModels.Account>();

            config.CreateMap<ApiModels.PostInput, EntityModels.Post>();
            config.CreateMap<EntityModels.Post, ApiModels.Post>();
        }
    }
}