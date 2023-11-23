using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;
using Nito.AsyncEx.Synchronous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrytonCoreNext.Crypting.Models
{
    public class CryptingRecognition : ICryptingRecognition
    {
        private readonly ICrypting _cryptingMethod;

        private readonly MD5 _MD5Hash;

        private const int MethodMaxSize = 14;

        private const int ExtensionMaxSize = 18;

        private const int KeysMaxSize = 1024;

        private const int KeysLenghtSize = 4;

        private const int CheckSumMaxSize = 16;

        private const int CheckSumKeysMaxSize = 16;

        private List<ERObject> _objectsToRecognize;

        private Dictionary<ERObject, int> _sizeOf;

        public CryptingRecognition(ICrypting crypting)
        {
            _MD5Hash = MD5.Create();
            _objectsToRecognize = [];
            _cryptingMethod = crypting;
            CreateDictionariy();
            InitializeObjectToRecognize();
        }

        public RecognitionResult GetRecognitionBytes(Recognition recon)
        {
            var objectsToStoreByRObject = new Dictionary<ERObject, byte[]>();

            if (recon.Keys.Length + 8 > _sizeOf[ERObject.Keys] || // max 3des size is n*8-(n%8)
                !_objectsToRecognize.Any()) 
            {
                return new RecognitionResult(EStatus.Error);
            }

            if (_objectsToRecognize.Contains(ERObject.Method))
            {
                var methodString = recon.Method.ToString();
                if (methodString.Length > _sizeOf[ERObject.Method])
                {
                    methodString = methodString[.._sizeOf[ERObject.Method]];
                }
                objectsToStoreByRObject.Add(ERObject.Method, Encoding.ASCII.GetBytes(methodString));
            }

            if (_objectsToRecognize.Contains(ERObject.Extension))
            {
                if (recon.Extension.Length > _sizeOf[ERObject.Extension])
                {
                    recon.Extension = recon.Extension[.._sizeOf[ERObject.Extension]];
                }
                objectsToStoreByRObject.Add(ERObject.Extension, Encoding.ASCII.GetBytes(recon.Extension));
            }

            if (_objectsToRecognize.Contains(ERObject.Keys))
            {
                var keysBytes = recon.Keys;
                var protectedKeysBytesResult = CryptCheckArray(keysBytes);
                if (protectedKeysBytesResult.Status != EStatus.Success)
                {
                    return new RecognitionResult(EStatus.Error);
                }
                var protectedKeysBytes = protectedKeysBytesResult.Bytes;
                var keysCheckArray = _MD5Hash.ComputeHash(keysBytes);
                objectsToStoreByRObject.Add(ERObject.Keys, protectedKeysBytes);
                objectsToStoreByRObject.Add(ERObject.KeysLenght, Encoding.ASCII.GetBytes(protectedKeysBytes.Length.ToString()));
                objectsToStoreByRObject.Add(ERObject.CheckSumKeys, keysCheckArray);
            }
            objectsToStoreByRObject.Add(ERObject.CheckSum, Array.Empty<byte>());

            var offset = 0;
            var recognitionArray = new byte[objectsToStoreByRObject.Sum(x => _sizeOf[x.Key])];
            var noKeysCheckArray = Array.Empty<byte>();
            foreach (var obj in objectsToStoreByRObject)
            {
                if (obj.Key == ERObject.Keys)
                {
                    noKeysCheckArray = new byte[offset];
                    Buffer.BlockCopy(recognitionArray, 0, noKeysCheckArray, 0, noKeysCheckArray.Length);
                    noKeysCheckArray = _MD5Hash.ComputeHash(noKeysCheckArray);
                    objectsToStoreByRObject[ERObject.CheckSum] = noKeysCheckArray.Take(_sizeOf[ERObject.CheckSum]).ToArray();
                }
                Buffer.BlockCopy(obj.Value, 0, recognitionArray, offset, obj.Value.Length);
                offset += _sizeOf[obj.Key];
            }
            if (recognitionArray == null || recognitionArray.Length == 0)
            {
                return new RecognitionResult(EStatus.Error);
            }
            else
            {
                var re = RecognizeBytes(recognitionArray);
                return new(EStatus.Success, recognitionArray);
            }
        }

        public Recognition RecognizeBytes(byte[] bytes)
        {
            var recon = new Recognition();
            var maxSize = 0;
            foreach (var size in _sizeOf.Values)
            {
                maxSize += size;
            }

            if (maxSize > bytes.Length)
            {
                return recon;
            }
            var recognizeByteArray = new byte[maxSize];
            var offset = 0;
            var listOfReadedBytes = new List<(ERObject obj, byte[] bytes)>();
            var noKeysCheckArray = Array.Empty<byte>();
            foreach (var obj in _objectsToRecognize)
            {
                var size = _sizeOf[obj];
                var array = new byte[size];
                Buffer.BlockCopy(bytes, offset, array, 0, size);
                listOfReadedBytes.Add(new(obj, array));
                if (obj == ERObject.Keys)
                {
                    noKeysCheckArray = new byte[offset];
                    Buffer.BlockCopy(bytes, 0, noKeysCheckArray, 0, noKeysCheckArray.Length);
                    noKeysCheckArray = _MD5Hash.ComputeHash(noKeysCheckArray);
                }
                offset += size;
            }
            if (!noKeysCheckArray.SequenceEqual(listOfReadedBytes.Single(x => x.obj == ERObject.CheckSum).bytes))
            {
                return recon;
            }
            foreach (var item in listOfReadedBytes)
            {
                if (item.bytes == null)
                {
                    return recon;
                }
            }
            var extension = GetStringFromByteArray(listOfReadedBytes.Single(x => x.obj == ERObject.Extension).bytes);
            var method = GetStringFromByteArray(listOfReadedBytes.Single(x => x.obj == ERObject.Method).bytes);
            var keysLenght = Int32.Parse(GetStringFromByteArray(listOfReadedBytes.Single(x => x.obj == ERObject.KeysLenght).bytes));
            var keys = listOfReadedBytes.Single(x => x.obj == ERObject.Keys).bytes.Take(keysLenght).ToArray();
            var keysCheckSum = listOfReadedBytes.Single(x => x.obj == ERObject.CheckSumKeys).bytes;

            var decryptedKeysBytesResult = DecryptCheckArray(keys);
            if (decryptedKeysBytesResult.Status != EStatus.Success)
            {
                return recon;
            }
            var keysCheckArray = _MD5Hash.ComputeHash(decryptedKeysBytesResult.Bytes.Take(keysLenght).ToArray());

            if (!keysCheckArray.SequenceEqual(keysCheckSum))
            {
                return recon;
            } 
            return new(EStatus.Success, StringToMethodEnum(method), extension, keys, keysLenght, keysCheckSum);
        }


        private void InitializeObjectToRecognize()
        {
            _objectsToRecognize = Enum.GetValues(typeof(ERObject)).Cast<ERObject>().ToList();
        }

        private void CreateDictionariy()
        {
            _sizeOf = new()
            {
                { ERObject.CheckSum, CheckSumMaxSize},
                { ERObject.CheckSumKeys, CheckSumKeysMaxSize },
                { ERObject.Method, MethodMaxSize },
                { ERObject.Extension, ExtensionMaxSize },
                { ERObject.Keys, KeysMaxSize },
                { ERObject.KeysLenght, KeysLenghtSize },
            };
        }

        private RecognitionResult CryptCheckArray(byte[] array)
        {
            try
            {
                var task = Task.Run(async () => await _cryptingMethod.Encrypt(array, new Progress<string>()));
                var resultArray = task.WaitAndUnwrapException();
                return new RecognitionResult(EStatus.Success, resultArray);
            }
            catch (CryptographicException)
            {
                return new RecognitionResult(EStatus.Error, Array.Empty<byte>());
            }
        }

        private RecognitionResult DecryptCheckArray(byte[] array)
        {
            try
            {
                var task = Task.Run(async () => await _cryptingMethod.Decrypt(array, new Progress<string>()));
                var resultArray = task.WaitAndUnwrapException(); 
                return new RecognitionResult(EStatus.Success, resultArray);
            }
            catch (CryptographicException)
            {
                return new RecognitionResult(EStatus.Error, Array.Empty<byte>());
            }
        }

        private string GetStringFromByteArray(byte[] array)
        {
            var str = Encoding.Default.GetString(array);
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            var indexToCut = str.IndexOf('\0');
            if (indexToCut == -1)
            {
                return string.Empty;
            }
            return str[..indexToCut];
        }

        private EMethod StringToMethodEnum(string methodString)
        {
            if (methodString.Equals(string.Empty))
            {
                return EMethod.AES;
            }
            if (Enum.TryParse(methodString, out EMethod enumMethod))
            {
                return enumMethod;
            }
            return EMethod.AES;
        }
    }
}
