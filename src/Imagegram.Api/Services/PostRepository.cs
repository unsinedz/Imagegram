using System;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public class PostRepository : RepositoryBase, IPostRepository
    {
        private readonly ICurrentUtcDateProvider currentUtcDateProvider;

        public PostRepository(
            ICurrentUtcDateProvider currentUtcDateProvider,
            IOptions<ConnectionStringOptions> connectionStringOptions,
            IDbConnectionFactory connectionFactory)
            : base(connectionStringOptions.Value.Default, connectionFactory)
        {
            this.currentUtcDateProvider = currentUtcDateProvider;
        }

        public async Task<EntityModels.Post> CreateAsync(EntityModels.Post post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            post.Id = Guid.NewGuid();
            post.CreatedAt = currentUtcDateProvider.UtcNow;

            using (var connection = OpenConnection())
            {
                await connection.InsertAsync(post);
                return post;
            }
        }
    }
}