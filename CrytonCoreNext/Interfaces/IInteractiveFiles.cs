using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.Interfaces
{
    public interface IInteractiveFiles
    {
        void UpdateFiles(IEnumerable<Models.File>? filesCollection);

        void DeleteFile();

        void AddFile();

        void AddFiles();
    }
}
