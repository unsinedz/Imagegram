using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface ICommentRepository
    {
        Task<Guid> CreateAsync(EntityModels.Comment comment);
        Task<EntityModels.Comment> GetAsync(Guid id);
        Task<ICollection<EntityModels.Comment>> GetByPostAsync(Guid postId, int? limit, long? previousCommentCursor);
        Task<ICollection<EntityModels.Comment>> GetLatestForPostsAsync(ICollection<Guid> postIds, int? limit);
    }
}