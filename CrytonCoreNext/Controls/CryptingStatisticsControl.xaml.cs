using CrytonCoreNext.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrytonCoreNext.Controls
{
    /// <summary>
    /// Interaction logic for CryptingStatisticsControl.xaml
    /// </summary>
    public partial class CryptingStatisticsControl : UserControl
    {
        public static readonly DependencyProperty SpeedProperty = 
            DependencyProperty.Register("Speed", typeof(object), typeof(CryptingStatisticsControl), new PropertyMetadata(0));

        public int Speed
        {
            get { return (int)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public static readonly DependencyProperty StrenghtProperty =
            DependencyProperty.Register("Strenght", typeof(object), typeof(CryptingStatisticsControl), new PropertyMetadata(0));

        public int Strenght
        {
            get { return (int)GetValue(StrenghtProperty); }
            set { SetValue(StrenghtProperty, value); }
        }

        public static readonly DependencyProperty ApplicationProperty =
            DependencyProperty.Register("Application", typeof(object), typeof(CryptingStatisticsControl), new PropertyMetadata(0));

        public int Application
        {
            get { return (int)GetValue(ApplicationProperty); }
            set { SetValue(ApplicationProperty, value); }
        }

        public CryptingStatisticsControl()
        {
            InitializeComponent();
        }

        private void SetVisibility()
        {
            var rectangles = statisticsGrid.FindVisualChildren<Rectangle>();
            var counter = 0;
            var stateCounter = 0;
            var states = new int[3] { Speed, Strenght, Application };
            foreach (var rect in rectangles)
            {
                if (counter - (5 * stateCounter) > states[stateCounter])
                {
                    rect.Visibility = Visibility.Hidden;
                }
                if ((counter + 1) % 5 == 0)
                {
                    stateCounter++;
                }
                counter++;
            }
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            SetVisibility();
        }
    }
}
