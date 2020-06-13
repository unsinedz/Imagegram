using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Imagegram.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Imagegram.Api.Services
{
    public class ImageService : IImageService
    {
        private readonly IReadOnlyList<string> supportedExtensions;
        private readonly int maxFileSizeKb;

        public ImageService(IOptions<PostOptions> postOptions)
        {
            var options = postOptions.Value;
            this.supportedExtensions = options.SupportedImageFormats
                .Split(";", StringSplitOptions.RemoveEmptyEntries);
            this.maxFileSizeKb = options.MaxFileSizeKb;
        }

        public async Task<ImageDescriptor> GetImageDescriptorAsync(IFormFile input)
        {
            var imageDescriptor = new ImageDescriptor
            {
                FileExtension = Path.GetExtension(input.FileName)
            };

            ValidateExtension(imageDescriptor.FileExtension);
            ValidateFileSize(input.Length);

            using (var stream = new MemoryStream())
            {
                await input.CopyToAsync(stream);
                imageDescriptor.Content = stream.ToArray();
            }

            return imageDescriptor;
        }

        private void ValidateExtension(string extension)
        {
            if (!supportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("File format is not supported.");
        }

        private void ValidateFileSize(long bytesCount)
        {
            var kbSize = bytesCount / (1 << 10);
            if (kbSize > maxFileSizeKb)
                throw new ArgumentException("File size is too large.");
        }
    }
}