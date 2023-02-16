﻿using CrytonCoreNext.Crypting.Models;
using System;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICrypting
    {
        public string Name { get; }

        public string DescriptionName { get; }

        public int ProgressCount { get; }

        Task<byte[]> Encrypt(byte[] data, IProgress<string> progress);

        Task<byte[]> Decrypt(byte[] data, IProgress<string> progress);

        ICryptingView<CryptingMethodViewModel> GetViewModel();

        string GetName();
    }
}
