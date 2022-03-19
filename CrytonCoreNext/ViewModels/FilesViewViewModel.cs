using CrytonCoreNext.Abstract;

namespace CrytonCoreNext.ViewModels
{
    public class FilesViewViewModel : ViewModelBase
    {
        public string? Text { get; init; }

        public FilesViewViewModel(string text = null)
        {
            Text = text;
        }
    }
}
