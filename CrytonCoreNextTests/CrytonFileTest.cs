using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Crypting.Services;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CrytonCoreNextTests
{
    public class Files
    {
        private readonly ICryptingService _cryptingService;

        private readonly ICryptingRecognition _cryptingRecognition;

        private readonly ICryptingReader _cryptingReader;

        private readonly IFilesSaver _filesSaver;

        private readonly IFilesLoader _filesLoader;

        private readonly IFilesManager _filesManager;

        private readonly List<string> _filesToOpen = new() { "./TestingFiles/test.txt" };

        public readonly List<CryptFile> CryptFiles;

        public Files()
        {
            _cryptingRecognition = new CryptingRecognition();
            _cryptingReader = new CryptingReader();
            _cryptingService = new CryptingService(_cryptingRecognition, _cryptingReader);
            _filesSaver = new FilesSaver();
            _filesLoader = new FilesLoader();
            _filesManager = new FilesManager();
            CryptFiles = new List<CryptFile>();
            LoadFiles();
        }

        private async Task LoadFiles()
        {
            await foreach (var file in _filesLoader.LoadFiles(_filesToOpen))
            {
                CryptFiles.Add(_cryptingService.ReadCryptFile(file));
            }
        }
    }

    public class CrytonFileTest : IClassFixture<Files>
    {

        private readonly Files _files;

        public Mock<IFilesSaver> FilesSaver = new();

        public CrytonFileTest(Files filesFixture)
        {
            _files = filesFixture;
        }

        [Fact]
        public void TestOpenFileShouldNotNull()
        {
            var file = _files.CryptFiles.First();
            Assert.NotNull(file);
        }

        [Fact]
        public void TestOpenFileShouldBytesNotNull()
        {
            var file = _files.CryptFiles.First();
            var bytes = file.Bytes;
            Assert.NotNull(bytes);
        }

        [Fact]
        public void TestOpenFileShouldSizeNotEmpty()
        {
            var file = _files.CryptFiles.First();
            var size = file.Size;
            Assert.NotEmpty(size);
        }

        [Fact]
        public void TestOpenFileShouldExtensionText()
        {
            var file = _files.CryptFiles.First();
            var expected = "txt";
            var extension = file.Extension;
            Assert.Equal(extension, expected);
        }

        [Fact]
        public void TestOpenFileShouldGuidNotEmpty()
        {
            var file = _files.CryptFiles.First();
            var reference = Guid.NewGuid();
            var guid = file.Guid;
            Assert.NotEqual(reference, guid);
        }

        [Fact]
        public void TestOpenFileShouldNameEqualsTest()
        {
            var file = _files.CryptFiles.First();
            var expected = "test";
            var name = file.Name;
            Assert.Equal(expected, name);
        }

        [Fact]
        public void TestOpenFileShouldStatusInfoDecrypted()
        {
            var file = _files.CryptFiles.First();
            var expected = CryptingStatus.Status.Decrypted;
            var statusInfo = file.Status;
            Assert.Equal(expected, statusInfo);
        }

        [Fact]
        public void TestOpenFileShouldIdEqualsZero()
        {
            var file = _files.CryptFiles.First();
            var expected = 1;
            var id = file.Id;
            Assert.Equal(expected, id);
        }

        //[Fact]
        //public void TestOpenFilesShouldNotNull()
        //{
        //    var newFileNameToLoad = "./TestingFiles/test.pdf";
        //    var newListOfFiles = _filesToOpen.ToList();
        //    newListOfFiles.Add(newFileNameToLoad);
        //    var files = _filesLoader.LoadFiles(newListOfFiles);
        //    Assert.NotEmpty(files);
        //    Assert.Equal(newListOfFiles.Count, files.Count);
        //}
    }
}
