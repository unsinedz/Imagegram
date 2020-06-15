using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Imagegram.Api.Extensions;
using Imagegram.Api.Services;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Repositories
{
    public class CommentRepository : RepositoryBase, ICommentRepository
    {
        private readonly IMapper mapper;
        private readonly ICurrentUtcDateProvider currentUtcDateProvider;

        public CommentRepository(
            IMapper mapper,
            ICurrentUtcDateProvider currentUtcDateProvider,
            IOptions<ConnectionStringOptions> connectionStringOptions,
            IDbConnectionFactory connectionFactory)
            : base(connectionStringOptions.Value.Default, connectionFactory)
        {
            this.mapper = mapper;
            this.currentUtcDateProvider = currentUtcDateProvider;
        }

        public async Task<Guid> CreateAsync(EntityModels.Comment comment)
        {
            comment.Id = Guid.NewGuid();
            comment.CreatedAt = currentUtcDateProvider.UtcNow;
            using (var connection = OpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    await connection.InsertAsync(comment, transaction);
                    await connection.ExecuteAsync(@"
                        update [dbo].[Posts]
                        set [CommentsCount] = [CommentsCount] + 1
                        where [Id] = @postId",
                        new { postId = comment.PostId },
                        transaction);
                    transaction.Commit();
                }

                return comment.Id;
            }
        }

        public async Task<ProjectionModels.Comment> GetAsync(Guid id)
        {
            using (var connection = OpenConnection())
            {
                var comments = await connection.QueryAsync<EntityModels.Comment, EntityModels.Account, ProjectionModels.Comment>(
                    @"select c.[Id]
                        ,c.[Content]
                        ,c.[CreatedAt]
                        ,c.[ItemCursor]
                        ,a.[Id]
                        ,a.[Name]
                    from [dbo].[Comments] c
                    inner join [dbo].[Accounts] a on a.[Id] = c.[CreatorId]
                    where c.[Id] = @id",
                    MapEntitiesIntoProjection,
                    new { id }
                );
                return comments.SingleOrDefault();
            }
        }

        public async Task<ICollection<ProjectionModels.Comment>> GetByPostAsync(Guid postId, int? limit, long? previousCommentCursor)
        {
            using (var connection = OpenConnection())
            {
                var limitExpression = limit.HasValue && limit.Value > 0
                    ? $" top ({limit.Value})"
                    : "";
                var cursorExpression = previousCommentCursor.HasValue
                    ? $" and [{nameof(EntityModels.Comment.ItemCursor)}] > @previousCommentCursor"
                    : "";
                var comments = await connection.QueryAsync<EntityModels.Comment, EntityModels.Account, ProjectionModels.Comment>(
                    $@"select{limitExpression} c.[Id]
                        ,c.[Content]
                        ,c.[CreatedAt]
                        ,c.[ItemCursor]
                        ,a.Id
                        ,a.Name
                    from [dbo].[Comments] c
                    inner join [dbo].[Accounts] a on a.[Id] = c.[CreatorId]
                    where [{nameof(EntityModels.Comment.PostId)}] = @postId{cursorExpression}
                    order by {nameof(EntityModels.Comment.CreatedAt)} desc",
                    MapEntitiesIntoProjection,
                    new { postId, limit, previousCommentCursor }
                );
                return comments.AsList();
            }
        }

        public async Task<ICollection<EntityModels.Comment>> GetLatestForPostsAsync(ICollection<Guid> postIds, int? limit)
        {
            using (var connection = OpenConnection())
            {
                var comments = await connection.QueryAsync<EntityModels.Comment>(
                    "[dbo].[spSelectLastPostComments]",
                    new { commentLimit = limit, postIds = postIds.ToUdtIds() },
                    commandType: CommandType.StoredProcedure
                );
                return comments.AsList();
            }
        }

        private ProjectionModels.Comment MapEntitiesIntoProjection(EntityModels.Comment comment, EntityModels.Account account)
        {
            var projection = mapper.Map<ProjectionModels.Comment>(comment);
            projection.Creator = mapper.Map<ProjectionModels.Account>(account);
            return projection;
        }
    }
}