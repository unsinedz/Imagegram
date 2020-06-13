using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Imagegram.Api.Extensions;
using Microsoft.Extensions.Logging;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public class PostService : IPostService
    {
        private const string DesiredImageExtension = ".jpg";
        
        private readonly IPostRepository postRepository;
        private readonly IImageConverter imageConverter;
        private readonly IFileService fileService;
        private readonly ICurrentUtcDateProvider currentUtcDateProvider;
        private readonly ILogger<PostService> logger;

        public PostService(
            IPostRepository postRepository,
            IImageConverter imageConverter,
            IFileService fileService,
            ICurrentUtcDateProvider currentUtcDateProvider,
            ILogger<PostService> logger)
        {
            this.postRepository = postRepository;
            this.imageConverter = imageConverter;
            this.fileService = fileService;
            this.currentUtcDateProvider = currentUtcDateProvider;
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

        public async Task<ICollection<EntityModels.Post>> GetLatestAsync(int? limit, long? previousPostCursor)
        {
            return await postRepository.GetLatestAsync(limit, previousPostCursor);
        }

        private string GenerateRandomImageName(string desiredExtension)
        {
            return Guid.NewGuid().ToString() + desiredExtension;
        }
    }
}