using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CrytonCoreNext.Services
{
    public class ImageConverterService
    {
        public byte[] ConvertImageToJpeg(ImageFile file)
        {
            using var s = new MemoryStream(file.Bitmap.ToMat().ToBytes());
            s.ReadByte();
            using (var pngImage = Image.FromStream(s))
            {
                using (var jpegImage = new Bitmap(pngImage.Width, pngImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
                {
                    using (var graphics = Graphics.FromImage(jpegImage))
                    {
                        graphics.DrawImage(pngImage, 0, 0, pngImage.Width, pngImage.Height);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        jpegImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        public Mat ConvertGifToMat(ImageFile file)
        {
            using var s = new MemoryStream(file.Bytes);
            s.ReadByte();
            var gif = Image.FromStream(s);
            var dim = new FrameDimension(gif.FrameDimensionsList[0]);
            var frames = gif.GetFrameCount(dim);

            var resultingImage = new Bitmap(gif.Width * frames, gif.Height);

            for (int i = 0; i < frames; i++)
            {
                gif.SelectActiveFrame(dim, i);

                Rectangle destRegion = new Rectangle(gif.Width * i, 0, gif.Width, gif.Height);
                Rectangle srcRegion = new Rectangle(0, 0, gif.Width, gif.Height);

                using (Graphics grD = Graphics.FromImage(resultingImage))
                {
                    grD.DrawImage(gif, destRegion, srcRegion, GraphicsUnit.Pixel);
                }
            }

            resultingImage.Save(s, ImageFormat.Jpeg);
            return BitmapSourceConverter.ToMat(resultingImage.ToBitmapSource());
        }
    }
}
