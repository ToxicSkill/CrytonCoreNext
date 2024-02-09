using System.Windows;

namespace CrytonCoreNext.Interfaces
{
    interface IWindow
    {
        event RoutedEventHandler Loaded;

        void Show();
    }
}
