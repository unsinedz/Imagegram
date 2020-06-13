using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {
        public AccountRepository(IOptions<ConnectionStringOptions> connectionStringOptions, IDbConnectionFactory connectionFactory)
            : base(connectionStringOptions.Value.Default, connectionFactory)
        {
        }

        public async Task<EntityModels.Account> CreateAsync(EntityModels.Account account)
        {
            if (account is null)
                throw new ArgumentNullException(nameof(account));

            account.Id = Guid.NewGuid();

            using (var connection = OpenConnection())
            {
                await connection.InsertAsync(account);
                return account;
            }
        }

        public async Task<ICollection<EntityModels.Account>> GetAsync(params Guid[] ids)
        {
            using (var connection = OpenConnection())
            {
                var accounts = await connection.QueryAsync<EntityModels.Account>(
                    "select * from [dbo].[Accounts] where [Id] in @ids",
                    new { ids });
                return accounts.AsList();
            }
        }
    }
}