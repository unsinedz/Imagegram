using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
{
    public class PostService : IPostService
    {
        private const string DesiredImageExtension = ".jpg";

        private readonly IPostRepository postRepository;
        private readonly IAccountRepository accountRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IImageConverter imageConverter;
        private readonly IFileService fileService;
        private readonly ICurrentUtcDateProvider currentUtcDateProvider;
        private readonly IMapper mapper;
        private readonly IOptionsMonitor<PostOptions> postOptions;
        private readonly ILogger<PostService> logger;

        public PostService(
            IPostRepository postRepository,
            IAccountRepository accountRepository,
            ICommentRepository commentRepository,
            IImageConverter imageConverter,
            IFileService fileService,
            ICurrentUtcDateProvider currentUtcDateProvider,
            IMapper mapper,
            IOptionsMonitor<PostOptions> postOptions,
            ILogger<PostService> logger)
        {
            this.postRepository = postRepository;
            this.accountRepository = accountRepository;
            this.commentRepository = commentRepository;
            this.imageConverter = imageConverter;
            this.fileService = fileService;
            this.currentUtcDateProvider = currentUtcDateProvider;
            this.mapper = mapper;
            this.postOptions = postOptions;
            this.logger = logger;
        }

        public async Task<ProjectionModels.Post> CreateAsync(EntityModels.Post post, ImageDescriptor postImage)
        {
            var jpegBytes = imageConverter.ConvertToFormat(postImage.Content, ImageFormat.Jpeg);
            post.ImageUrl = (await fileService.SaveFileAsync(GenerateRandomImageName(DesiredImageExtension), jpegBytes))
                .ConvertToUrlPath();
            post.CreatedAt = currentUtcDateProvider.UtcNow;

            Guid createdId;
            try
            {
                createdId = await postRepository.CreateAsync(post);
            }
            catch (Exception e) // execute for any exception type
            {
                logger.LogError("Post was not saved. Details: {0}.", e.ToString());
                await fileService.DeleteFileAsync(post.ImageUrl);
                throw;
            }

            return await postRepository.GetAsync(createdId);
        }

        public async Task<ICollection<ProjectionModels.Post>> GetAsync(int? limit, long? previousPostCursor)
        {
            var posts = await postRepository.GetLatestAsync(limit, previousPostCursor);
            var comments = await commentRepository.GetLatestForPostsAsync(
                posts.Select(x => x.Id).Distinct().ToList(),
                postOptions.CurrentValue.FetchCommentsLimit);

            var postCreatorIds = posts.Select(x => x.CreatorId);
            var commentCreatorIds = comments.Select(x => x.CreatorId);

            var commentsByPost = comments.GroupBy(x => x.PostId).ToDictionary(x => x.Key);
            var accounts = (await accountRepository.GetAsync(postCreatorIds
                .Concat(commentCreatorIds)
                .Distinct()
                .ToArray())).ToDictionary(x => x.Id);
            return posts.Select(x =>
            {
                var post = mapper.Map<ProjectionModels.Post>(x);
                post.Creator = mapper.Map<ProjectionModels.Account>(accounts[x.CreatorId]);
                post.Comments = commentsByPost.GetValueOrDefault(x.Id)?.Select(c =>
                {
                    var comment = mapper.Map<ProjectionModels.Comment>(c);
                    comment.Creator = mapper.Map<ProjectionModels.Account>(accounts[c.CreatorId]);
                    return comment;
                }).ToList();
                return post;
            }).ToList();
        }

        private string GenerateRandomImageName(string desiredExtension)
        {
            return Guid.NewGuid().ToString() + desiredExtension;
        }
    }
}