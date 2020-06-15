using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Imagegram.Api.Services;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Repositories
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {
        public AccountRepository(IOptions<ConnectionStringOptions> connectionStringOptions, IDbConnectionFactory connectionFactory)
            : base(connectionStringOptions.Value.Default, connectionFactory)
        {
        }

        public async Task<Guid> CreateAsync(EntityModels.Account account)
        {
            if (account is null)
                throw new ArgumentNullException(nameof(account));

            account.Id = Guid.NewGuid();

            using (var connection = OpenConnection())
            {
                await connection.InsertAsync(account);
                return account.Id;
            }
        }

        public async Task<ProjectionModels.Account> GetAsync(params Guid[] ids)
        {
            using (var connection = OpenConnection())
            {
                var accounts = await connection.QueryAsync<ProjectionModels.Account>(
                    @"select * from [dbo].[Accounts]
                    where [Id] in @ids",
                    new { ids });
                return accounts.SingleOrDefault();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var connection = OpenConnection())
            {
                await connection.ExecuteAsync(
                    @"delete from [dbo].[Accounts]
                    where [Id] = @id",
                    new { id });
            }
        }
    }
}