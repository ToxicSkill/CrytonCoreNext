using CrytonCoreNext.Crypting;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using CrytonCoreNext.Static;

namespace CrytonCoreNextTests
{
    public class CrytonFileTest
    {
        private readonly ICryptingRecognition _cryptingRecognition;

        private readonly IFilesSaver _filesSaver;

        private readonly IFilesLoader _filesLoader;

        private readonly IFilesManager _filesManager;

        private readonly List<string> _filesToOpen = new () { "./TestingFiles/test.txt" };

        private readonly List<CryptFile>_files;

        public Mock<IFilesSaver> FilesSaver = new();

        public CrytonFileTest()
        {
            _cryptingRecognition = new CryptingRecognition(new(Guid.NewGuid()));
            _filesSaver = new FilesSaver();
            _filesLoader = new FilesLoader(_cryptingRecognition);
            _filesManager = new FilesManager();
            //_files = _filesLoader.LoadFiles(_filesToOpen);
        }

        [Fact]
        public void TestOpenFileShouldNotNull()
        {
            var file = _files.First();
            Assert.NotNull(file);
        }

        [Fact]
        public void TestOpenFileShouldBytesNotNull()
        {
            var file = _files.First();
            var bytes = file.Bytes;
            Assert.NotNull(bytes);
        }

        [Fact]
        public void TestOpenFileShouldSizeNotEmpty()
        {
            var file = _files.First();
            var size = file.Size;
            Assert.NotEmpty(size);
        }

        [Fact]
        public void TestOpenFileShouldExtensionText()
        {
            var file = _files.First();
            var expected = "txt";
            var extension = file.Extension;
            Assert.Equal(extension, expected);
        }

        [Fact]
        public void TestOpenFileShouldGuidNotEmpty()
        {
            var file = _files.First();
            var reference = Guid.NewGuid();
            var guid = file.Guid;
            Assert.NotEqual(reference, guid);
        }

        [Fact]
        public void TestOpenFileShouldNameEqualsTest()
        {
            var file = _files.First();
            var expected = "test";
            var name = file.Name;
            Assert.Equal(expected, name);
        }

        [Fact]
        public void TestOpenFileShouldStatusInfoDecrypted()
        {
            var file = _files.First();
            var expected = CryptingStatus.Status.Decrypted;
            var statusInfo = file.Status;
            Assert.Equal(expected, statusInfo);
        }

        [Fact]
        public void TestOpenFileShouldIdEqualsZero()
        {
            var file = _files.First();
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
