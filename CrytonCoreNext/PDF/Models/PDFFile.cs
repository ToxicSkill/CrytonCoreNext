using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.PDF.Models
{
    public class PdfRangeFile : INotifyPropertyChanged
    {
        private bool _isSelectedToSplit;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int From { get; set; }

        public int To { get; set; }

        public string Name { get; set; }

        public bool IsSelectedToSplit { get => _isSelectedToSplit; set { _isSelectedToSplit = value; NotifyPropertyChanged(); } }

        public string Description => $"From {From} to {To}";

        public PdfRangeFile(int from, int to, string name)
        {
            From = from;
            To = to;
            Name = name;
            IsSelectedToSplit = true;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public struct PdfImageContainer
    {
        public EDirection SplitDirection { get; set; }

        public bool IsVerticalSplitLineLeftVisible { get; set; }

        public bool IsVerticalSplitLineRightVisible { get; set; }

        public int PageNumber { get; init; }

        public WriteableBitmap Image { get; init; }

        public PdfImageContainer(int pageNumber, WriteableBitmap writeableBitmap)
        {
            PageNumber = pageNumber;
            Image = writeableBitmap;
            IsVerticalSplitLineLeftVisible = false;
            IsVerticalSplitLineRightVisible = false;
            SplitDirection = EDirection.None;
        }
    }

    public class PDFFile : File
    {
        private byte[] _password;

        private byte[] _entropy;

        private WriteableBitmap _image;

        public WriteableBitmap PageImage { get => _image; set { _image = value; NotifyPropertyChanged(); } }

        public EFileStatus Status { get; set; }

        public EPdfStatus PdfStatus { get; set; }

        public string Password { get => Unprotect(); set => Protect(value); }

        public double Dimensions { get; set; }

        public int NumberOfPages { get; set; }

        public int LastPage { get; set; } = 0;

        public bool HasMoreThanOnePage => NumberOfPages > 0;

        public bool IsOnFirstPage => LastPage == 0;

        public bool IsOnLastPage => PdfStatus != EPdfStatus.Opened || LastPage == NumberOfPages - 1;

        public bool HasPassword { get; set; }

        public bool IsVisible { get; set; } = true;

        public string PageCountStatus
        {
            get
            {
                return $"{LastPage + 1} / {NumberOfPages}";
            }
        }

        public Dictionary<SymbolIcon, string> Metadata { get; set; } = [];

        public bool IsOpened { get; set; }

        public PDFFile(File file,
            EPdfStatus pdfStatus) : base(file)
        {
            PdfStatus = pdfStatus;
            Dimensions = 1.0;
        }

        public PDFFile(File file,
            EPdfStatus pdfStatus,
            string password,
            double dimensions,
            int numberOfPages) : base(file)
        {
            PdfStatus = pdfStatus;
            Password = password;
            Dimensions = dimensions;
            NumberOfPages = numberOfPages;
            IsOpened = PdfStatus == EPdfStatus.Opened;
        }

        private void Protect(string password)
        {
            _entropy = new byte[20];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(_entropy);
            }

            _password = ProtectedData.Protect(Encoding.UTF8.GetBytes(password), _entropy, DataProtectionScope.CurrentUser);
        }

        private string Unprotect()
        {
            if (_password == null || _entropy == null)
            {
                return string.Empty;
            }
            var bytes = ProtectedData.Unprotect(_password, _entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
