using System;
using System.Drawing.Imaging;
using System.Threading.Tasks;
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

        public PostService(
            IPostRepository postRepository,
            IImageConverter imageConverter,
            IFileService fileService,
            ICurrentUtcDateProvider currentUtcDateProvider)
        {
            this.postRepository = postRepository;
            this.imageConverter = imageConverter;
            this.fileService = fileService;
            this.currentUtcDateProvider = currentUtcDateProvider;
        }

        public async Task<EntityModels.Post> CreateAsync(EntityModels.Post post, ImageDescriptor postImage)
        {
            var jpegBytes = imageConverter.ConvertToFormat(postImage.Content, ImageFormat.Jpeg);
            post.ImageUrl = await fileService.SaveFileAsync(GenerateRandomImageName(DesiredImageExtension), jpegBytes);
            post.CreatedAt = currentUtcDateProvider.UtcNow;
            try
            {
                return await postRepository.CreateAsync(post);
            }
            catch // execute for any exception type
            {
                await fileService.DeleteFileAsync(post.ImageUrl);
                throw;
            }
        }

        private string GenerateRandomImageName(string desiredExtension)
        {
            return Guid.NewGuid().ToString() + desiredExtension;
        }
    }
}