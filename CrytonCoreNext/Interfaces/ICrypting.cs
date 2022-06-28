﻿using CrytonCoreNext.Abstract;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface ICrypting
    {
        byte[] Encrypt(byte[] data);

        byte[] Decrypt(byte[] data);

        ViewModelBase GetViewModel();

        string GetName();

        bool ParseSettingsObjects(Dictionary<string, object> objects, bool encryption);
    }
}