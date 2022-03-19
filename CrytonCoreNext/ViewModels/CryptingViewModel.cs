using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using System;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        public CryptingViewModel()
        {
            PostPopup("Cześć Homiczku!",
                new TimeSpan(0, 0, 15));
        }
    }
}
