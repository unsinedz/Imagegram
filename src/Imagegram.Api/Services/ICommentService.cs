using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface ICommentService
    {
        Task<EntityModels.Comment> CreateAsync(EntityModels.Comment comment);
    }
}