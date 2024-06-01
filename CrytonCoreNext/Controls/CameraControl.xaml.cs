using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Models;
using CrytonCoreNext.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Size = OpenCvSharp.Size;

namespace CrytonCoreNext.Controls
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    [ObservableObject]
    public partial class CameraControl : UserControl
    {
        public static readonly DependencyProperty CameraNameProperty =
            DependencyProperty.Register("CameraName", typeof(string), typeof(CameraControl), new PropertyMetadata(default(string)));

        public string CameraName
        {
            get { return (string)GetValue(CameraNameProperty); }
            set { SetValue(CameraNameProperty, value); }
        }

        public static readonly DependencyProperty CameraTypeProperty =
            DependencyProperty.Register("CameraType", typeof(ECameraType), typeof(CameraControl), new PropertyMetadata(null));

        public ECameraType CameraType
        {
            get { return (ECameraType)GetValue(CameraTypeProperty); }
            set { SetValue(CameraTypeProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CameraControl), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        private BackgroudDeepEffectStruct _effectStruct;

        private Size _size;

        [ObservableProperty]
        private WriteableBitmap backgroundImage;

        public CameraControl()
        {
            InitializeComponent();
            _size = new Size(250, 400);
            _effectStruct = new BackgroudDeepEffectStruct(_size, 20);
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            //for (var i = 1; i < 4; i++)
            //{
            //    var image = new Image
            //    {
            //        Source = new BitmapImage(new Uri(@"E:\\Code\\C#\\CrytonCoreNext\\CrytonCoreNextTests\\TestingFiles\\stars.png")),
            //        Margin = new Thickness(10 * i)
            //    };
            //    _size = new(image.Source.Width, image.Source.Height);
            //    canvas.Children.Add(image);
            //}
        }

        private void UpdateBackgroundImage(Point point, Size refSize)
        {
            //var stem = 1;
            //var xSign = point.X < refSize.Width ? 1 * stem : -1 * stem;
            //var ySign = point.Y < refSize.Height ? 1 * stem : -1 * stem;

            //foreach (Image item in canvas.Children)
            //{
            //    var oldThickness = item.Margin;
            //    item.Margin = new Thickness(
            //        oldThickness.Left + xSign,
            //        oldThickness.Top + ySign,
            //        oldThickness.Right + xSign,
            //        oldThickness.Bottom + ySign);
            //}
        }

        private void canvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //UpdateBackgroundImage(e.GetPosition(sender as IInputElement), _size);
        }
    }
}
