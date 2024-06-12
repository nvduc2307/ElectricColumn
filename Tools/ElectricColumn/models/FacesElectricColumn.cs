using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Faces;

namespace CadDev.Tools.ElectricColumn.models
{
    public class FacesElectricColumn
    {
        public FaceCad Earth1Face { get; set; }
        public FaceCad Earth2Face { get; set; }
        public FaceCad West1Face { get; set; }
        public FaceCad West2Face { get; set; }
        public FaceCad South1Face { get; set; }
        public FaceCad South2Face { get; set; }
        public FaceCad North1Face { get; set; }
        public FaceCad North2Face { get; set; }

        public FacesElectricColumn(List<Point3d> startFace, List<Point3d> midFace, List<Point3d> endFace)
        {
            GetFaces(startFace, midFace, endFace);
        }
        private void GetFaces(List<Point3d> startFace, List<Point3d> midFace, List<Point3d> endFace)
        {
            Earth1Face = new FaceCad(startFace[3], startFace[2], midFace[3]);
            North1Face = new FaceCad(startFace[2], startFace[1], midFace[2]);
            West1Face = new FaceCad(startFace[1], startFace[0], midFace[1]);
            South1Face = new FaceCad(startFace[0], startFace[3], midFace[0]);

            Earth2Face = new FaceCad(midFace[3], midFace[2], endFace[3]);
            North2Face = new FaceCad(midFace[2], midFace[1], endFace[2]);
            West2Face = new FaceCad(midFace[1], midFace[0], endFace[1]);
            South2Face = new FaceCad(midFace[0], midFace[3], endFace[0]);
        }
    }
}
