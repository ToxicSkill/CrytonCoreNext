﻿using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CrytonCoreNext.Static.CryptingStatus;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingService
    {
        Task<byte[]> RunCrypting(ICryptingView<CryptingMethodViewModel> cryptingView, CryptFile file, IProgress<string> progress);

        void ModifyFile(CryptFile file, byte[] bytes, Status status, string methodName);

        void AddRecognitionBytes(CryptFile file);

        List<ICryptingView<CryptingMethodViewModel>> GetCryptingViews();

        bool IsCorrectMethod(CryptFile file, ICryptingView<CryptingMethodViewModel> cryptingView);

        CryptingStatus.Status GetOpositeStatus(CryptingStatus.Status currentStatus);

        CryptFile ReadCryptFile(File file);

        void RegisterFileChangedEvent(ref CryptingViewModel.HandleFileChanged? onFileChanged);
    }
}
