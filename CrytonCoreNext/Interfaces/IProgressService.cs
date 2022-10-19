using System.Windows;

namespace CrytonCoreNext.Interfaces
{
    public interface IProgressService
    {
        Visibility PreparingVisibility { get; set; }

        Visibility StartingVisibility { get; set; }

        Visibility FinishedVisibility { get; set; }

        Visibility UpdatingVisibility { get; set; }

        Visibility SuccessVisibility { get; set; }

        void UpdateProgress(Visibility visibility);

        void HideAllProgress();
    }
}
