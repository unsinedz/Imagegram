using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiModels = Imagegram.Api.Models.Api;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;

        public AccountsController(IAccountRepository accountRepository, IMapper mapper)
        {
            this.accountRepository = accountRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ApiModels.Account>> PostAsync([Required, FromBody] ApiModels.AccountInput accountInput)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var account = await accountRepository.CreateAsync(mapper.Map<EntityModels.Account>(accountInput));
            return mapper.Map<ApiModels.Account>(account);
        }
    }
}
