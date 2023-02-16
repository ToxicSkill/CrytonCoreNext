using CrytonCoreNext.Abstract;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingView<out T> : INavigableView<T> where T : ViewModelBase
    {
    }
}
