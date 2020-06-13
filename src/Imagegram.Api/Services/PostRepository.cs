using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
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

        public async Task<ICollection<EntityModels.Post>> GetLatestAsync(int limit, long previousPostCursor)
        {
            using (var connection = OpenConnection())
            {
                var posts = await connection.QueryAsync<EntityModels.Post>(
                    "exec [dbo].[spSelectLatestPosts] @limit=@_limit @previousPostCursor=@_previousPostCursor",
                    new { _limit = limit, _previousPostCursor = previousPostCursor },
                    commandType: CommandType.StoredProcedure);
                return posts.AsList();
            }
        }
    }
}