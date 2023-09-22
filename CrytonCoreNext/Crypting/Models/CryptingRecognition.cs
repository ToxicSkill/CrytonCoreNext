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

namespace CrytonCoreNext.Crypting.Models
{

    public class CryptingRecognition : ICryptingRecognition
    {
        private readonly List<ERObject> _objectsToRecognize;

        private readonly ICrypting _cryptingMethod;

        private readonly MD5 _MD5Hash;

        private const int MethodMaxSize = 14;

        private const int ExtensionMaxSize = 18;

        private const int KeysMaxSize = 1064;

        private const int CheckSumMaxSize = 16;

        private Dictionary<ERObject, int> _sizeOf;

        public CryptingRecognition(ICrypting crypting)
        {
            _MD5Hash = MD5.Create();
            _objectsToRecognize = new();
            _cryptingMethod = crypting;
            CreateDictionariy();
        }

        private void CreateDictionariy()
        {
            _sizeOf = new()
            {
                { ERObject.CheckSum, CheckSumMaxSize},
                { ERObject.Method, MethodMaxSize },
                { ERObject.Extension, ExtensionMaxSize },
                { ERObject.Keys, KeysMaxSize }
            };
        }

        public void AddObject(ERObject obj)
        {
            _objectsToRecognize.Add(obj);
        }

        public void RemoveObject(ERObject obj)
        {
            if (_objectsToRecognize.Contains(obj))
            {
                _objectsToRecognize.Remove(obj);
            }
        }

        public RecognitionResult GetRecognitionBytes(Recognition recon)
        {
            var objectsToStoreByRObject = new Dictionary<ERObject, string>();

            if (recon.Keys.Length + 8> _sizeOf[ERObject.Keys] || // max 3des size is n*8-(n%8)
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
                objectsToStoreByRObject.Add(ERObject.Method, methodString);
            }

            if (_objectsToRecognize.Contains(ERObject.Extension))
            {
                if (recon.Extension.Length > _sizeOf[ERObject.Extension])
                {
                    recon.Extension = recon.Extension[.._sizeOf[ERObject.Extension]];
                }
                objectsToStoreByRObject.Add(ERObject.Extension, recon.Extension);
            }

            if (_objectsToRecognize.Contains(ERObject.Keys))
            { 
                objectsToStoreByRObject.Add(ERObject.Keys, recon.Keys);
            }           

            var offset = 0;
            var checkArray = new byte[objectsToStoreByRObject.Sum(x => _sizeOf[x.Key])];
            var protectedKeysBytes = Array.Empty<byte>();
            foreach (var obj in objectsToStoreByRObject)
            {
                var bytesToAdd = Encoding.ASCII.GetBytes(obj.Value);
                if (obj.Key == ERObject.Keys)
                {
                    protectedKeysBytes = CryptCheckArray(bytesToAdd).Bytes;
                }
                Buffer.BlockCopy(bytesToAdd, 0, checkArray, offset, bytesToAdd.Length);
                offset += _sizeOf[obj.Key];
            }
            var combinedString = string.Join("", objectsToStoreByRObject.Select(x => x.Value).ToArray());
            var checkArrayBytes = Encoding.ASCII.GetBytes(combinedString);

            var hashedArray = _MD5Hash.ComputeHash(checkArray);
            offset = 0;
            foreach (var obj in objectsToStoreByRObject)
            {
                if (obj.Key == ERObject.Keys)
                {
                    break;
                }
                offset += _sizeOf[obj.Key];
            }
            Buffer.BlockCopy(new byte[_sizeOf[ERObject.Keys]], 0, checkArray, offset, _sizeOf[ERObject.Keys]);
            Buffer.BlockCopy(protectedKeysBytes, 0, checkArray, offset, protectedKeysBytes.Length);
            var recognizableBytes = new byte[checkArray.Length + hashedArray.Length];

            Buffer.BlockCopy(checkArray, 0, recognizableBytes, 0, checkArray.Length);
            Buffer.BlockCopy(hashedArray, 0, recognizableBytes, checkArray.Length, hashedArray.Length);

            if (checkArrayBytes == null || checkArrayBytes.Length == 0)
            {
                return new RecognitionResult(EStatus.Error);
            }
            else
            {
                var re = RecognizeBytes(recognizableBytes);
                return new(EStatus.Success, recognizableBytes);
            }
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
            Buffer.BlockCopy(bytes, 0, recognizeByteArray, 0, maxSize);
            var method = new byte[_sizeOf[ERObject.Method]];
            Array.Copy(recognizeByteArray, 0, method, 0, _sizeOf[ERObject.Method]);
            var extension = new byte[_sizeOf[ERObject.Extension]];
            Array.Copy(recognizeByteArray, method.Length, extension, 0, _sizeOf[ERObject.Extension]);
            var keys = new byte[_sizeOf[ERObject.Keys]];
            Array.Copy(recognizeByteArray,  method.Length + extension.Length, keys, 0, _sizeOf[ERObject.Keys]);
            var checkSum = new byte[_sizeOf[ERObject.CheckSum]];
            Array.Copy(recognizeByteArray, method.Length + extension.Length + keys.Length, checkSum, 0, _sizeOf[ERObject.CheckSum]);

            if (method == null || extension == null || keys == null || checkSum == null)
            {
                return recon;
            }

            var offset = 0;
            for (var i = keys.Length - 1; i >=0; i--)
            {
                if (keys[i] != 0)
                {
                    offset = i + 1;
                    break;
                }
            }
            var protectedKeys = new byte[offset];
            Array.Copy(keys, 0, protectedKeys, 0, offset);
            try
            {
                var decryptedBytes = DecryptCheckArray(protectedKeys).Bytes;
            }
            catch (CryptographicException)
            {
            }

            var checkArray = new byte[
                _sizeOf[ERObject.Method] +
                _sizeOf[ERObject.Extension] +
                _sizeOf[ERObject.Keys]];

            Buffer.BlockCopy(method, 0, checkArray, 0, method.Length);
            Buffer.BlockCopy(extension, 0, checkArray, method.Length, extension.Length);
            Buffer.BlockCopy(keys, 0, checkArray, method.Length + extension.Length, keys.Length);
            var hashedArray = _MD5Hash.ComputeHash(checkArray);

            if (hashedArray.SequenceEqual(checkSum))
            {
                return new(EStatus.Success, StringToMethodEnum(GetStringFromByteArray(method)), GetStringFromByteArray(extension), GetStringFromByteArray(keys));
            }

            return recon;
        }

        private string GetStringFromByteArray(byte[] array)
        {
            var str = Encoding.Default.GetString(array);
            var indexToCut = str.IndexOf('\0');
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
