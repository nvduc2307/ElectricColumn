using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;
using CadDev.Utils.Lines;

namespace CadDev.Utils.Geometries
{
    public static class Geometries
    {
        public static double Distance(this Point3d p, LineCad l)
        {
            var d = 0.0;
            try
            {
                d = p.Distance(l.StartP);
                var dir = l.Dir;
                var vt = (l.StartP - p).GetNormal();
                if (dir.DotProduct(vt).IsEqual(0)) return p.Distance(l.StartP);
                if (Math.Abs(dir.DotProduct(vt)).IsEqual(1)) return 0;

                var angle = dir.DotProduct(vt) > 0
                    ? vt.AngleTo(dir)
                    : vt.AngleTo(-dir);
                d = Math.Sin(angle) * d;
            }
            catch (Exception)
            {
                d = 0.0;
            }
            return d;
        }

        public static double Distance(this Point3d p)
        {
            var result = 0.0;
            try
            {
                result = Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static double Distance(this Vector3d vt)
        {
            var result = 0.0;
            try
            {
                result = Math.Sqrt(vt.X * vt.X + vt.Y * vt.Y + vt.Z * vt.Z);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static double Distance(this Point3d p1, Point3d p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;
            var z = p1.Z - p2.Z;
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public static double Distance(this Point3d p, FaceCad faceCad)
        {
            var result = 0.0;
            try
            {
                var d = p.Distance(faceCad.BasePoint);
                var vt = (faceCad.BasePoint - p).VectorNormal();
                var angle = faceCad.Normal.DotProduct(vt) >= 0
                    ? faceCad.Normal.AngleTo(vt)
                    : faceCad.Normal.AngleTo(-vt);
                result = Math.Cos(angle) * d;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static Vector3d VectorNormal(this Vector3d vt)
        {
            return vt / vt.Distance();
        }

        public static Point3d MidPoint(this Point3d p1, Point3d p2)
        {
            var x = (p1.X + p2.X) * 0.5;
            var y = (p1.Y + p2.Y) * 0.5;
            var z = (p1.Z + p2.Z) * 0.5;
            return new Point3d(x, y, z);
        }

        public static Point3d RayPointToFace(this Point3d p, Vector3d vtRay, FaceCad faceCad)
        {
            Point3d result = p;
            try
            {
                var vt = (faceCad.BasePoint - p).VectorNormal();
                var normalFace = vt.DotProduct(faceCad.Normal) >= 0 ? faceCad.Normal : -faceCad.Normal;
                var angle1 = normalFace.AngleTo(vt);
                var angle2 = normalFace.AngleTo(vtRay);

                var angle1D = normalFace.AngleTo(vt) * 180 / Math.PI;
                var angle2D = normalFace.AngleTo(vtRay) * 180 / Math.PI;

                var dm = p.Distance(faceCad.BasePoint);

                var dd = p.Distance(faceCad);

                var d = Math.Cos(angle1) * p.Distance(faceCad.BasePoint) / Math.Cos(angle2);
                result = p + vtRay * d;
            }
            catch (Exception)
            {
                result = p;
            }
            return result;
        }

        public static double AngleTo(this Vector3d vt1, Vector3d vt2)
        {
            var result = 0.0;
            try
            {
                var cos = vt1.DotProduct(vt2) / (vt1.Distance() * vt2.Distance());
                result = Math.Acos(cos);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static Point3d Round(this Point3d p, int n = 4)
        {
            return new Point3d(Math.Round(p.X, n), Math.Round(p.Y, n), Math.Round(p.Z, n));
        }

        public static Point3d PointToLine(this Point3d p, Line l)
        {
            var result = p;
            try
            {
                var dir = (l.EndPoint - l.StartPoint).GetNormal();
                var d = p.Distance(l.StartPoint);
                var vt = (l.StartPoint - p).GetNormal();
                if (dir.DotProduct(vt).IsEqual(0)) return l.StartPoint;
                if (Math.Abs(dir.DotProduct(vt)).IsEqual(1, 0.00000001)) return p;

                var normal = dir.CrossProduct(vt);

                var vti = dir.CrossProduct(normal).DotProduct(vt) > 0
                    ? dir.CrossProduct(normal).GetNormal()
                    : -dir.CrossProduct(normal).GetNormal();

                var angle = dir.DotProduct(vt) > 0
                    ? vt.AngleTo(dir)
                    : vt.AngleTo(-dir);

                d = Math.Sin(angle) * d;

                result = p + vti * d;
            }
            catch (Exception)
            {
                result = p;
            }
            //ddang sai
            return result;
        }

        public static Point3d Mirror(this Point3d p, Line l)
        {
            var pm = p.PointToLine(l);
            return p.Mirror(pm);
        }

        public static Point3d Mirror(this Point3d p, Point3d pc)
        {
            return new Point3d(pc.X * 2 - p.X, pc.Y * 2 - p.Y, pc.Z * 2 - p.Z);
        }

        public static System.Windows.Point GetCenterCanvas(this IEnumerable<System.Windows.Point> points)
        {
            var result = new System.Windows.Point();
            try
            {
                var maxx = points.Max(x => x.X);
                var minx = points.Min(x => x.X);
                var maxy = points.Max(x => x.Y);
                var miny = points.Min(y => y.Y);
                result = new System.Windows.Point(0.5 * (maxx + minx), 0.5 * (maxy + miny));
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
