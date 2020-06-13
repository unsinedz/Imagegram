using System.Collections.Generic;
using System.Threading.Tasks;
using Imagegram.Api.Models.Entity;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface IPostService
    {
        Task<EntityModels.Post> CreateAsync(EntityModels.Post post, ImageDescriptor postImage);
        Task<ICollection<Post>> GetLatestAsync(int? limit, long? previousPostCursor);
    }
}