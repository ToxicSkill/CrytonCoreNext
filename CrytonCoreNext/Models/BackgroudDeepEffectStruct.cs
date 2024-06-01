using OpenCvSharp;
using System;

namespace CrytonCoreNext.Models
{
    public struct BackgroudDeepEffectStruct
    {
        private Size _size;

        private double[] _enforceByPlane;

        public Point[] Points { get; set; }

        public Point[] AffinePoints { get; set; }

        public int[] Planes { get; set; }

        public BackgroudDeepEffectStruct(Size size, int pointCount, int planes = 3)
        {
            _size = size;
            var rnd = new Random();
            Points = new Point[pointCount];
            Planes = new int[pointCount];
            AffinePoints = new Point[pointCount];
            _enforceByPlane = new double[planes];
            for (var i = 0.5; i < planes; i += 0.5)
            {
                _enforceByPlane[(int)(i + 0.5) - 1] = i;
            }
            for (var i = 0; i < pointCount; i++)
            {
                Points[i] = new Point(rnd.Next(0, size.Width), rnd.Next(0, size.Height));
                Planes[i] = rnd.Next(0, planes);
            }
            Array.Copy(Points, 0, AffinePoints, 0, Points.Length);
        }

        public void UpdatePoints(System.Windows.Point originTransform)
        {
            var refSize = new Size(_size.Width / 2, _size.Height / 2);

            for (var i = 0; i < Points.Length; i++)
            {
                var relativityX = Math.Abs(originTransform.X - refSize.Width) * _enforceByPlane[Planes[i]];
                var relativityY = Math.Abs(originTransform.Y - refSize.Height) * _enforceByPlane[Planes[i]];
                if (originTransform.X > refSize.Width)
                {
                    relativityX *= (-1);
                }
                if (originTransform.Y > refSize.Height)
                {
                    relativityX *= (-1);
                }
                AffinePoints[i] = new Point(Points[i].X + relativityX, Points[i].Y + relativityY);
            }
        }
    }
}
