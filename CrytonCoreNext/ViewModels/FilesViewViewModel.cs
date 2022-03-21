using CrytonCoreNext.Abstract;
using CrytonCoreNext.Models;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.ViewModels
{
    public class FilesViewViewModel : ViewModelBase
    {
        public int SelectedItemIndex { get; set; } = -1;

        public ObservableCollection<File> FilesView { get; init; }

        public FilesViewViewModel()
        {
            FilesView = new ();
        }
    }
}
