using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
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
            _cryptingRecognition = new CryptingRecognition();
        }

        [Fact]
        public void TestNamesTrueShoudNotNull()
        {
            var method = EMethod.AES;
            var extension = "pdf";
            var result = _cryptingRecognition.GetRecognitionBytes(new CrytonCoreNext.Models.Recognition()
            { 
                Method = EMethod.AES,
                Extension = "pdf"
            });
            Assert.Equal(result.Status, CrytonCoreNext.Enums.EStatus.Success);
        }

        [Fact]
        public void TestNamesTrueShoudCorretLenght()
        {
            var result = _cryptingRecognition.GetRecognitionBytes(new CrytonCoreNext.Models.Recognition()
            {
                Method = EMethod.AES,
                Extension = "pdf"
            });
            Assert.Equal(_correctLenght, result.Bytes.Length);
        }


        [Fact]
        public void TestNamesFalseShoudEmpty()
        {
            var result = _cryptingRecognition.GetRecognitionBytes(new CrytonCoreNext.Models.Recognition()
            {
                Method = EMethod.AES,
                Extension = string.Empty
            });
            Assert.Equal(_defaultBytes, result.Bytes);
        }

        [Fact]
        public void TestNamesPartlyFalseShoudEmpty()
        {
            var result = _cryptingRecognition.GetRecognitionBytes(new CrytonCoreNext.Models.Recognition()
            {
                Method = EMethod.AES,
                Extension = string.Empty
            });
            Assert.Equal(_defaultBytes, result.Bytes);
        }

        [Fact]
        public void TestNamesToLongShoudNotEmpty()
        {
            var result = _cryptingRecognition.GetRecognitionBytes(new CrytonCoreNext.Models.Recognition()
            {
                Method = EMethod.AES,
                Extension = "txtxtxtxtxtxtxtxtx"
            });
            Assert.Equal(_correctLenght, result.Bytes.Length);
        }

        [Fact]
        public void TestNamesMaxLenghtShoudNotNull()
        {
            var result = _cryptingRecognition.GetRecognitionBytes(new CrytonCoreNext.Models.Recognition()
            {
                Method = EMethod.AES,
                Extension = "txttxtttx"
            });
            Assert.Equal(_correctLenght, result.Bytes.Length);
        }
    }
}