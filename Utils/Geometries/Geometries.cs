using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;

namespace CadDev.Utils.Geometries
{
    public static class Geometries
    {
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
    }
}
