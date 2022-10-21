using System;

namespace CrytonCoreNext.Interfaces
{
    public interface IProgressView
    {
        IProgress<T> InitializeProgress<T>(int stages);

        void ClearProgress();
    }
}
