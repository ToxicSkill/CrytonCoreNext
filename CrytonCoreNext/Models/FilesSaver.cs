using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CrytonCoreNext.Models
{
    public class FilesSaver : IFilesSaver
    {

        private readonly ICryptingRecognition _cryptingRecognition;

        public FilesSaver(ICryptingRecognition cryptingRecognition)
        {
            _cryptingRecognition = cryptingRecognition;
        }

        public bool SaveFile(string fileName, Models.File file)
        {
            try
            {
                if (file.Status)
                {
                    var recognitionBytes = _cryptingRecognition.PrepareRerecognizableBytes(file.Method, file.Extension);
                    var newBytes = recognitionBytes.Concat(file.Bytes);
                    if (recognitionBytes != null)
                    {
                        if (recognitionBytes.Length > 0)
                        {
                            ByteArrayToFile(fileName, newBytes.ToArray());
                            return true;
                        }
                    }
                }
                else
                {
                    ByteArrayToFile(fileName, file.Bytes);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private static void ByteArrayToFile(string fileName, byte[] bytes) => System.IO.File.WriteAllBytes(fileName, bytes);
    }
}
