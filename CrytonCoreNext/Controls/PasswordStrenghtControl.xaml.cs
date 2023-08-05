using CrytonCoreNext.Enums;
using CrytonCoreNext.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CrytonCoreNext.Controls
{
    /// <summary>
    /// Interaction logic for PasswordStrenghtControl.xaml
    /// </summary>
    public partial class PasswordStrenghtControl : UserControl
    {
        public static readonly DependencyProperty StrenghtProperty =
            DependencyProperty.Register(
                "Strenght", 
                typeof(EStrength), 
                typeof(PasswordStrenghtControl), 
                new PropertyMetadata(EStrength.None, new PropertyChangedCallback(StrenghtPropertyChanged)));


        private static List<Rectangle> Rectangles = new List<Rectangle>();
        
        public EStrength Strenght
        {
            get { return (EStrength)GetValue(StrenghtProperty); }
            set { SetValue(StrenghtProperty, value); }
        }

        public PasswordStrenghtControl()
        {
            InitializeComponent();
            ClearRectangles();
            Rectangles.Add(VeryWeak);
            Rectangles.Add(Weak);
            Rectangles.Add(Reasonable);
            Rectangles.Add(Strong);
            Rectangles.Add(VeryStrong);
        }

        private void ClearRectangles()
        {
            var rectangles = strenghtGrid.FindVisualChildren<Rectangle>() ?? new List<Rectangle>();
            foreach (var rect in rectangles)
            {
                rect.Visibility = Visibility.Collapsed;
            }
        }
        private Rectangle _rectToChange;

        private static void StrenghtPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var appearAnimation = new DoubleAnimation
            {
                To = 1,
                From = 0,
                Duration = TimeSpan.FromSeconds(2),
                FillBehavior = FillBehavior.HoldEnd
            };
            var disappearAnimation = new DoubleAnimation
            {
                To = 0,
                From = 1,
                Duration = TimeSpan.FromSeconds(2),
                FillBehavior = FillBehavior.HoldEnd
            }; 
            var iterator = 0;
            var newValue = (int)(EStrength)e.NewValue;
            var oldValue = (int)(EStrength)e.OldValue;
            if (newValue == oldValue)
            {
                return;
            }
            foreach (var rect in Rectangles)
            {
                iterator++;
                if (iterator <= newValue && iterator > oldValue)
                {
                    rect.Visibility = Visibility.Visible;
                    rect.BeginAnimation(UIElement.OpacityProperty, appearAnimation);
                }
                else if (oldValue > newValue && iterator <= oldValue && iterator > newValue)
                {
                    rect.Visibility = Visibility.Visible;
                    rect.BeginAnimation(UIElement.OpacityProperty, disappearAnimation);
                }
            }
        }

    }
}
