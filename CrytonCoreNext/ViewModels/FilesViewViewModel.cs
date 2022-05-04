using CrytonCoreNext.Abstract;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Models;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.ViewModels
{
    public class FilesViewViewModel : ViewModelBase
    {
        private bool _showFilesView = false;

        private int _selectedItemIndex = 0;

        public int SelectedItemIndex
        {
            get => _selectedItemIndex;
            set
            {
                if (_selectedItemIndex != value || value == 0)
                {
                    _selectedItemIndex = value;
                    OnPropertyChanged(nameof(SelectedItemIndex));
                }
            }
        }

        public ObservableCollection<File>? FilesView { get; init; }

        public bool ShowFilesView 
        { 
            get => _showFilesView;
            set
            {
                _showFilesView = value;
                OnPropertyChanged(nameof(ShowFilesView));
            }
        }

        public FilesViewViewModel(ObservableCollection<File>? files = null, bool showFilesView = false)
        {
            ShowFilesView = showFilesView;
            FilesView = files?.Copy();
            InitializeFiles();
        }
        
        private void InitializeFiles()
        {
            if (FilesView != null && FilesView.Count > 0)
            {
                SelectedItemIndex = 0;
            }
        }
    }
}
