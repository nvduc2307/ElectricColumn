using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Lines;

namespace CadDev.Tools.ElectricColumn.iservices
{
    public interface IElectricColumnService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="MaxXMinY"></param>
        /// <param name="MinXMinY"></param>
        /// <param name="MaxXMaxY"></param>
        /// <param name="MinXMaxY"></param>
        public void GetControlPoints(List<Point3d> ps, out Point3d MaxXMinY, out Point3d MinXMinY, out Point3d MaxXMaxY, out Point3d MinXMaxY);
        /// <summary>
        /// create first face (mat phang thap nhat)
        /// create second face (mat phang o giua)
        /// create end face (mat phang tren cung)
        /// </summary>
        public void GetFaceBaseElectricColumn(
            Point3d basePoint,
            List<Point3d> ps, 
            Point3d MaxXMinY, 
            Point3d MinXMinY, 
            Point3d MaxXMaxY, 
            Point3d MinXMaxY,
            out double widthMax, 
            out double widthMin, 
            out List<Point3d> startFace, 
            out List<Point3d> midFace, 
            out List<Point3d> endFace);

        public void GetLinesOfFourDirection(
            IEnumerable<Line> ls, 
            double minY, 
            Point3d centerBase, 
            Point3d centerBaseInstall, 
            out IEnumerable<LineCad> linesFaceY, 
            out IEnumerable<LineCad> linesFaceX);
    }
}
