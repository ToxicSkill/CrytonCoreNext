using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace CrytonCoreNext.Models
{
    public partial class CustomColor : ObservableObject
    {
        [ObservableProperty]
        public SolidColorBrush color;

        [ObservableProperty]
        public string description;
    }
}
