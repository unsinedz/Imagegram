using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Imagegram.Api.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IOptionsMonitor<FileStorageOptions> fileStorageOptions;

        public FileService(IWebHostEnvironment webHostEnvironment, IOptionsMonitor<FileStorageOptions> fileStorageOptions)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.fileStorageOptions = fileStorageOptions;
        }
        
        /// <summary>
        /// Saves file to storage.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="content">The byte content.</param>
        /// <returns>Relative path to the file.</returns>
        public async Task<string> SaveFileAsync(string fileName, byte[] content)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"File name cannot be null or whitespace.", nameof(fileName));

            if (content is null)
                throw new ArgumentNullException(nameof(content));

            var relativePath = GetRelativePathToFile(fileName);
            var fullName = GetFullPath(relativePath);
            if (File.Exists(fullName))
                throw new InvalidOperationException("File already exists.");

            await File.WriteAllBytesAsync(fullName, content);
            return relativePath;
        }

        // Using Task for consistency with another class methods: using only async API.
        public Task DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"File name cannot be null or whitespace.", nameof(fileName));

            var fullName = GetFullPath(fileName);
            File.Delete(fileName);
            return Task.CompletedTask;
        }

        private string GetFullPath(string relativePathToFile)
        {
            return Path.Combine(webHostEnvironment.WebRootPath, relativePathToFile);
        }

        private string GetRelativePathToFile(string fileName)
        {
            return Path.Combine(fileStorageOptions.CurrentValue.PostImageDirectory, fileName);
        }
    }
}