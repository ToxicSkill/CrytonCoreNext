using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CrytonCoreNext.Crypting.Models
{

    public class CryptingRecognition : ICryptingRecognition
    {
        private List<ERObject> _objectsToRecognize; 

        private readonly MD5 _MD5Hash;

        public CryptingRecognition()
        {
            _MD5Hash = MD5.Create();
            _objectsToRecognize = new();
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

            if (!_objectsToRecognize.Contains(ERObject.CheckSum) ||
                recon.Keys.Length > recon.SizeOf[ERObject.Keys] ||
                !_objectsToRecognize.Any()) 
            {
                return new RecognitionResult(EStatus.Error);
            }

            if (_objectsToRecognize.Contains(ERObject.Method))
            {
                var methodString = recon.Method.ToString();
                if (methodString.Length > recon.SizeOf[ERObject.Method])
                {
                    methodString = methodString[..recon.SizeOf[ERObject.Method]];
                }
                objectsToStoreByRObject.Add(ERObject.Method, methodString);
            }

            if (_objectsToRecognize.Contains(ERObject.Extension))
            {
                if (recon.Extension.Length > recon.SizeOf[ERObject.Extension])
                {
                    recon.Extension = recon.Extension[..recon.SizeOf[ERObject.Extension]];
                }
                objectsToStoreByRObject.Add(ERObject.Extension, recon.Extension);
            }

            if (_objectsToRecognize.Contains(ERObject.Keys))
            { 
                objectsToStoreByRObject.Add(ERObject.Keys, recon.Keys);
            }


            var offset = 0;
            byte[] recognizableArray = new byte[objectsToStoreByRObject.Sum(x => recon.SizeOf[x.Key])];
            foreach (var obj in objectsToStoreByRObject)
            {
                var bytesToAdd = Encoding.ASCII.GetBytes(obj.Value);
                Buffer.BlockCopy(bytesToAdd, 0, recognizableArray, offset, bytesToAdd.Length);
                offset += recon.SizeOf[obj.Key];
            }
            var combinedString = string.Join("", objectsToStoreByRObject.Select(x => x.Value).ToArray());
            var stringBytes = Encoding.ASCII.GetBytes(combinedString);
            if (stringBytes == null || stringBytes.Length == 0)
            {
                return new RecognitionResult(EStatus.Error);
            }
            else
            {
                return new(EStatus.Success, recognizableArray);
            }
        }

        public Recognition RecognizeBytes(byte[] bytes)
        {
            var recon = new Recognition();
            var maxSize = 0;
            foreach (var size in recon.SizeOf.Values)
            {
                maxSize += size;
            }

            if (maxSize > bytes.Length)
            {
                return recon;
            }

            var recognizeByteArray = new byte[maxSize];
            Buffer.BlockCopy(bytes, 0, recognizeByteArray, 0, maxSize);
            var method = new byte[recon.SizeOf[ERObject.Method]];
            Array.Copy(recognizeByteArray, 0, method, 0, recon.SizeOf[ERObject.Method]);
            var extension = new byte[recon.SizeOf[ERObject.Extension]];
            Array.Copy(recognizeByteArray, method.Length, extension, 0, recon.SizeOf[ERObject.Extension]);
            var keys = new byte[recon.SizeOf[ERObject.Keys]];
            Array.Copy(recognizeByteArray,  method.Length + extension.Length, keys, 0, recon.SizeOf[ERObject.Keys]);
            var checkSum = new byte[recon.SizeOf[ERObject.CheckSum]];
            Array.Copy(recognizeByteArray, method.Length + extension.Length + keys.Length, checkSum, 0, recon.SizeOf[ERObject.CheckSum]);

            var checkArray = new byte[
                recon.SizeOf[ERObject.Method] +
                recon.SizeOf[ERObject.Extension] +
                recon.SizeOf[ERObject.Keys]];

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

        public byte[] PrepareRerecognizableBytes(Recognition recon)
        {
            var res = GetRecognitionBytes(recon);
            var methodString = recon.Method.ToString();
            if (methodString.Length > recon.SizeOf[ERObject.Method])
            {
                methodString = methodString[..recon.SizeOf[ERObject.Method]];
            }

            if (recon.Extension.Length > recon.SizeOf[ERObject.Extension])
            {
                recon.Extension = recon.Extension[..recon.SizeOf[ERObject.Extension]];
            }

            var offset = 0;
            byte[] recognizableArray = new byte[recon.SizeOf.Sum(x => x.Value)];
            byte[] checkSum = new byte[recon.SizeOf[ERObject.CheckSum]];

            var methodBytes = Encoding.ASCII.GetBytes(methodString);
            var extensionBytes = Encoding.ASCII.GetBytes(recon.Extension);
            var keysBytes = Encoding.ASCII.GetBytes(recon.Keys);

            offset = 0;
            var checkArray = new byte[
                recon.SizeOf[ERObject.Method] + 
                recon.SizeOf[ERObject.Extension] +
                recon.SizeOf[ERObject.Keys]];
            Buffer.BlockCopy(methodBytes, 0, checkArray, 0, methodBytes.Length);
            Buffer.BlockCopy(extensionBytes, 0, checkArray, recon.SizeOf[ERObject.Method], extensionBytes.Length);
            Buffer.BlockCopy(keysBytes, 0, checkArray, recon.SizeOf[ERObject.Method] + recon.SizeOf[ERObject.Extension], keysBytes.Length);
            var hashedArray = _MD5Hash.ComputeHash(checkArray);

            Buffer.BlockCopy(methodBytes, 0, recognizableArray, offset, methodBytes.Length);
            offset += recon.SizeOf[ERObject.Method];
            Buffer.BlockCopy(extensionBytes, 0, recognizableArray, offset, extensionBytes.Length);
            offset += recon.SizeOf[ERObject.Extension];
            Buffer.BlockCopy(keysBytes, 0, recognizableArray, offset, keysBytes.Length);
            offset += recon.SizeOf[ERObject.Keys];
            Buffer.BlockCopy(hashedArray, 0, recognizableArray, offset, hashedArray.Length);

            return recognizableArray;
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
