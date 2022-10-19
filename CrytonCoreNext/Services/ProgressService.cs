using CrytonCoreNext.Abstract;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.Windows;

namespace CrytonCoreNext.Services
{
    public class ProgressService : NotificationBase, IProgressService
    {
        private readonly Dictionary<int, string> _progressDictionary;

        private int _progressCounter;

        public Visibility PreparingVisibility { get; set; }

        public Visibility StartingVisibility { get; set; }

        public Visibility FinishedVisibility { get; set; }

        public Visibility UpdatingVisibility { get; set; }

        public Visibility SuccessVisibility { get; set; }


        public ProgressService()
        {
            _progressDictionary = new();
            FillProgressDictionary();
        }

        public void UpdateProgress(Visibility visibility)
        {
            var propertyName = _progressDictionary[_progressCounter];
            var property = GetType().GetProperty(propertyName);
            if (property != null)
            {
                property.SetValue(this, visibility, null);
            }

            OnPropertyChanged(propertyName);
            _progressCounter++;
        }

        public void HideAllProgress()
        {
            RestartVounter();
            for (var i = 0; i < _progressDictionary.Count; i++)
            {
                UpdateProgress(Visibility.Hidden);
            }
            RestartVounter();
        }

        private void RestartVounter()
        {
            _progressCounter = 0;
        }

        private void FillProgressDictionary()
        {
            var properties = GetType().GetProperties();
            foreach (var (item, index) in properties.WithIndex())
            {
                if (item.PropertyType == typeof(Visibility))
                {
                    _progressDictionary.Add(index, item.Name);
                }
            }
        }
    }
}
