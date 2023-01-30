using CrytonCoreNext.Abstract;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Helpers
{
    public class Delegate
    {
        public delegate void NavigationDelegate(INavigableView<ViewModelBase> view);
    }
}
