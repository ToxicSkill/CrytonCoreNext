using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Drawers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CrytonCoreNext.AI.Services
{
    public class AIImageLoader
    {
        private const int MaxSingleDimensionSize = 1024;

        private readonly ImageDrawer _imageDrawer;

        public AIImageLoader(ImageDrawer imageDrawer)
        {
            _imageDrawer = imageDrawer;
        }

        public AIImage InitializeFile(CrytonCoreNext.Models.File file)
        {
            var width = 0;
            var height = 0;
            using (FileStream imageFile = new(file.Path, FileMode.Open, FileAccess.Read))
            {
                using Image tif = Image.FromStream(stream: imageFile,
                                                    useEmbeddedColorManagement: false,
                                                    validateImageData: false);
                width = (int)tif.PhysicalDimension.Width;
                height = (int)tif.PhysicalDimension.Height;
            }
            if (width == default || height == default)
            {
                return default;
            }
            var aiImage = new AIImage(file.Path, _imageDrawer);
            var image = Cv2.ImDecode(file.Bytes, ImreadModes.Unchanged);
            aiImage.Image = image.ToWriteableBitmap();
            aiImage.Constrains = new System.Drawing.Size((int)aiImage.Image.Width, (int)aiImage.Image.Height);
            aiImage.DetectionImage = aiImage.Image;
            aiImage.AdjusterImage = aiImage.Image;
            var constrains = new List<double>() { aiImage.Image.Width, aiImage.Image.Height };
            if (constrains.Any(x => x > MaxSingleDimensionSize))
            {
                var max = constrains.Max();
                var ratio = MaxSingleDimensionSize / max;
                var newHeight = aiImage.Image.Height;
                var newWidth = aiImage.Image.Width;
                if (aiImage.Image.Width > aiImage.Image.Height)
                {
                    newHeight *= ratio;
                    newWidth = MaxSingleDimensionSize;
                }
                else
                {
                    newWidth *= ratio;
                    newHeight = MaxSingleDimensionSize;
                }
                aiImage.ResizedImage = aiImage.Image.ToMat().EmptyClone();
                Cv2.Resize(aiImage.Image.ToMat(), aiImage.ResizedImage, new OpenCvSharp.Size(newWidth, newHeight));
            }
            else
            {
                aiImage.ResizedImage = aiImage.Image.ToMat();
            }
            return aiImage;
        }
    }
}
