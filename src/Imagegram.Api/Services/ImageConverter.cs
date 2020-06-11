using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Imagegram.Api.Services
{
    public class ImageConverter : IImageConverter
    {
        public byte[] ConvertToFormat(byte[] bytes, ImageFormat targetImageFormat)
        {
            using (var sourceStream = new MemoryStream(bytes, 0, bytes.Length))
            {
                var image = Image.FromStream(sourceStream, useEmbeddedColorManagement: true, validateImageData: true);
                using (var targetStream = new MemoryStream())
                {
                    image.Save(targetStream, targetImageFormat);
                    return targetStream.ToArray();
                }
            }
        }
    }
}