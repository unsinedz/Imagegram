using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Repositories
{
    public interface IAccountRepository
    {
        Task<Guid> CreateAsync(EntityModels.Account account);
        Task DeleteAsync(Guid id);
        Task<ProjectionModels.Account> GetAsync(params Guid[] ids);
    }
}