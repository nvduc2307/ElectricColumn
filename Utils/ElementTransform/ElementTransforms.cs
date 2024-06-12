using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CadDev.Utils.ElementTransform
{
    public static class ElementTransforms
    {
        public static void Move(this Entity elementInCad, Vector3d vector3D)
        {
            elementInCad.TransformBy(Matrix3d.Displacement(vector3D));
        }
        public static void Rotate(this Entity elementInCad, Vector3d axis, Point3d center, double angleDeg)
        {
            elementInCad.TransformBy(Matrix3d.Rotation(angleDeg, axis, center));
        }
    }
}
