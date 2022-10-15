using CrytonCoreNext.Crypting;
using CrytonCoreNext.Interfaces;
using System;
using Xunit;
using Moq;
using CrytonCoreNext.Services;
using System.Collections.Generic;

namespace CrytonCoreNextTests
{
    public class CryptingTest
    {
        private readonly ICryptingService _cryptingService;

        private readonly ICrypting _aes = new AES(new Mock<IJsonSerializer>().Object);

        private readonly ICrypting _rsa = new RSA();

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
    }
}
