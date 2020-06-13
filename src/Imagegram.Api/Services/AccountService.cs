using System.Threading.Tasks;
using AutoMapper;
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
    }
}