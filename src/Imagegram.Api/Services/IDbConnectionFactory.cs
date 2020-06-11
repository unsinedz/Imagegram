using System.Data;

namespace Imagegram.Api.Services
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create(string connectionString);
    }
}