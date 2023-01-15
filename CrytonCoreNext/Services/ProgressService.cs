using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Interfaces;
using System;
using System.Windows;

namespace CrytonCoreNext.Services
{
    [ObservableObject]
    public partial class ProgressService : IProgressService
    {
        private const int ProgressMaxValue = 100;

        private const int DefaultStageCount = 1;

        private const int DefaultProgressCounter = 0;

        private int _progressCounter = 0;

        private int _stages = 1;

        [ObservableProperty]
        public Visibility progressVisibility;

        [ObservableProperty]
        public int progressValue;

        [ObservableProperty]
        public string progressCounter;

        [ObservableProperty]
        public string progressMessage;

        [ObservableProperty]
        public Visibility showLabels;

        public ProgressService()
        {
            ClearProgress();
            ProgressCounter = string.Empty;
            ProgressMessage = string.Empty;
            ShowLabels = Visibility.Visible;
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

        public void SetLabelsVisibility(bool visible)
        {
            ShowLabels = visible ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateProgress(Visibility visibility = Visibility.Visible)
        {
            ProgressVisibility = visibility;
        }

        private void SetProgresstMessages(string message)
        {
            ProgressMessage = message;
        }

        private void SetProgressCounter()
        {
            _progressCounter++;
            ProgressCounter = _progressCounter.ToString() + " / " + _stages.ToString();
            SetProgressValue();
        }

        private void SetProgressValue()
        {
            ProgressValue = _progressCounter * (ProgressMaxValue / _stages);
        }

        private void ReportProgress<T>(T progressMessage)
        {
            SetProgressCounter();
            SetProgresstMessages(progressMessage?.ToString() ?? string.Empty);
        }
    }
}
