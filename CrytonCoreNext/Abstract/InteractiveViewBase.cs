using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Threading;

namespace CrytonCoreNext.Abstract
{
    public class InteractiveViewBase : ViewModelBase, IDisposable
    {
        private DispatcherTimer? _timer;

        protected List<string> SubsribeProperties;

        public InformationPopupViewModel PopupViewModel { get; set; }

        public FilesViewViewModel FilesViewViewModel { get; set; }

        public string FileSize { get; set; }

        public InteractiveViewBase(List<string> subsribeProperties)
        {
            PopupViewModel = new ();
            FilesViewViewModel = new ();

            NotifyObjectChangeByName(new List<string>()
            { 
                nameof(PopupViewModel),
                nameof(FilesViewViewModel)
            }.ToArray());

            SubsribeProperties = subsribeProperties;
        }

        public void PostPopup(string informationString, int seconds, Color color = default)
        {
            PopupViewModel = new (informationString, color);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
            InitializeTimerWithAction(CollapsePopup, seconds);
        }

        public void UpdateFilesView(ObservableCollection<Models.File>? files = null)
        {
            FilesViewViewModel = new (files);
            FilesViewViewModel.PropertyChanged += SelectedItem_PropertyChanged;
            OnPropertyChanged(nameof(FilesViewViewModel));
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
<<<<<<< Updated upstream
            foreach (var property in SubsribeProperties)
            {
                switch (property)
                {
                    case nameof(FileSize):
                        FileSize = FilesViewViewModel.FilesView[FilesViewViewModel.SelectedItemIndex].Size;
                        break;
                    default:
                        break;
                }
                OnPropertyChanged(property);
            }

            //var propertyReference = SubsribeProperties.Where(x => x.dependecy.name == e.PropertyName).Select(v => v.reference).FirstOrDefault();
            //var propertyDependece = SubsribeProperties.Where(x => x.dependecy.name == e.PropertyName).Select(v => v.dependecy).FirstOrDefault();
            //if (SubsribeProperties.Any(x => x.dependecy.name == e.PropertyName))
            //{
            //    this.GetType().GetProperty(propertyReference.name).SetValue(this, FilesViewViewModel.FilesView[(int)this.GetType().GetProperty(propertyDependece.property).PropertyType.GetProperty(propertyDependece.name).GetValue(FilesViewViewModel, null)].Size);
            //    OnPropertyChanged(propertyReference.name);
            //}
=======
            var tempSelectedItemIndex = FilesViewViewModel.SelectedItemIndex;
            if (tempSelectedItemIndex != -1)
            {
                FileSize = FilesViewViewModel.FilesView.ElementAt(tempSelectedItemIndex).Size;
            }
            OnPropertyChanged(nameof(FileSize));
>>>>>>> Stashed changes
        }

        public void ShowFilesView(bool show)
        {
            FilesViewViewModel.ShowFilesView = show;
        }

        private void ShowInformationBar(bool show)
        {
            PopupViewModel.ShowPopup = show;
        }

        private void InitializeTimerWithAction(Action<object, EventArgs> obj, int seconds)
        {
            _timer = new();
            _timer.Tick += new EventHandler(obj);
            _timer.Interval = new TimeSpan(0, 0, seconds);
            _timer.Start();
        }

        private void CollapsePopup(object sender, EventArgs e)
        {
            ShowInformationBar(false);
            _timer?.Stop();
        }

        private void NotifyObjectChangeByName(string[] objects)
        {
            foreach (var obj in objects)
            {
                OnPropertyChanged(obj);
            }
        }

        public virtual void Dispose() { }
    }
}