using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesLoader
    {
        List<Models.File> LoadFiles(string[] paths, int currentFilesCount = 0);
    }
}
