using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ApiModels = Imagegram.Api.Models.Api;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api
{
    public static class MapperConfigurator
    {
        public static void ConfigureMappings(IMapperConfigurationExpression config)
        {
            config.CreateMap<ApiModels.AccountInput, EntityModels.Account>().ReverseMap();
            config.CreateMap<EntityModels.Account, ProjectionModels.Account>();
            config.CreateMap<ProjectionModels.Account, ApiModels.Account>();

            config.CreateMap<ApiModels.PostInput, EntityModels.Post>().ReverseMap();
            config.CreateMap<EntityModels.Post, ProjectionModels.Post>();
            config.CreateMap<ProjectionModels.Post, ApiModels.Post>()
                .ForMember(
                    x => x.Creator,
                    x => x.MapFrom((source, dest, _, context) => context.Mapper.Map<ApiModels.Account>(source.Creator)))
                .ForMember(
                    x => x.Comments,
                    x => x.MapFrom((source, dest, _, context)
                        => source.Comments?.Select(x => context.Mapper.Map<ApiModels.Comment>(x)).ToList()
                            ?? new List<ApiModels.Comment>(0)))
                .ForMember(x => x.Cursor, x => x.MapFrom(source => source.VersionCursor));

            config.CreateMap<ProjectionModels.Comment, ApiModels.Comment>()
                .ForMember(x => x.Cursor, x => x.MapFrom(source => source.VersionCursor));
        }
    }
}