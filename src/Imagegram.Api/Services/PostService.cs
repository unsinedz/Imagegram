using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Extensions;
using Microsoft.Extensions.Logging;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
{
    public class PostService : IPostService
    {
        private const string DesiredImageExtension = ".jpg";
        
        private readonly IPostRepository postRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IImageConverter imageConverter;
        private readonly IFileService fileService;
        private readonly ICurrentUtcDateProvider currentUtcDateProvider;
        private readonly IMapper mapper;
        private readonly ILogger<PostService> logger;

        public PostService(
            IPostRepository postRepository,
            IAccountRepository accountRepository,
            IImageConverter imageConverter,
            IFileService fileService,
            ICurrentUtcDateProvider currentUtcDateProvider,
            IMapper mapper,
            ILogger<PostService> logger)
        {
            this.postRepository = postRepository;
            this.accountRepository = accountRepository;
            this.imageConverter = imageConverter;
            this.fileService = fileService;
            this.currentUtcDateProvider = currentUtcDateProvider;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<EntityModels.Post> CreateAsync(EntityModels.Post post, ImageDescriptor postImage)
        {
            var jpegBytes = imageConverter.ConvertToFormat(postImage.Content, ImageFormat.Jpeg);
            post.ImageUrl = (await fileService.SaveFileAsync(GenerateRandomImageName(DesiredImageExtension), jpegBytes)).ConvertToUrlPath();
            post.CreatedAt = currentUtcDateProvider.UtcNow;
            try
            {
                return await postRepository.CreateAsync(post);
            }
            catch (Exception e) // execute for any exception type
            {
                await RevertWithLog(e.ToString());
                throw;
            }

            async Task RevertWithLog(string details)
            {
                logger.LogError("Post was not saved. Details: {0}.", details);
                await fileService.DeleteFileAsync(post.ImageUrl);
            }
        }

        public async Task<ICollection<ProjectionModels.Post>> GetLatestAsync(int? limit, long? previousPostCursor)
        {
            var posts = await postRepository.GetLatestAsync(limit, previousPostCursor);
            var accounts = (await accountRepository.GetAsync(posts.Select(x => x.CreatorId).Distinct().ToArray()))
                .ToDictionary(x => x.Id);
            return posts.Select(x =>
            {
                var post = mapper.Map<ProjectionModels.Post>(x);
                post.Creator = mapper.Map<ProjectionModels.Account>(accounts[x.CreatorId]);
                return post;
            }).ToList();
        }

        private string GenerateRandomImageName(string desiredExtension)
        {
            return Guid.NewGuid().ToString() + desiredExtension;
        }
    }
}