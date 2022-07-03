using CrytonCoreNext.Crypting;
using System;
using Xunit;


namespace CrytonCoreNextTests
{
    public class CrytonFileRecognitionTest
    {
        Guid guid;
        CryptingRecognition ct;
        private static readonly byte[] _defaultBytes = Array.Empty<byte>();
        private static readonly int _correctLenght = 64;

        public CrytonFileRecognitionTest()
        {
            guid = Guid.NewGuid();
            ct = new CryptingRecognition(new(guid));
        }

        [Fact]
        public void TestNamesTrueShoudNotNull()
        {
            var method = "AES";
            var extension = "pdf";
            var result = ct.PrepareRerecognizableBytes(method, extension);
            Assert.NotNull(result);
        }

        [Fact]
        public void TestNamesTrueShoudCorretLenght()
        {
            var method = "AES";
            var extension = "pdf";
            var result = ct.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_correctLenght, result.Length);
        }


        [Fact]
        public void TestNamesFalseShoudEmpty()
        {
            var method = string.Empty;
            var extension = string.Empty;
            var result = ct.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_defaultBytes, result);
        }

        [Fact]
        public void TestNamesPartlyFalseShoudEmpty()
        {
            var method = "AES";
            var extension = string.Empty;
            var result = ct.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_defaultBytes, result);
        }

        [Fact]
        public void TestNamesToLongShoudNotEmpty()
        {
            var method = "AESAESAEASAESAESAESAES";
            var extension = "txtxtxtxtxtxtxtxtx";
            var result = ct.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_correctLenght, result.Length);
        }

        [Fact]
        public void TestNamesMaxLenghtShoudNotNull()
        {
            var method = "AESA";
            var extension = "txttxtttx";
            var result = ct.PrepareRerecognizableBytes(method, extension);
            Assert.Equal(_correctLenght, result.Length);
        }
    }
}