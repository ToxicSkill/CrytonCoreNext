using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Controls
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    [ObservableObject]
    public partial class CameraControl : UserControl
    {
        private Dictionary<ECameraType, Color> _colorByCameraType;

        private Dictionary<ECameraType, SymbolRegular> _symbolByCameraType;

        public static readonly DependencyProperty CameraNameProperty =
            DependencyProperty.Register("CameraName", typeof(string), typeof(CameraControl), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty CameraTypeProperty =
            DependencyProperty.Register("CameraType", typeof(ECameraType), typeof(CameraControl), new PropertyMetadata(null));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CameraControl), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty FpsProperty =
            DependencyProperty.Register("Fps", typeof(double), typeof(CameraControl), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CameraControl), new PropertyMetadata(default(bool)));

        public string CameraName
        {
            get { return (string)GetValue(CameraNameProperty); }
            set { SetValue(CameraNameProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                symbol.Filled = IsSelected;
            }
        }

        public double Fps
        {
            get { return (double)GetValue(FpsProperty); }
            set { SetValue(FpsProperty, value); }
        }

        public ECameraType CameraType
        {
            get { return (ECameraType)GetValue(CameraTypeProperty); }
            set { SetValue(CameraTypeProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public CameraControl()
        {
            InitializeComponent();
            _colorByCameraType = new()
            {
                { ECameraType.USB, new Color(){ A = 255, R = 253, G =  174,  B= 142} },
                { ECameraType.IP, new Color(){ A = 255, R = 244, G =  227,  B= 134} }
            };
            _symbolByCameraType = new()
            {
                { ECameraType.USB, SymbolRegular.TvUsb20 },
                { ECameraType.IP, SymbolRegular.ProtocolHandler20 }
            };
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            symbol.Filled = true;
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsSelected)
            {
                symbol.Filled = false;
            }
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            border.Background = new LinearGradientBrush(_colorByCameraType[CameraType], new Color() { A = 0 }, 90);
            symbol.Symbol = _symbolByCameraType[CameraType];
        }
    }
}
