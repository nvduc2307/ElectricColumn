using Autodesk.AutoCAD.Geometry;
using System.Windows.Media.Media3D;
using Point = System.Windows.Point;

namespace CadDev.Utils.CanvasUtils.Utils
{
    public static class PointExt
    {
        public static Point ToPoint2D(this Point3D point3D)
        {
            return new Point(point3D.X, point3D.Y);
        }
        public static Point3D ToPoint3D(this System.Windows.Point point, double z = 0)
        {
            return new Point3D(point.X, point.Y, z);
        }
        public static Point3D ToPoint3DWindow(this Point3d point)
        {
            return new Point3D(point.X, point.Y, point.Z);
        }
        public static Point3d ToXYZ(this Point3D point)
        {
            return new Point3d(point.X, point.Y, point.Z);
        }
        public static Point Rotate(this Point point, Point ori, double angleDegrees)
        {
            if (angleDegrees == 0)
                return point;
            // Original point
            double pointX = point.X;
            double pointY = point.Y;

            // Origin point
            double originX = ori.X;
            double originY = ori.Y;

            // Convert the angle to radians
            double angleRadians = Math.PI * angleDegrees / 180.0;

            // Translate the point to the origin
            double translatedX = pointX - originX;
            double translatedY = pointY - originY;

            // Calculate the new coordinates after rotation
            double rotatedX = translatedX * Math.Cos(angleRadians) - translatedY * Math.Sin(angleRadians);
            double rotatedY = translatedX * Math.Sin(angleRadians) + translatedY * Math.Cos(angleRadians);

            // Translate the rotated point back to its original position
            double newX = rotatedX + originX;
            double newY = rotatedY + originY;

            return new Point(newX, newY);
        }
        public static Point3D RotateOnXY(this Point3D point, Point3D ori, double angleDegrees)
        {
            if (angleDegrees == 0)
                return point;
            var p2d = new Point(point.X, point.Y);
            var pori = new Point(ori.X, ori.Y);
            p2d = p2d.Rotate(pori, angleDegrees);
            return new Point3D(p2d.X, p2d.Y, point.Z);
        }
    }
}
