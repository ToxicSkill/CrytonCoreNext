using CrytonCoreNext.Abstract;
using CrytonCoreNext.Enums;
using System;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        public CryptingViewModel()
        {
            PostPopup("Hello world!", new TimeSpan(0, 0, 15), EPopopColor.Information);
            UpdateFilesView();
        }
    }
}
