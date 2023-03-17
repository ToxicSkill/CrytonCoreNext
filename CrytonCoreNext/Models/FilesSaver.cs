using CrytonCoreNext.Interfaces.Files;
using System;

namespace CrytonCoreNext.Models
{
    public class FilesSaver : IFilesSaver
    {
        public bool SaveFile(string fileName, File file)
        {
            try
            {
                ByteArrayToFile(fileName, file.Bytes);
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private static void ByteArrayToFile(string fileName, byte[] bytes) => System.IO.File.WriteAllBytes(fileName, bytes);
    }
}
