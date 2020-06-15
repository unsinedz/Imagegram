using System.Data;
using Microsoft.Data.SqlClient;

namespace Imagegram.Api.Services
{
    public class MsSqlConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}