using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Models
{
    public class ImageFile : File
    {
        private WriteableBitmap _bitmap;

        public WriteableBitmap Bitmap { get => _bitmap; set { _bitmap = value; NotifyPropertyChanged(); } }

        public double Width { get; }

        public double Height { get; }

        public bool Color { get; }

        public ImageFile(File file) : base(file)
        {
            LoadImage();
            if (Bitmap != null)
            {
                Width = Bitmap.Width;
                Height = Bitmap.Height;
            }
        }

        private void LoadImage()
        {
            using var mat = new Mat(Path);
            Bitmap = mat.ToWriteableBitmap();
        }
    }
}
