﻿namespace CrytonCoreNext.Interfaces.Files
{
    public interface IFilesSaver
    {
        bool SaveFile(string fileName, Models.File file);
    }
}
