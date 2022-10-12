using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Static;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CrytonCoreNext.ViewModels
{
    public class InformationPopupViewModel : ViewModelBase
    {
        private bool _showPopup = false;

        public string InformationString { get; init; }

        private Color _defaultColor = ColorStatus.Information;

        public Color BackgroundColor { get; set; }

        public ICommand CollapsePopupCommand { get; set; }

        public ScrollBarVisibility VerticalScrollBarVisbility { get; set; }

        public bool ShowPopup 
        {
            get => _showPopup;
            set
            { 
                _showPopup = value;
                OnPropertyChanged(nameof(ShowPopup)); 
            } 
        }

        public InformationPopupViewModel(string informationString = "", Color color = default)
        {
            if (Color.Equals(color, default))
            {
                color = _defaultColor;
            }

            BackgroundColor = color;
            InformationString = informationString;
            VerticalScrollBarVisbility = ScrollBarVisibility.Hidden;
            CollapsePopupCommand = new Command(CollapsePopup, true);
        }

        private void CollapsePopup()
        {
            ShowPopup = false;
        }
    }
}