using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces.Files
{
    public interface IFilesLoader
    {
        IAsyncEnumerable<Models.File> LoadFiles(List<string> filesNames, IProgress<double> progress, int currentIndex = 0);
    }
}
