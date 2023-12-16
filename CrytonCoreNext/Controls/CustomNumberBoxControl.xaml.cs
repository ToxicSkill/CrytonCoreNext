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


        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public CustomNumberBoxControl()
        {
            InitializeComponent();
        }

        private void SubtractValue(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            --Value;
        }

        private void AddValue(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ++Value;
        }
    }
}
