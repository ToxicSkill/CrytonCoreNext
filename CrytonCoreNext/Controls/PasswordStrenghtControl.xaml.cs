using CrytonCoreNext.Enums;
using CrytonCoreNext.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CrytonCoreNext.Controls
{
    /// <summary>
    /// Interaction logic for PasswordStrenghtControl.xaml
    /// </summary>
    public partial class PasswordStrenghtControl : UserControl
    {
        public static readonly DependencyProperty StrenghtProperty =
            DependencyProperty.Register("Strenght", typeof(EStrength), typeof(PasswordStrenghtControl), new PropertyMetadata(EStrength.VeryWeak, new PropertyChangedCallback(StrenghtPropertyChanged)));


        public EStrength Strenght
        {
            get { return (EStrength)GetValue(StrenghtProperty); }
            set {  SetValue(StrenghtProperty, value); }
        }

        public PasswordStrenghtControl()
        {
            InitializeComponent();
        }

        private static void StrenghtPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rectangles = d.FindVisualChildren<Rectangle>();
            var iterator = 0;
            var value = (int)(EStrength)e.NewValue;
            foreach (var rect in rectangles)
            {
                rect.Visibility = iterator > value ? Visibility.Collapsed : Visibility.Visible;
                iterator++;
            }
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateRectsVisibility();
        }

        private void UpdateRectsVisibility()
        {
            var rectangles = strenghtGrid.FindVisualChildren<Rectangle>();
            var iterator = 0;
            foreach (var rect in rectangles)
            {
                rect.Visibility = iterator > (int)Strenght ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
