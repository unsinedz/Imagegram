using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
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
                    @"select p.*
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

        private ProjectionModels.Post MapEntitiesIntoProjection(EntityModels.Post post, EntityModels.Account account)
        {
            var projection = mapper.Map<ProjectionModels.Post>(post);
            projection.Creator = mapper.Map<ProjectionModels.Account>(account);
            return projection;
        }
    }
}