using OpenCvSharp;
using System.Drawing;

namespace CrytonCoreNext.Extensions
{
    public static class OpenCvCastExtensions
    {

        public static Rect ToRect(this RectangleF rectangle)
        {
            return new Rect((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public static Rect ToRect(this RectangleF rectangle, int scale)
        {
            return new Rect((int)rectangle.X / scale, (int)rectangle.Y / scale, (int)rectangle.Width / scale, (int)rectangle.Height / scale);
        }

        public static Scalar ToScalar(this Color color)
        {
            return new Scalar(color.B, color.G, color.R);
        }
    }
}
