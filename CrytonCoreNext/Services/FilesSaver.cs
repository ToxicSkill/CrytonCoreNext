﻿using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using System;
using System.Linq;

namespace CrytonCoreNext.Services
{
    public class FilesSaver : IFilesSaver
    {
        private readonly ICryptingRecognition _cryptingRecognition;

        public FilesSaver(ICryptingRecognition cryptingRecognition)
        {
            _cryptingRecognition = cryptingRecognition;
        }

        public bool SaveFile(EDialogFilters.DialogFilters filter, string title, Models.File file)
        {
            WindowDialog.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = EDialogFilters.ExtensionToFilter(filter),
                Multiselect = false,
                Title = title
            });
            var chosenPath = saveDialog.RunDialog();
            if (chosenPath.Count == 1)
            {
                try
                {
                    if (file.Status == true)
                    {
                        var recognitionBytes = _cryptingRecognition.PrepareRerecognizableBytes(file.Method, file.Extension);
                        var newBytes = recognitionBytes.Concat(file.Bytes);
                        if (recognitionBytes != null)
                        {
                            if (recognitionBytes.Length > 0)
                            {
                                ByteArrayToFile(chosenPath.First(), newBytes.ToArray());
                                return true;
                            }
                        }
                    }
                    else
                    {
                        ByteArrayToFile(chosenPath.First(), file.Bytes);
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
                return true;
            }
            return false;
        }

        private void ByteArrayToFile(string fileName, byte[] bytes) => System.IO.File.WriteAllBytes(fileName, bytes);
    }
}
