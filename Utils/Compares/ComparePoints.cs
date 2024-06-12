using Autodesk.AutoCAD.Geometry;
using System.Collections;

namespace CadDev.Utils.Compares
{
    public class ComparePoints : IEqualityComparer<Point3d>
    {
        public bool Equals(Point3d x, Point3d y)
        {
            return x.X.IsEqual(y.X) && x.Y.IsEqual(y.Y) && x.Z.IsEqual(y.Z);
        }

        public int GetHashCode(Point3d obj)
        {
            return 0;
        }
    }

    public static class Points
    {
        public static bool IsSeem(this Point3d x, Point3d y)
        {
            return x.X.IsEqual(y.X) && x.Y.IsEqual(y.Y) && x.Z.IsEqual(y.Z);
        }
    }
}
