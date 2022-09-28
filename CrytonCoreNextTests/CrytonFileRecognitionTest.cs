using CrytonCoreNext.Crypting;
using CrytonCoreNext.Interfaces;
using System;
using Xunit;


namespace CrytonCoreNextTests
{
    public class CrytonFileRecognitionTest
    {
        private static readonly byte[] _defaultBytes = Array.Empty<byte>();
        private static readonly int _correctLenght = 64;
        private readonly ICryptingRecognition _cryptingRecognition;

        public CrytonFileRecognitionTest()
        {
            _cryptingRecognition = new CryptingRecognition(new(Guid.NewGuid()));
        }

        [Fact]
        public void TestNamesTrueShoudNotNull()
        {
            var method = "AES";
            var extension = "pdf";
            var result = _cryptingRecognition.PrepareRerecognizableBytes(method, extension);
            Assert.NotNull(result);
        }

        [Fact]
        public void TestNamesTrueShoudCorretLenght()
        {
            var method = "AES";
            var extension = "pdf";
            var result = _cryptingRecognition.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_correctLenght, result.Length);
        }


        [Fact]
        public void TestNamesFalseShoudEmpty()
        {
            var method = string.Empty;
            var extension = string.Empty;
            var result = _cryptingRecognition.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_defaultBytes, result);
        }

        [Fact]
        public void TestNamesPartlyFalseShoudEmpty()
        {
            var method = "AES";
            var extension = string.Empty;
            var result = _cryptingRecognition.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_defaultBytes, result);
        }

        [Fact]
        public void TestNamesToLongShoudNotEmpty()
        {
            var method = "AESAESAEASAESAESAESAES";
            var extension = "txtxtxtxtxtxtxtxtx";
            var result = _cryptingRecognition.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_correctLenght, result.Length);
        }

        [Fact]
        public void TestNamesMaxLenghtShoudNotNull()
        {
            var method = "AESA";
            var extension = "txttxtttx";
            var result = _cryptingRecognition.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_correctLenght, result.Length);
        }
    }
}