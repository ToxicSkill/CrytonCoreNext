using System.Windows;
using System.Windows.Controls;

namespace CrytonCoreNext.Controls
{
    /// <summary>
    /// Interaction logic for CustomNumberBoxControl.xaml
    /// </summary> 
    public partial class CustomNumberBoxControl : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(CustomNumberBoxControl), new PropertyMetadata(0));

        public static readonly DependencyProperty MinProperty =
           DependencyProperty.Register("Min", typeof(int), typeof(CustomNumberBoxControl), new PropertyMetadata(int.MinValue));

        public static readonly DependencyProperty MaxProperty =
                   DependencyProperty.Register("Max", typeof(int), typeof(CustomNumberBoxControl), new PropertyMetadata(int.MaxValue));


        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public CustomNumberBoxControl()
        {
            InitializeComponent();
        }

        private void SubtractValue(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Value - 1 >= Min)
            {
                --Value;
            }
        }

        private void AddValue(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Value + 1 <= Max)
            {
                ++Value;
            }
        }



        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SetCaret();
        }

        private void TextBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetCaret();
        }

        private void SetCaret()
        {
            textBox.CaretIndex = textBox.Text.Length;
            textBox.ScrollToEnd();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox.Text, out int value))
            {
                if (value < Min || value > Max)
                {
                    rect.Visibility = Visibility.Visible;
                }
                else
                {
                    rect.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                rect.Visibility = Visibility.Hidden;
            }
        }
    }
}
