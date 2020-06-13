using System;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface ICommentRepository
    {
        Task<Guid> CreateAsync(EntityModels.Comment comment);
        Task<EntityModels.Comment> GetAsync(Guid id);
    }
}