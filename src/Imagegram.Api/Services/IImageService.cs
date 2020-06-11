using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Api.Services
{
    public interface IImageService
    {
        Task<ImageDescriptor> GetImageDescriptorAsync(IFormFile input);
    }
}