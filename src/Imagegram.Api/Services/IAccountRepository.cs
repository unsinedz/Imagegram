using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public interface IAccountRepository
    {
        Task<EntityModels.Account> CreateAsync(EntityModels.Account account);
        Task<ICollection<EntityModels.Account>> GetAsync(params Guid[] ids);
    }
}