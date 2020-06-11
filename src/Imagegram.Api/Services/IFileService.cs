using System.Threading.Tasks;

namespace Imagegram.Api.Services
{
    public interface IFileService
    {
        Task DeleteFileAsync(string fileName);
        Task<string> SaveFileAsync(string fileName, byte[] content);
    }
}