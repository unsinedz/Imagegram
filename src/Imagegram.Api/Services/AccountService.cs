using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Exceptions;
using Imagegram.Api.Repositories;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            this.accountRepository = accountRepository;
            this.mapper = mapper;
        }

        public async Task<ProjectionModels.Account> CreateAsync(EntityModels.Account account)
        {
            var createdAccount = await accountRepository.CreateAsync(account);
            return mapper.Map<ProjectionModels.Account>(createdAccount);
        }

        public async Task<ProjectionModels.Account> DeleteAsync(Guid id)
        {
            var account = await accountRepository.GetAsync(id);
            if (account == null)
                throw new NotFoundException("Account was not found.");

            await accountRepository.DeleteAsync(id);
            return mapper.Map<ProjectionModels.Account>(account);
        }

        public async Task<ProjectionModels.Account> GetAsync(Guid id)
        {
            return await accountRepository.GetAsync(id);
        }
    }
}