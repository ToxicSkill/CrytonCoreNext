using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Enums;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {

        public ICommand PostFilesCommand { get; set; }

        public CryptingViewModel()
        {
            PostFilesCommand = new Command(UpdateFiles, true);
            //PostPopup("Hello world!", 5, EPopopColor.Information);
        }

        private void UpdateFiles()
        {
            UpdateFilesView(new() { new() { Name = "Haa432432423", Id = 0 }, new() { Name = "Ha2a", Id = 1 } });
        }
    }
}
