using System;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public class CommentRepository : RepositoryBase, ICommentRepository
    {
        private readonly ICurrentUtcDateProvider currentUtcDateProvider;

        public CommentRepository(
            ICurrentUtcDateProvider currentUtcDateProvider,
            IOptions<ConnectionStringOptions> connectionStringOptions,
            IDbConnectionFactory connectionFactory)
            : base(connectionStringOptions.Value.Default, connectionFactory)
        {
            this.currentUtcDateProvider = currentUtcDateProvider;
        }

        public async Task<Guid> CreateAsync(EntityModels.Comment comment)
        {
            comment.Id = Guid.NewGuid();
            comment.CreatedAt = currentUtcDateProvider.UtcNow;
            using (var connection = OpenConnection())
            {
                await connection.InsertAsync(comment);
                return comment.Id;
            }
        }

        public async Task<EntityModels.Comment> GetAsync(Guid id)
        {
            using (var connection = OpenConnection())
            {
                return await connection.GetAsync<EntityModels.Comment>(id);
            }
        }
    }
}