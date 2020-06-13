using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
{
    public interface IPostService
    {
        Task<ProjectionModels.Post> CreateAsync(EntityModels.Post post, ImageDescriptor postImage);
        Task<ICollection<ProjectionModels.Post>> GetLatestAsync(int? limit, long? previousPostCursor);
    }
}