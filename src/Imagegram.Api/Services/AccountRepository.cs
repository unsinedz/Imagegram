using System;
using System.Data;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string connectionString;
        private readonly IDbConnectionFactory connectionFactory;

        public AccountRepository(IOptions<ConnectionStringOptions> connectionStringOptions, IDbConnectionFactory connectionFactory)
        {
            this.connectionString = connectionStringOptions.Value.Default;
            this.connectionFactory = connectionFactory;
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

        private IDbConnection OpenConnection()
        {
            var connection = connectionFactory.Create(connectionString);
            connection.Open();
            return connection;
        }
    }
}