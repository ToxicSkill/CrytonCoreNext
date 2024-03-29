﻿using CrytonCoreNext.Crypting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Models
{
    public class Crypting
    {
        private readonly List<(ICrypting method, string name)> Cryptors;

        public Crypting(List<(ICrypting method, string name)> cryptors)
        {
            Cryptors = new(cryptors);
        }

        public async Task<byte[]> Encrypt(byte[] data, string name, IProgress<string> progress)
        {
            var aed = Cryptors.Where(x => x.name == name).Select(x => x.method).FirstOrDefault();
            if (aed != null)
                return await aed.Encrypt(data, progress);
            return [];
        }

        public async Task<byte[]> Decrypt(byte[] data, string name, IProgress<string> progress)
        {
            var aed = Cryptors.Where(x => x.name == name).Select(x => x.method).FirstOrDefault();
            if (aed != null)
                return await aed.Decrypt(data, progress);
            return [];
        }
    }
}
