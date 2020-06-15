using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Imagegram.Api.Services;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Repositories
{
    public class PostRepository : RepositoryBase, IPostRepository
    {
        private readonly IMapper mapper;
        private readonly ICurrentUtcDateProvider currentUtcDateProvider;

        public PostRepository(
            IMapper mapper,
            ICurrentUtcDateProvider currentUtcDateProvider,
            IOptions<ConnectionStringOptions> connectionStringOptions,
            IDbConnectionFactory connectionFactory)
            : base(connectionStringOptions.Value.Default, connectionFactory)
        {
            this.mapper = mapper;
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

        public async Task<ProjectionModels.Post> GetAsync(Guid id)
        {
            using (var connection = OpenConnection())
            {
                var posts = await connection.QueryAsync<EntityModels.Post, EntityModels.Account, ProjectionModels.Post>(
                    @"select p.[Id]
                        ,p.[ImageUrl]
                        ,p.[CreatedAt]
                        ,p.[ItemCursor]
                        ,a.[Id]
                        ,a.[Name]
                    from [dbo].[Posts] p
                    inner join [dbo].[Accounts] a on a.[Id] = p.[CreatorId]
                    where p.[Id] = @id",
                    MapEntitiesIntoProjection,
                    new { id }
                );
                return posts.AsList().SingleOrDefault();
            }
        }

        public async Task<ICollection<ProjectionModels.Post>> GetLatestAsync(int? limit, long? previousPostCursor)
        {
            using (var connection = OpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var limitExpression = limit.HasValue && limit.Value > 0
                        ? $" top (@limit)"
                        : "";
                    var cursorExpression = previousPostCursor.HasValue
                        ? $"where [{nameof(EntityModels.Post.ItemCursor)}] > @previousPostCursor"
                        : "";
                    var posts = await connection.QueryAsync<EntityModels.Post, EntityModels.Account, EntityModels.Comment, EntityModels.Account, ProjectionModels.Post>(
                        $@"select{limitExpression} p.[Id]
                            ,p.[ImageUrl]
                            ,p.[CreatedAt]
                            ,p.[ItemCursor]
                            ,a.[Id] as [PostCreatorId]
                            ,a.[Name] as [PostCreatorName]
                        into #LatestPostsWithCreators
                        from [dbo].[Posts] p
                        inner join [dbo].[Accounts] a on a.[Id] = p.[CreatorId]
		                {cursorExpression}
		                order by [CommentsCount] desc, [CreatedAt] desc;

                        select [Id]
                            ,[ImageUrl]
                            ,[CreatedAt]
                            ,[ItemCursor]
                            ,[PostCreatorId] as [Id]
                            ,[PostCreatorName] as [Name]
                            ,[CommentId] as [Id]
                        	,[CommentContent] as [Content]
                        	,[CommentCreatedAt] as [CreatedAt]
	                    	,[CommentItemCursor] as [ItemCursor]
                        	,[CommentCreatorId] as [Id]
                        	,[CommentCreatorName] as [Name]
                        from (
                            select p.[Id]
                                ,p.[ImageUrl]
                                ,p.[CreatedAt]
                                ,p.[ItemCursor]
                                ,p.[PostCreatorId]
                                ,p.[PostCreatorName]
                                ,c.[Id] as [CommentId]
                        		,c.[Content] as [CommentContent]
                        		,c.[CreatedAt] as [CommentCreatedAt]
	                    		,c.[ItemCursor] as [CommentItemCursor]
                        		,row_number() over (partition by p.[Id] order by c.[CreatedAt] desc) as [CommentRank]
                                ,a.[Id] as [CommentCreatorId]
                                ,a.[Name] as [CommentCreatorName]
                        	from #LatestPostsWithCreators p
                        	inner join [dbo].[Comments] c on p.[Id] = c.[PostId]
                            inner join [dbo].[Accounts] a on a.[Id] = c.[CreatorId]
                        ) as RankedComments
                        where [CommentRank] <= @perPostCommentLimit;

                        truncate table #LatestPostsWithCreators;",
                        (post, postCreator, comment, commentCreator) =>
                        {
                            var postProjection = MapEntitiesIntoProjection(post, postCreator);
                            postProjection.Comments = new List<ProjectionModels.Comment>
                            {
                            MapEntitiesIntoProjection(comment, commentCreator)
                            };
                            return postProjection;
                        },
                        new { limit, previousPostCursor, perPostCommentLimit = 3 },
                        transaction);
                    transaction.Commit();
                    return posts.GroupBy(x => x.Id)
                        .Select(x => x.Aggregate((acc, current) =>
                        {
                            acc.Comments.Add(current.Comments.Single());
                            return acc;
                        }))
                        .AsList();
                }
            }
        }

        private ProjectionModels.Post MapEntitiesIntoProjection(EntityModels.Post post, EntityModels.Account account)
        {
            var projection = mapper.Map<ProjectionModels.Post>(post);
            projection.Creator = mapper.Map<ProjectionModels.Account>(account);
            return projection;
        }

        private ProjectionModels.Comment MapEntitiesIntoProjection(EntityModels.Comment comment, EntityModels.Account account)
        {
            var projection = mapper.Map<ProjectionModels.Comment>(comment);
            projection.Creator = mapper.Map<ProjectionModels.Account>(account);
            return projection;
        }
    }
}