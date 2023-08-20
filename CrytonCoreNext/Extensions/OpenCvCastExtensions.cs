﻿using OpenCvSharp;
using System.Drawing;

namespace CrytonCoreNext.Extensions
{
    public static class OpenCvCastExtensions
    {

        public static Rect ToRect(this RectangleF rectangle)
        {
            return new Rect((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }
    }
}
