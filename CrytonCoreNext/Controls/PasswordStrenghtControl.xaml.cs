using CrytonCoreNext.Enums;
using CrytonCoreNext.Extensions;
using System;
using System.Collections.Generic;
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
        private const int AnimationDurationSeconds = 2;

        private static readonly DoubleAnimation AppearAnimation = new ()
        {
            To = 1,
            From = 0,
            Duration = TimeSpan.FromSeconds(AnimationDurationSeconds),
            FillBehavior = FillBehavior.HoldEnd
        };

        private static readonly DoubleAnimation DisappearAnimation = new()
        {
            To = 0,
            From = 1,
            Duration = TimeSpan.FromSeconds(AnimationDurationSeconds),
            FillBehavior = FillBehavior.HoldEnd
        };

        public static readonly DependencyProperty StrenghtProperty =
            DependencyProperty.Register(
                "Strenght", 
                typeof(EStrength), 
                typeof(PasswordStrenghtControl), 
                new PropertyMetadata(EStrength.None, new PropertyChangedCallback(StrenghtPropertyChanged)));


        private static readonly List<Rectangle> Rectangles = new();
        
        public EStrength Strenght
        {
            get { return (EStrength)GetValue(StrenghtProperty); }
            set { SetValue(StrenghtProperty, value); }
        }

        public PasswordStrenghtControl()
        {
            InitializeComponent();
            InitializeRectangles();
            ClearRectangles();
        }

        private void InitializeRectangles()
        {
            Rectangles.Add(VeryWeak);
            Rectangles.Add(Weak);
            Rectangles.Add(Reasonable);
            Rectangles.Add(Strong);
            Rectangles.Add(VeryStrong);
        }

        private static void ClearRectangles()
        {
            foreach (var rect in Rectangles)
            {
                rect.Visibility = Visibility.Collapsed;
            }
        }

        private static void StrenghtPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
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
                    rect.BeginAnimation(UIElement.OpacityProperty, AppearAnimation);
                }
                else if (oldValue > newValue && iterator <= oldValue && iterator > newValue)
                {
                    rect.Visibility = Visibility.Visible;
                    rect.BeginAnimation(UIElement.OpacityProperty, DisappearAnimation);
                }
            }
        }

    }
}
