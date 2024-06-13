using Autodesk.AutoCAD.Geometry;
using PointCanvas = System.Windows.Point;

namespace CadDev.Utils.CanvasUtils
{
    public static class GeometryInCanvas
    {
        public static double Distance(this PointCanvas vt)
        {
            return Math.Sqrt(vt.X * vt.X + vt.Y * vt.Y);
        }
        public static PointCanvas GetVector(this PointCanvas p1, PointCanvas p2)
        {
            return new PointCanvas(p2.X - p1.X, p2.Y - p1.Y);
        }
        public static PointCanvas VectorInit(this PointCanvas vt)
        {
            var d = vt.Distance();
            return new PointCanvas(vt.X / d, vt.Y / d);
        }
        public static IEnumerable<PointCanvas> ConvertPoint(this IEnumerable<Point3d> points)
        {
            var result = new List<PointCanvas>();
            if (points.Count() > 0)
            {
                foreach (var point in points)
                {
                    result.Add(point.ConvertPoint());
                }
            }
            return result;
        }
        public static PointCanvas ConvertPoint(this Point3d point)
        {
            return new PointCanvas(point.X, -point.Y);
        }
        public static PointCanvas ConvertPointOXZToOXY(this Point3d point)
        {
            return new PointCanvas(point.X, -point.Z);
        }
        public static PointCanvas Rotate(this PointCanvas p, PointCanvas c, double angle)
        {
            var x = (p.X - c.X) * Math.Cos(angle) - (p.Y - c.Y) * Math.Sin(angle) + c.X;
            var y = (p.X - c.X) * Math.Sin(angle) + (p.Y - c.Y) * Math.Cos(angle) + c.Y;
            return new PointCanvas(x, y);
        }
    }
}
