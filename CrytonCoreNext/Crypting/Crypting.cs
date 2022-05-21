using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CrytonCoreNext.Crypting
{
    public class Crypting
    {
        private readonly List<(ICrypting method, string name)> Cryptors;

        public Crypting(List<(ICrypting method, string name)> cryptors)
        {
            Cryptors = new (cryptors);
        }

        public byte[] Encrypt(byte[] data, string name)
        {
            var aed = Cryptors.Where(x => x.name == name).Select(x => x.method).FirstOrDefault();
            if (aed != null)
                return aed.Encrypt(data);
            return default(byte[]);
        }

        public byte[] Decrypt(byte[] data, string name)
        {
            var aed = Cryptors.Where(x => x.name == name).Select(x => x.method).FirstOrDefault();
            if (aed != null)
                return aed.Decrypt(data);
            return default(byte[]);
        }
    }
}
