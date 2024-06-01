using System.Windows.Media;

namespace CrytonCoreNext.Models
{
    public class CameraDetection(string label, string score)
    {
        public string Label { get; set; } = label;

        public string Score { get; set; } = score;

        public SolidColorBrush Color { get; set; } = StringToColor(label);

        private static SolidColorBrush StringToColor(string input)
        {
            var hash = input.GetHashCode();
            var red = (hash & 0xFF0000) >> 16;
            var green = (hash & 0x00FF00) >> 8;
            var blue = hash & 0x0000FF;

            return new(System.Windows.Media.Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }
    }
}
