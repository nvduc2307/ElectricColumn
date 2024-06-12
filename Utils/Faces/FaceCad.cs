﻿using Autodesk.AutoCAD.Geometry;

namespace CadDev.Utils.Faces
{
    public class FaceCad
    {
        public Vector3d Normal { get; set; }
        public Point3d BasePoint { get; set; }

        public FaceCad(Vector3d normal, Point3d basePoint)
        {
            Normal = normal;
            BasePoint = basePoint;
        }
        public FaceCad(Point3d p1, Point3d p2, Point3d p3)
        {
            BasePoint = p1;
            var vt1 = (p2 - p1).GetNormal();
            var vt2 = (p3 - p1).GetNormal();
            Normal = vt1.CrossProduct(vt2).GetNormal();
        }
    }
}
