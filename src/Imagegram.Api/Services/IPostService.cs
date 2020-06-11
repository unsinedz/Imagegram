using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface IPostService
    {
        Task<EntityModels.Post> CreateAsync(EntityModels.Post post, ImageDescriptor postImage);
    }
}