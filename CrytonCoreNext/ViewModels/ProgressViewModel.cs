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


        public IProgress<T> InitializeProgress<T>(int stages = 0)
        {
            return ProgressService.SetProgress<T>(stages);
        }

        public void ClearProgress(object o = null, EventArgs e = null)
        {
            ProgressService.ClearProgress();
        }

        public void ShowLabels(bool show)
        {
            ProgressService.SetLabelsVisibility(show);
        }
    }
}
