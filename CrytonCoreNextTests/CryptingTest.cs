﻿using CrytonCoreNext.Crypting.Cryptors;
using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Crypting.Services;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Providers;
using Moq;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CrytonCoreNextTests
{
    public class CryptingTest
    {
        private readonly ICryptingService _cryptingService;

        private readonly ICryptingRecognition _cryptingRecognition = new Mock<ICryptingRecognition>().Object;

        private readonly ICryptingReader _cryptingReader = new Mock<ICryptingReader>().Object;

        private readonly IProgress<string> _progress = new Mock<Progress<string>>().Object;

        public CryptingTest()
        {
            _cryptingService = new CryptingService(_cryptingRecognition, _cryptingReader);
        }


        [Fact]
        public async void TestCryptingAESShouldWork()
        {
            ICrypting aes = new AES();
            var stringByte = "abcd1234!@#$";
            var bytes = Encoding.ASCII.GetBytes(stringByte);
            var cryptFile = new CryptFile(
                new CrytonCoreNext.Models.File("test", "", DateTime.Now, "", 0, bytes), 
                CrytonCoreNext.Static.CryptingStatus.Status.Decrypted, 
                EMethod.AES, 
                "",
                Guid.NewGuid());
            var enrypted = await _cryptingService.RunCrypting(aes, cryptFile, _progress);
            cryptFile.Bytes = enrypted;
            cryptFile.Status = CrytonCoreNext.Static.CryptingStatus.Status.Encrypted;
            var decrypted = await _cryptingService.RunCrypting(aes, cryptFile, _progress);
            Assert.Equal(bytes, decrypted);
            Assert.NotEqual(enrypted, decrypted);
        }

        [Fact]
        public async void TestCryptingRSAShouldWork()
        {
            ICrypting rsa = new CrytonCoreNext.Crypting.Cryptors.RSA();
            var stringByte = "abcd1234!@#$";
            var bytes = Encoding.ASCII.GetBytes(stringByte);
            var cryptFile = new CryptFile(
                new CrytonCoreNext.Models.File("test", "", DateTime.Now, "", 0, bytes),
                CrytonCoreNext.Static.CryptingStatus.Status.Decrypted,
                EMethod.RSA,
                "",
                Guid.NewGuid());
            var enrypted = await _cryptingService.RunCrypting(rsa, cryptFile, _progress);
            cryptFile.Bytes = enrypted;
            cryptFile.Status = CrytonCoreNext.Static.CryptingStatus.Status.Encrypted;
            var decrypted = await _cryptingService.RunCrypting(rsa, cryptFile, _progress);
            Assert.Equal(bytes, decrypted);
            Assert.NotEqual(enrypted, decrypted);
        }

        [Fact]
        public async Task TripleDESEncryptDecryptShouldPass()
        {
            IPasswordProvider passwordProvider = new PasswordProvider();
            ICrypting _3des = new _3DES(passwordProvider);
            IPasswordProvider helper = (IPasswordProvider)_3des.GetHelper();
            helper.SetPassword("abcd1234");
            var stringByte = "{ \"ToSerialzie\":{\"Key\":\"BEA11965A5244BAEE406448F6BF8BF8B094F5768A6361910C0F787F74C9543C4\",\"IV\":\"D83945FDE5ED652415D96A7888EE21CE\"},\"Name\":\"AES\"}";
            var bytes = Encoding.ASCII.GetBytes(stringByte);
            var enrypted = await _3des.Encrypt(bytes, _progress);
            var decrypted = await _3des.Decrypt(enrypted, _progress);
            Assert.Equal(bytes, decrypted);
        }

        [Fact]
        public async Task TripleDESEncryptDecryptShouldFail()
        {
            IPasswordProvider passwordProvider = new PasswordProvider();
            ICrypting _3des = new _3DES(passwordProvider);
            IPasswordProvider helper = (IPasswordProvider)_3des.GetHelper();
            helper.SetPassword("abcd1234");
            var stringByte = "{ \"ToSerialzie\":{\"Key\":\"BEA11965A5244BAEE406448F6BF8BF8B094F5768A6361910C0F787F74C9543C4\",\"IV\":\"D83945FDE5ED652415D96A7888EE21CE\"},\"Name\":\"AES\"}";
            var bytes = Encoding.ASCII.GetBytes(stringByte);
            var enrypted = await _3des.Encrypt(bytes, _progress);
            var decrypted = Array.Empty<byte>();
            helper.SetPassword("nonenone");
            try
            {
                decrypted = await _3des.Decrypt(enrypted, _progress);
            }
            catch (CryptographicException)
            {
            }
            Assert.NotEqual(bytes, decrypted);
        }
    }
}
