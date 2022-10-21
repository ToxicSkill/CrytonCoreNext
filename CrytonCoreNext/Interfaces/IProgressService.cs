using System;

namespace CrytonCoreNext.Interfaces
{
    public interface IProgressService
    {
        void ClearProgress();

        IProgress<T> SetProgress<T>(int stages);
    }
}
