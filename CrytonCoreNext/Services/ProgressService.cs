using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using System;
using System.Windows;

namespace CrytonCoreNext.Services
{
    public class ProgressService : NotificationBase, IProgressService
    {
        private const int ProgressMaxValue = 100;

        private const int DefaultStageCount = 1;

        private const int DefaultProgressCounter = 0;

        private int _progressCounter = 0;

        private int _stages = 1;

        public Visibility ProgressVisibility { get; private set; }

        public int ProgressValue { get; set; }

        public string ProgressCounter { get; private set; }

        public string ProgressMessage { get; private set; }

        public ProgressService()
        {
            ClearProgress();
        }

        public void ClearProgress()
        {
            UpdateProgress(Visibility.Hidden);
            _progressCounter = DefaultProgressCounter;
            _stages = DefaultStageCount;
            SetProgresstMessages(string.Empty);
            SetProgressCounter();
        }

        public IProgress<T> SetProgress<T>(int stages)
        {
            _stages = stages;
            _progressCounter = 0;
            UpdateProgress();
            return new Progress<T>(ReportProgress<T>);
        }

        private void UpdateProgress(Visibility visibility = Visibility.Visible)
        {
            ProgressVisibility = visibility;
            OnPropertyChanged(nameof(ProgressVisibility));
        }

        private void SetProgresstMessages(string message)
        {
            ProgressMessage = message;
            OnPropertyChanged(nameof(ProgressMessage));
        }

        private void SetProgressCounter()
        {
            _progressCounter++;
            ProgressCounter = _progressCounter.ToString() + " / " + _stages.ToString();
            SetProgressValue();
            OnPropertyChanged(nameof(ProgressCounter));
        }

        private void SetProgressValue()
        {
            ProgressValue = _progressCounter * (ProgressMaxValue / _stages);
            OnPropertyChanged(nameof(ProgressValue));
        }

        private void ReportProgress<T>(T progressMessage)
        {
            SetProgressCounter();
            SetProgresstMessages(progressMessage?.ToString() ?? string.Empty);
        }
    }
}
