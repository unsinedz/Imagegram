using System.Threading.Tasks;
using Imagegram.Api.Models.Entity;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface IAccountRepository
    {
        Task<EntityModels.Account> CreateAsync(EntityModels.Account account);
    }
}