using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Enums;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFFile : File
    {
        private byte[] _password;

        private byte[] _entropy;

        private WriteableBitmap _image;

        public WriteableBitmap PageImage { get => _image; set { _image = value; NotifyPropertyChanged(); } }

        public PdfVersion Version { get; set; }

        public IDocReader Reader { get; set; }

        public EFileStatus Status { get; set; }

        public EPdfStatus PdfStatus { get; set; }

        public string Password { get => Unprotect(); set => Protect(value); }

        public double Dimensions { get; set; }

        public int NumberOfPages { get; set; }

        public int LastPage { get; set; } = 0;

        public bool HasMoreThanOnePage => NumberOfPages > 0;

        public bool IsOnFirstPage => LastPage == 0;

        public bool IsOnLastPage => PdfStatus != EPdfStatus.Opened ? true : LastPage == NumberOfPages - 1;

        public string PageCountStatus 
        { 
            get
            {
                return $"{LastPage + 1} / {NumberOfPages}";
            } 
        }

        public Dictionary<SymbolIcon, string> Metadata { get; set; }

        public bool IsOpened { get; set; }

        public PDFFile(File file,
            EPdfStatus pdfStatus) : base(file)
        {
            PdfStatus = pdfStatus;
        }

        public PDFFile(File file,
            PdfVersion version,
            IDocReader reader,
            EPdfStatus pdfStatus,
            string password,
            double dimensions,
            int numberOfPages) : base(file)
        {
            Version = version;
            PdfStatus = pdfStatus;
            Reader = reader;
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
