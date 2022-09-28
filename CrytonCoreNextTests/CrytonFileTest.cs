using CrytonCoreNext.Crypting;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using Xunit;
using Moq;

namespace CrytonCoreNextTests
{
    public class CrytonFileTest
    {
        private readonly ICryptingRecognition _cryptingRecognition;
        private readonly IFilesSaver _filesSaver;
        private readonly IFilesLoader _filesLoader;

        public CrytonFileTest()
        {
            _cryptingRecognition = new CryptingRecognition(new(Guid.NewGuid()));
            _filesSaver = new FilesSaver(_cryptingRecognition);
            _filesLoader = new FilesLoader(_cryptingRecognition);
        }
    }
}
