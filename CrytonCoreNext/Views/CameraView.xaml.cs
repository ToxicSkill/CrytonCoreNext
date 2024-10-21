using CrytonCoreNext.ViewModels;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
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
            var particles = 20;
            var plans = 4;
            for (var i = plans; i != 0; i--)
            {
                for (var j = 0; j < particles; j++)
                {
                    var newX = rnd.Next(0, (int)width);
                    var newY = rnd.Next(0, (int)height);
                    var elipse = new Ellipse()
                    {
                        Opacity = 0,
                        Width = 5 + i,
                        Height = 5 + i,
                        Fill = new SolidColorBrush(ApplicationAccentColorManager.PrimaryAccent),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(newX, newY, 0, 0),
                        Effect = new BlurEffect()
                        {
                            Radius = 1 + 2 * i,
                            RenderingBias = RenderingBias.Quality,
                            KernelType = KernelType.Gaussian
                        },
                    };
                    var sb = new Storyboard();
                    var delay = rnd.Next(0, particles);
                    var ta = new ThicknessAnimation
                    {
                        BeginTime = TimeSpan.FromSeconds(delay),
                        From = new Thickness(newX, newY, 0, 0),
                        To = new Thickness(newX + (50 * (rnd.Next(0, 2) == 0 ? (-1) : 1)), newY + (50 * (rnd.Next(0, 2) == 0 ? (-1) : 1)), 0, 0),
                        Duration = new Duration(TimeSpan.FromSeconds(20)),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever,
                        AccelerationRatio = 0.5,
                        DecelerationRatio = 0.5
                    };
                    elipse.BeginAnimation(OpacityProperty,
                        new DoubleAnimation()
                        {
                            From = 0,
                            To = 1,
                            BeginTime = TimeSpan.FromSeconds(delay),
                            Duration = TimeSpan.FromSeconds(10)
                        });
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