using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface ICommentService
    {
        Task<EntityModels.Comment> CreateAsync(EntityModels.Comment comment);
        Task<ICollection<EntityModels.Comment>> GetAllAsync(Guid postId, int? limit, long? previousCommentCursor);
    }
}