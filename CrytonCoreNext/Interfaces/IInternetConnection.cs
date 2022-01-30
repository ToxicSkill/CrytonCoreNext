using System.Windows.Media;

namespace CrytonCoreNext.Interfaces;

public interface IInternetConnection
{
    bool GetInternetStatus();

    SolidColorBrush GetColorInternetStatus();
}
