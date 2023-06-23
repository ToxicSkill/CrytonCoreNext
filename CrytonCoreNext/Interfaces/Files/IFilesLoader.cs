using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces.Files
{
    public interface IFilesLoader
    {
        IAsyncEnumerable<Models.File> LoadFiles(List<string> filesNames, int currentIndex = 0);
    }
}
