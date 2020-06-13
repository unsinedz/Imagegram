using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Services;
using Microsoft.AspNetCore.Authorization;
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IMapper mapper;

        public AccountsController(IAccountService accountRepository, IMapper mapper)
        {
            this.accountService = accountRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiModels.Account> PostAsync([Required, FromBody] ApiModels.AccountInput accountInput)
        {
            var account = await accountService.CreateAsync(mapper.Map<EntityModels.Account>(accountInput));
            return mapper.Map<ApiModels.Account>(account);
        }
    }
}
