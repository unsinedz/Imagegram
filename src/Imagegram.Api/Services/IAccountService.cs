using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
{
    public interface IAccountService
    {
        Task<ProjectionModels.Account> CreateAsync(EntityModels.Account account);
    }
}