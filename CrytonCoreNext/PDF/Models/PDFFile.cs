using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Enums;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using System;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFFile : File
    {
        private byte[] _password;

        private byte[] _entropy;

        public PdfVersion Version { get; set; }

        public IDocReader Reader { get; set; }

        public EFileStatus Status { get; set; }

        public EPdfStatus PdfStatus { get; set; }

        public string Password { get => Unprotect(); set => Protect(value); }

        public double Dimensions { get; set; }

        public int NumberOfPages { get; set; }

        public int LastPage { get; set; } = 0;

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
            var bytes = ProtectedData.Unprotect(_password, _entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
