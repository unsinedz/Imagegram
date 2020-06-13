using System.Drawing.Imaging;

namespace Imagegram.Api.Services
{
    public interface IImageConverter
    {
        byte[] ConvertToFormat(byte[] bytes, ImageFormat targetImageFormat);
    }
}