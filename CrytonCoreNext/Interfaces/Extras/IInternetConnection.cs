using System.Windows.Media;

namespace CrytonCoreNext.Interfaces.Extras;

public interface IInternetConnection
{
    bool GetInternetStatus();

    SolidColorBrush GetColorInternetStatus();
}
