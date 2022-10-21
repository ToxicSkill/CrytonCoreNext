using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using System;

namespace CrytonCoreNext.ViewModels
{
    public class ProgressViewModel : ViewModelBase, IProgressView
    {
        public IProgressService ProgressService { get; init; }

        public ProgressViewModel(IProgressService progressService)
        {
            ProgressService = progressService;
        }

        public IProgress<T> InitializeProgress<T>(int stages)
        {
            return ProgressService.SetProgress<T>(stages);
        }

        public void ClearProgress()
        {
            ProgressService.ClearProgress();
        }
    }
}
