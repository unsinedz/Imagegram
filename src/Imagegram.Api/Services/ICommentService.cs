using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
{
    public interface ICommentService
    {
        Task<ProjectionModels.Comment> CreateAsync(EntityModels.Comment comment);
        Task<ICollection<ProjectionModels.Comment>> GetAsync(Guid postId, int? limit, long? previousCommentCursor);
    }
}