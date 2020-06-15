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

        public async Task<Guid> CreateAsync(EntityModels.Post post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            post.Id = Guid.NewGuid();
            post.CreatedAt = currentUtcDateProvider.UtcNow;

            using (var connection = OpenConnection())
            {
                await connection.InsertAsync(post);
                return post.Id;
            }
        }

        public async Task<EntityModels.Post> GetAsync(Guid id)
        {
            using (var connection = OpenConnection())
            {
                return await connection.GetAsync<EntityModels.Post>(id);
            }
        }

        public async Task<ICollection<EntityModels.Post>> GetLatestAsync(int? limit, long? previousPostCursor)
        {
            using (var connection = OpenConnection())
            {
                var limitExpression = limit.HasValue && limit.Value > 0
                    ? $" top (@limit)"
                    : "";
                var cursorExpression = previousPostCursor.HasValue
                    ? $"where [{nameof(EntityModels.Post.VersionCursor)}] > @previousPostCursor"
                    : "";
                var posts = await connection.QueryAsync<EntityModels.Post>(
                    $@"select{limitExpression} * from [dbo].[Posts]
		            {cursorExpression}
		            order by [CommentsCount] desc, [CreatedAt] desc",
                    new { limit, previousPostCursor });
                return posts.AsList();
            }
        }
    }
}