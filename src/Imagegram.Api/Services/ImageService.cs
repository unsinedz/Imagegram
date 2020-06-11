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
            ValidateExtension(Path.GetExtension(input.FileName));
            ValidateFileSize(input.Length);

            var imageDescriptor = new ImageDescriptor
            {
                FileName = input.FileName
            };
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
                throw new InvalidFileException("File format is not supported.");
        }

        private void ValidateFileSize(long bytesCount)
        {
            var kbSize = bytesCount / (1 << 10);
            if (kbSize > maxFileSizeKb)
                throw new InvalidFileException("File size is too large.");
        }
    }
}