using System.Windows.Media.Media3D;

namespace CadDev.Utils.CanvasUtils.Utils
{
	public static class ThreeDExt
	{
		public static Vector3D AxisX => new Vector3D(1, 0, 0);
		public static Vector3D AxisY => new Vector3D(0, 1, 0);
		public static Vector3D AxisZ => new Vector3D(0, 0, 1);
		public static Vector3D CanvasAxisX => new Vector3D(1, 0, 0);
		public static Vector3D CanvasAxisY => new Vector3D(0, -1, 0);
		public static Point3D MidPointWith(this Point3D p1, Point3D p2)
		{
			var x = (p1.X + p2.X) / 2;
			var y = (p1.Y + p2.Y) / 2;
			var z = (p1.Z + p2.Z) / 2;
			return new Point3D(x, y, z);
		}
		public static double Dot(this Vector3D p1, Vector3D p2)
		{
			return Vector3D.DotProduct(p1, p2);
		}
	}
}
