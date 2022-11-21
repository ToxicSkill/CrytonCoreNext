using System;

namespace CrytonCoreNext.Interfaces
{
    public interface IProgressView
    {
        IProgress<T> InitializeProgress<T>(int stages = 0);

        void ChangeProgressType(BusyIndicator.IndicatorType indicatorType);

        void ClearProgress(object o = null, EventArgs e = null);

        void ShowLabels(bool show);
    }
}
