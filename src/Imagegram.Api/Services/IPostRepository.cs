using System.Collections.Generic;
using System.Threading.Tasks;
using Imagegram.Api.Models.Entity;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface IPostRepository
    {
        Task<EntityModels.Post> CreateAsync(EntityModels.Post post);
        Task<ICollection<Post>> GetLatestAsync(int? limit, long? previousPostCursor);
    }
}