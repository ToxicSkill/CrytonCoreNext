﻿using CrytonCoreNext.Crypting;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CrytonCoreNextTests
{
    public class CryptingTest
    {
        private readonly ICryptingService _cryptingService;

        private readonly ICrypting _aes = new AES(new Mock<IJsonSerializer>().Object);

        private readonly ICrypting _rsa = new RSA(new Mock<IJsonSerializer>().Object, new Mock<IXmlSerializer>().Object, new Mock<IProgressView>().Object);

        private readonly IProgress<string> _progress = new Mock<Progress<string>>().Object;

        public CryptingTest()
        {
            _cryptingService = new CryptingService(new List<ICrypting>() { _aes, _rsa });
        }

        [Fact]
        public void TestNamesOfCryptorsShoudEqualTwo()
        {
            var cryptors = _cryptingService.GetCryptors();
            var expected = 2;
            Assert.Equal(cryptors.Count, expected);
        }

        [Fact]
        public void TestCurrentCrypiingShouldBeAES()
        {
            var currentCrypting = _cryptingService.GetCurrentCrypting();
            var expected = _aes;
            Assert.Equal(currentCrypting, expected);
        }

        [Fact]
        public void TestCryptingNamesShouldBeCorrect()
        {
            var currentCryptingNames = _cryptingService.GetCryptors();
            var expected = new List<ICrypting>() { _aes, _rsa };
            Assert.Equal(currentCryptingNames, expected);
        }

        [Fact]
        public void TestCryptingAESShouldWork()
        {
            var cryptors = _cryptingService.GetCryptors();
            var stringByte = "abcd1234!@#$";
            var bytes = Encoding.ASCII.GetBytes(stringByte);
            var encryption = cryptors[0].Encrypt(bytes, _progress);
            var decryption = cryptors[0].Decrypt(encryption.Result, _progress);
            Assert.Equal(bytes, decryption.Result);
            Assert.NotEqual(bytes, encryption.Result);
        }

        [Fact]
        public void TestCryptingRSAShouldWork()
        {
            var cryptors = _cryptingService.GetCryptors();
            var stringByte = "abcd1234!@#$";
            var bytes = Encoding.ASCII.GetBytes(stringByte);
            var encryption = cryptors[1].Encrypt(bytes, _progress);
            var decryption = cryptors[1].Decrypt(encryption.Result, _progress);
            Assert.Equal(bytes, decryption.Result);
            Assert.NotEqual(bytes, encryption.Result);
        }
    }
}
