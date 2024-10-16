using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Enums;
using CrytonCoreNext.PDF.Enums;
using CrytonCoreNext.Services;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;
using Size = OpenCvSharp.Size;

namespace CrytonCoreNext.Models
{
    public partial class ImageFile : File
    {
        private readonly Dictionary<EPDFFormat, Size> _sizeByPdfFormat = [];

        private Mat _image;

        private int _rotationCount = 0;

        [ObservableProperty]
        private EPDFFormat _selectedPdfFormat;

        [ObservableProperty]
        private Size _exportSize;

        [ObservableProperty]
        private WriteableBitmap _adjusterBitmap;

        [ObservableProperty]
        private string _exportSizeString;

        [ObservableProperty]
        private bool _grayscale;

        public static List<EPDFFormat> AvailablePdfFormats { get; set; } = Enum.GetValues(typeof(EPDFFormat)).Cast<EPDFFormat>().ToList();

        public ImageFile(File file, Mat image) : base(file)
        {
            SelectedPdfFormat = AvailablePdfFormats.Last();
            InitializeDictionaries();
            if (image != null)
            {
                _image = image;
            }
        }

        public ImageFile(File file) : base(file)
        {
            SelectedPdfFormat = AvailablePdfFormats.Last();
            InitializeDictionaries();
            LoadImage();
        }

        partial void OnSelectedPdfFormatChanged(EPDFFormat value)
        {
            UpdateExportSize();
        }

        partial void OnGrayscaleChanged(bool value)
        {
            using var mat = _image.Clone();
            if (value)
            {
                switch (mat.Channels())
                {
                    case 3:
                        Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);
                        break;
                    case 4:
                        Cv2.CvtColor(mat, mat, ColorConversionCodes.BGRA2GRAY);
                        break;
                    default:
                        break;
                }

            }
            _rotationCount %= 4;
            var rotates = _rotationCount < 0 ? _rotationCount + 4 : _rotationCount;
            for (var i = 0; i < rotates; i++)
            {
                Cv2.Rotate(mat, mat, RotateFlags.Rotate90Clockwise);
            }
            AdjusterBitmap = mat.ToWriteableBitmap();
        }

        private void InitializeDictionaries()
        {
            _sizeByPdfFormat.Add(EPDFFormat.A1, new(iText.Kernel.Geom.PageSize.A1.GetWidth(), iText.Kernel.Geom.PageSize.A1.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A2, new(iText.Kernel.Geom.PageSize.A2.GetWidth(), iText.Kernel.Geom.PageSize.A2.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A3, new(iText.Kernel.Geom.PageSize.A3.GetWidth(), iText.Kernel.Geom.PageSize.A3.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A4, new(iText.Kernel.Geom.PageSize.A4.GetWidth(), iText.Kernel.Geom.PageSize.A4.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A5, new(iText.Kernel.Geom.PageSize.A5.GetWidth(), iText.Kernel.Geom.PageSize.A5.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A6, new(iText.Kernel.Geom.PageSize.A6.GetWidth(), iText.Kernel.Geom.PageSize.A6.GetHeight()));
        }

        private void LoadImage()
        {
            try
            {
                if (Extension.ToLowerInvariant() == Enum.GetName(typeof(EImageExtensions), EImageExtensions.gif))
                {
                    var converter = new ImageConverterService();
                    _image = converter.ConvertGifToMat(this);
                }
                else
                {
                    _image = new Mat(Path);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            finally
            {
                AdjusterBitmap = _image.ToWriteableBitmap();
            }
        }

        private void UpdateExportSize()
        {
            if (!_sizeByPdfFormat.ContainsKey(SelectedPdfFormat))
            {
                return;
            }
            var desiredSize = _sizeByPdfFormat[SelectedPdfFormat];
            var isVertical = _image.Width < _image.Height;
            desiredSize.Deconstruct(out int width, out int height);
            if (!isVertical)
            {
                var wRatio = desiredSize.Width / (double)_image.Width;
                var ratio = _image.Width / (double)_image.Height;
                var newW = (int)((double)_image.Width * wRatio);
                desiredSize = new Size(newW, newW * (1 / ratio));
            }
            else
            {
                var hRatio = desiredSize.Height / _image.Height;
                var ratio = _image.Height / _image.Width;
                var newH = (int)((double)_image.Height * hRatio);
                desiredSize = new Size(newH * (1 / ratio), newH);
            }
            UpdateExportSize(desiredSize);
        }

        private void UpdateExportSize(Size desiredSize)
        {
            ExportSize = desiredSize;
            ExportSizeString = $"Image will be resized to {desiredSize.Width}x{desiredSize.Height}";
        }

        [RelayCommand]
        private void RotateRight()
        {
            using var mat = AdjusterBitmap.ToMat();
            Cv2.Rotate(mat, mat, RotateFlags.Rotate90Clockwise);
            AdjusterBitmap = mat.ToWriteableBitmap();
            UpdateExportSize();
            _rotationCount++;
        }

        [RelayCommand]
        private void RotateLeft()
        {
            using var mat = AdjusterBitmap.ToMat();
            Cv2.Rotate(mat, mat, RotateFlags.Rotate90Counterclockwise);
            AdjusterBitmap = mat.ToWriteableBitmap();
            UpdateExportSize();
            _rotationCount--;
        }
    }
}
