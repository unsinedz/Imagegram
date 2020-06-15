using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Extensions;
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
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IMapper mapper;

        public AccountsController(IAccountService accountService, IMapper mapper)
        {
            this.accountService = accountService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Creates new account.
        /// </summary>
        /// <param name="accountInput">The account input data.</param>
        /// <returns>An instance of created account.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiModels.Account> PostAsync([Required, FromBody] ApiModels.AccountInput accountInput)
        {
            var account = await accountService.CreateAsync(mapper.Map<EntityModels.Account>(accountInput));
            return mapper.Map<ApiModels.Account>(account);
        }

        /// <summary>
        /// Deletes current account.
        /// </summary>
        /// <returns>An instance of deleted account.</returns>
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ApiModels.Account> DeleteAsync()
        {
            var accountId = User.GetId();
            var account = await accountService.DeleteAsync(accountId);
            return mapper.Map<ApiModels.Account>(account);
        }
    }
}
