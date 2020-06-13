using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface IPostRepository
    {
        Task<EntityModels.Post> CreateAsync(EntityModels.Post post);
        Task<ICollection<EntityModels.Post>> GetLatestAsync(int? limit, long? previousPostCursor);
    }
}