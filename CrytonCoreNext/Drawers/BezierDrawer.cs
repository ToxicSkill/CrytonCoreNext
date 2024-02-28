using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CrytonCoreNext.Drawers
{
    public static class BezierDrawer
    {
        private static System.Windows.Point[] ControlPoints(System.Windows.Point[] points)
        {
            var t = 1.0 / 5;
            var pc = new List<System.Windows.Point>();
            for (var i = 1; i < points.Length - 1; i++)
            {
                var dx = points[i - 1].X - points[i + 1].X;
                var dy = points[i - 1].Y - points[i + 1].Y;
                var point = new System.Windows.Point(points[i].X - dx * t, points[i].Y - dy * t);
                pc.Add(point);
                point = new System.Windows.Point(points[i].X + dx * t, points[i].Y + dy * t);
                pc.Add(point);
            }
            return pc.ToArray();
        }

        public static Path DrawableBezier(System.Windows.Point[] pointsArray, OpenCvSharp.Scalar color)
        {
            // var points = ControlPoints(pointsArray);
            var b = ApproximateBezier(pointsArray.ToList());// GetBezierApproximation(pointsArray, pointsArray.Length * 20);
            PathFigure pf = new PathFigure(b.Points[0], new[] { b }, false);
            PathFigureCollection pfc = [pf];
            var pge = new PathGeometry();
            pge.Figures = pfc;
            Path p = new()
            {
                Data = pge,
                Stroke = new SolidColorBrush(Color.FromRgb((byte)color.Val0, (byte)color.Val1, (byte)color.Val2))
            };
            return p;
            //var bounds = p.Data.GetRenderBounds(null);
            //p.Measure(bounds.Size);
            //p.Arrange(bounds);

            //var bitmap = new RenderTargetBitmap(
            //    (int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            //bitmap.Render(p);
            //Cv2.ImWrite(@"C:\Users\gizmo\Downloads\bezier.png", bitmap.ToMat());
        }
        static PolyLineSegment ApproximateBezier(List<System.Windows.Point> controlPoints)
        {
            var points = new PolyLineSegment();

            if (controlPoints.Count < 2)
            {
                throw new ArgumentException("At least two control points are required for a PolyLineSegment.");
            }

            for (var i = 0; i < controlPoints.Count - 1; i++)
            {
                var start = controlPoints[i];
                var end = controlPoints[i + 1];
                var bezierApproximation = ApproximateCubicBezier(start, end, i, controlPoints);
                points.Points.Add(bezierApproximation.Item2);
            }

            return points;
        }

        static Tuple<System.Windows.Point, System.Windows.Point> ApproximateCubicBezier(System.Windows.Point start, System.Windows.Point end, int i, List<System.Windows.Point> controlPoints)
        {
            var p0 = start;
            var p3 = end;
            var p1 = controlPoints[i];
            var p2 = controlPoints[i + 1];

            var t = 0.1; // Adjust this value based on the desired granularity of the approximation

            var q0 = new System.Windows.Point((1 - t) * p0.X + t * p1.X, (1 - t) * p0.Y + t * p1.Y);
            var q1 = new System.Windows.Point((1 - t) * p1.X + t * p2.X, (1 - t) * p1.Y + t * p2.Y);
            var q2 = new System.Windows.Point((1 - t) * p2.X + t * p3.X, (1 - t) * p2.Y + t * p3.Y);

            var r0 = new System.Windows.Point((1 - t) * q0.X + t * q1.X, (1 - t) * q0.Y + t * q1.Y);
            var r1 = new System.Windows.Point((1 - t) * q1.X + t * q2.X, (1 - t) * q1.Y + t * q2.Y);

            var approximation = Tuple.Create(q0, r1);
            return approximation;
        }

        private static PolyLineSegment GetBezierApproximation(System.Windows.Point[] controlPoints, int outputSegmentCount)
        {
            System.Windows.Point[] points = new System.Windows.Point[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return new PolyLineSegment(points, true);
        }

        private static System.Windows.Point GetBezierPoint(double t, System.Windows.Point[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new System.Windows.Point((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
        }
    }
}
