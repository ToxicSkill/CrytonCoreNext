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

namespace CrytonCoreNext.Models
{
    public partial class ImageFile : File
    {
        private readonly Dictionary<EPDFFormat, Size> _sizeByPdfFormat = [];

        [ObservableProperty]
        public WriteableBitmap bitmap;

        public double Width { get; }

        public double Height { get; }

        public bool Color { get; }

        [ObservableProperty]
        public Size exportSize;

        [ObservableProperty]
        public string exportSizeString;

        public static List<EPDFFormat> AvailablePdfFormats { get; set; } = Enum.GetValues(typeof(EPDFFormat)).Cast<EPDFFormat>().ToList();

        [ObservableProperty]
        public EPDFFormat selectedPdfFormat;

        public ImageFile(File file, Mat image) : base(file)
        {
            SelectedPdfFormat = AvailablePdfFormats.Last();
            InitializeDictionaries();
            if (image != null)
            {
                Bitmap = image.ToWriteableBitmap();
                if (Bitmap != null)
                {
                    Width = Bitmap.Width;
                    Height = Bitmap.Height;
                }
            }
        }

        public ImageFile(File file) : base(file)
        {
            SelectedPdfFormat = AvailablePdfFormats.Last();
            InitializeDictionaries();
            LoadImage();
            if (Bitmap != null)
            {
                Width = Bitmap.Width;
                Height = Bitmap.Height;
            }
        }

        partial void OnSelectedPdfFormatChanged(EPDFFormat value)
        {
            UpdateExportSize();
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
                    Bitmap = converter.ConvertGifToMat(this).ToWriteableBitmap();
                }
                else
                {
                    using var mat = new Mat(Path);
                    Bitmap = mat.ToWriteableBitmap();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Bitmap = null;
            }
        }

        private void UpdateExportSize()
        {
            if (!_sizeByPdfFormat.ContainsKey(SelectedPdfFormat))
            {
                return;
            }
            var desiredSize = _sizeByPdfFormat[SelectedPdfFormat];
            var isVertical = Bitmap.Width < Bitmap.Height;
            desiredSize.Deconstruct(out int width, out int height);
            if (!isVertical)
            {
                var wRatio = desiredSize.Width / Bitmap.Width;
                var ratio = Bitmap.Width / Bitmap.Height;
                var newW = (int)((double)Bitmap.Width * wRatio);
                desiredSize = new Size(newW, newW * (1 / ratio));
            }
            else
            {
                var hRatio = desiredSize.Height / Bitmap.Height;
                var ratio = Bitmap.Height / Bitmap.Width;
                var newH = (int)((double)Bitmap.Height * hRatio);
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
            using var mat = Bitmap.ToMat();
            Cv2.Rotate(mat, mat, RotateFlags.Rotate90Clockwise);
            Bitmap = mat.ToWriteableBitmap();
            UpdateExportSize();
        }

        [RelayCommand]
        private void RotateLeft()
        {
            using var mat = Bitmap.ToMat();
            Cv2.Rotate(mat, mat, RotateFlags.Rotate90Counterclockwise);
            Bitmap = mat.ToWriteableBitmap();
            UpdateExportSize();
        }
    }
}
