using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Repositories
{
    public interface ICommentRepository
    {
        Task<Guid> CreateAsync(EntityModels.Comment comment);
        Task<ProjectionModels.Comment> GetAsync(Guid id);
        Task<ICollection<ProjectionModels.Comment>> GetByPostAsync(Guid postId, int? limit, long? previousCommentCursor);
        Task<ICollection<EntityModels.Comment>> GetLatestForPostsAsync(ICollection<Guid> postIds, int? limit);
    }
}