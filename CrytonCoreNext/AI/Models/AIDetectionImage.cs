using CrytonCoreNext.Extensions;
using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.AI.Models
{
    public class AIDetectionImage : SimpleImageItemContainer
    {
        private readonly AIImage _parent;

        public List<SimpleImageItemContainer> DetectionImages { get; set; }

        public AIDetectionImage(AIImage parent)
        {
            DetectionImages = new();
            _parent = parent;
            ExtractDetectionImagesFromBitmap();
        }

        private void ExtractDetectionImagesFromBitmap()
        {
            using var mat = _parent.Image.ToMat();
            foreach (var prediction in _parent.Predictions)
            {
                DetectionImages.Add(
                    new()
                    {
                        Image = new Mat(mat, prediction.Rectangle.ToRect()).ToWriteableBitmap()
                    });
            }
        }
    }
}
