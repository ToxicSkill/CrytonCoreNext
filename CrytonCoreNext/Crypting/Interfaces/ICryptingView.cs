using CrytonCoreNext.Crypting.Models;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingView<out T> : INavigableView<T> where T : CryptingMethodViewModel
    {
    }
}