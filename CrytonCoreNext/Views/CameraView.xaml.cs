using CrytonCoreNext.ViewModels;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views
{
    public partial class CameraView : INavigableView<CameraViewModel>
    {
        public CameraViewModel ViewModel
        {
            get;
        }

        public CameraView(CameraViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            //Background.Content = new FluentWaves();
        }

        private void Animate()
        {
            var width = canvasBorder.ActualWidth;
            var height = canvasBorder.ActualHeight;
            var rnd = new Random();
            for (var i = 1; i < 4; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    var newX = rnd.Next(0, (int)width);
                    var newY = rnd.Next(0, (int)height);
                    var elipse = new Ellipse()
                    {
                        Width = 5 + 15 / i,
                        Height = 5 + 15 / i,
                        Fill = new SolidColorBrush(Colors.Red),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        Margin = new Thickness(newX, newY, 0, 0),
                        Effect = new BlurEffect()
                        {
                            Radius = i * 5
                        }
                    };
                    var sb = new Storyboard();
                    var ta = new ThicknessAnimation
                    {
                        BeginTime = new TimeSpan(0),
                        From = new Thickness(newX, newY, 0, 0),
                        To = new Thickness(newX + (50 * (rnd.Next(0, 2) == 0 ? (-1) : 1)), newY + (50 * (rnd.Next(0, 2) == 0 ? (-1) : 1)), 0, 0),
                        Duration = new Duration(TimeSpan.FromSeconds(4)),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };

                    Storyboard.SetTargetProperty(ta, new PropertyPath(MarginProperty));
                    sb.Children.Add(ta);
                    sb.Begin(elipse);
                    canvas.Children.Add(elipse);
                }
            }
        }

        private void UiPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Animate();
            //ViewModel.SetNavigationControl(CameraNavigationView);
            //Task.Run(ViewModel.OnLoaded);
        }
    }
}