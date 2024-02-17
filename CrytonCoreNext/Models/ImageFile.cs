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
    public class ImageFile : File
    {
        private EPDFFormat _selectedPdfFormat;

        private WriteableBitmap _bitmap;

        private readonly Dictionary<EPDFFormat, Size> _sizeByPdfFormat = [];

        public WriteableBitmap Bitmap { get => _bitmap; set { _bitmap = value; NotifyPropertyChanged(); } }

        public double Width { get; }

        public double Height { get; }

        public bool Color { get; }

        public Size ExportSize { get; set; }

        public string ExportSizeString { get; set; }

        public static List<EPDFFormat> AvailablePdfFormats { get; set; } = Enum.GetValues(typeof(EPDFFormat)).Cast<EPDFFormat>().ToList();

        public EPDFFormat SelectedPdfFormat
        {
            get => _selectedPdfFormat;
            set
            {
                _selectedPdfFormat = value;
                UpdateExportSize();
            }
        }

        public ImageFile(File file, Mat image) : base(file)
        {
            _selectedPdfFormat = AvailablePdfFormats.Last();
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
            _selectedPdfFormat = AvailablePdfFormats.Last();
            InitializeDictionaries();
            LoadImage();
            if (Bitmap != null)
            {
                Width = Bitmap.Width;
                Height = Bitmap.Height;
            }
        }

        private void InitializeDictionaries()
        {
            _sizeByPdfFormat.Add(EPDFFormat.A1, new(iText.Kernel.Geom.PageSize.A1.GetWidth(), iText.Kernel.Geom.PageSize.A1.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A2, new(iText.Kernel.Geom.PageSize.A2.GetWidth(), iText.Kernel.Geom.PageSize.A2.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A3, new(iText.Kernel.Geom.PageSize.A3.GetWidth(), iText.Kernel.Geom.PageSize.A3.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A4, new(iText.Kernel.Geom.PageSize.A4.GetWidth(), iText.Kernel.Geom.PageSize.A4.GetHeight()));
            _sizeByPdfFormat.Add(EPDFFormat.A5, new(iText.Kernel.Geom.PageSize.A5.GetWidth(), iText.Kernel.Geom.PageSize.A5.GetHeight()));
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
            ExportSize = desiredSize;
            ExportSizeString = $"{desiredSize.Width}x{desiredSize.Height}";
            NotifyPropertyChanged(nameof(ExportSizeString));
        }
    }
}
