using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
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

        public async Task<ICollection<EntityModels.Comment>> GetByPostAsync(Guid postId, int? limit, long? previousCommentCursor)
        {
            using (var connection = OpenConnection())
            {
                var limitExpression = limit.HasValue && limit.Value > 0
                    ? $" top ({limit.Value})"
                    : "";
                var cursorExpression = previousCommentCursor.HasValue
                    ? $" and [{nameof(EntityModels.Comment.VersionCursor)}] > @previousCommentCursor"
                    : "";
                var comments = await connection.QueryAsync<EntityModels.Comment>(
                    $@"select{limitExpression} * from [dbo].[Comments]
                    where [{nameof(EntityModels.Comment.PostId)}] = @postId{cursorExpression}
                    order by {nameof(EntityModels.Comment.CreatedAt)} desc",
                    new { postId, limit, previousCommentCursor });
                return comments.AsList();
            }
        }
    }
}