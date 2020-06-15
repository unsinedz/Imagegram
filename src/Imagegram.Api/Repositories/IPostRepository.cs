using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Repositories
{
    public interface IPostRepository
    {
        Task<Guid> CreateAsync(EntityModels.Post post);
        Task<ProjectionModels.Post> GetAsync(Guid id);
        Task<ICollection<ProjectionModels.Post>> GetLatestAsync(int? limit, long? previousPostCursor);
    }
}