using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Tools.ElectricColumn.iservices;
using CadDev.Utils.Compares;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using System.Linq;

namespace CadDev.Tools.ElectricColumn.services
{
    public class ElectricColumnService : IElectricColumnService
    {
        private Transaction _ts;
        private Database _db;

        public ElectricColumnService(Transaction ts, Database db)
        {
            _ts = ts;
            _db = db;
        }

        public void GetControlPoints(List<Point3d> ps, out Point3d MaxXMinY, out Point3d MinXMinY, out Point3d MaxXMaxY, out Point3d MinXMaxY)
        {
            MaxXMinY = new Point3d();
            MinXMinY = new Point3d();
            MaxXMaxY = new Point3d();
            MinXMaxY = new Point3d();
            var gp = ps
                .OrderBy(x => Math.Round(x.Y, 4))
                .GroupBy(x => Math.Round(x.Y, 4))
                .Select(x => x.ToList());

            var gpMinY = gp.FirstOrDefault();
            var gpMaxY = gp.LastOrDefault();

            if (gpMinY.Count >= 2)
            {
                var gpMinYSort = gpMinY.OrderBy(x => x.X).ToList();
                MinXMinY = gpMinYSort.First();
                MaxXMinY = gpMinYSort.Last();
            }

            if (gpMaxY.Count >= 2)
            {
                var gpMaxYYSort = gpMaxY.OrderBy(x => x.X).ToList();
                MinXMaxY = gpMaxYYSort.First();
                MaxXMaxY = gpMaxYYSort.Last();
            }
        }

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
            out List<Point3d> endFace)
        {
            widthMax = 0;
            widthMin = 0;
            startFace = new List<Point3d>();
            midFace = new List<Point3d>();
            endFace = new List<Point3d>();
            try
            {
                if (MaxXMinY.IsSeem(MinXMinY)) throw new System.Exception("Leu leu");
                if (MaxXMaxY.IsSeem(MinXMaxY)) throw new System.Exception("Leu leu");
                widthMax = MaxXMinY.Distance(MinXMinY);
                widthMin = MaxXMaxY.Distance(MinXMaxY);
                var pss = GetPointsMidElectricColumn(ps, widthMin);

                var dir = Vector3d.XAxis;
                var nor = Vector3d.YAxis;

                //startFace
                var startFace_basePoint = new Point3d(basePoint.X, basePoint.Y, basePoint.Z);
                var startFace_p1 = startFace_basePoint - dir * widthMax / 2 - nor * widthMax / 2;
                var startFace_p2 = startFace_basePoint - dir * widthMax / 2 + nor * widthMax / 2;
                var startFace_p3 = startFace_basePoint + dir * widthMax / 2 + nor * widthMax / 2;
                var startFace_p4 = startFace_basePoint + dir * widthMax / 2 - nor * widthMax / 2;

                startFace.Add(startFace_p1);
                startFace.Add(startFace_p2);
                startFace.Add(startFace_p3);
                startFace.Add(startFace_p4);

                //MidFace
                var midFace_basePoint = new Point3d(basePoint.X, basePoint.Y, basePoint.Z + pss.First().Y - MaxXMinY.Y);
                var midFace_p1 = midFace_basePoint - dir * widthMin / 2 - nor * widthMin / 2;
                var midFace_p2 = midFace_basePoint - dir * widthMin / 2 + nor * widthMin / 2;
                var midFace_p3 = midFace_basePoint + dir * widthMin / 2 + nor * widthMin / 2;
                var midFace_p4 = midFace_basePoint + dir * widthMin / 2 - nor * widthMin / 2;

                midFace.Add(midFace_p1);
                midFace.Add(midFace_p2);
                midFace.Add(midFace_p3);
                midFace.Add(midFace_p4);

                //EndFace
                var endFace_basePoint = new Point3d(basePoint.X, basePoint.Y, basePoint.Z + midFace_basePoint.Z + MaxXMaxY.Y - pss.First().Y);
                var endFace_p1 = endFace_basePoint - dir * widthMin / 2 - nor * widthMin / 2;
                var endFace_p2 = endFace_basePoint - dir * widthMin / 2 + nor * widthMin / 2;
                var endFace_p3 = endFace_basePoint + dir * widthMin / 2 + nor * widthMin / 2;
                var endFace_p4 = endFace_basePoint + dir * widthMin / 2 - nor * widthMin / 2;

                endFace.Add(endFace_p1);
                endFace.Add(endFace_p2);
                endFace.Add(endFace_p3);
                endFace.Add(endFace_p4);

            }
            catch (Exception)
            {
                widthMax = 0;
                widthMin = 0;
                startFace = new List<Point3d>();
                midFace = new List<Point3d>();
                endFace = new List<Point3d>();
            }
        }

        public void GetLinesOfFourDirection(
            IEnumerable<Line> ls, 
            double minY,
            Point3d centerBase,
            Point3d centerBaseInstall,
            out IEnumerable<LineCad> LinesFaceY, 
            out IEnumerable<LineCad> LinesFaceX)
        {
            LinesFaceY = new List<LineCad>();
            LinesFaceX = new List<LineCad>();
            try
            {
                var vt = centerBaseInstall - centerBase;
                LinesFaceY = ls
                    .Select(x => new LineCad(_ts, _db, new Point3d(x.StartPoint.X + vt.X, 0, x.StartPoint.Y - minY + vt.Y), new Point3d(x.EndPoint.X + vt.X, 0, x.EndPoint.Y - minY + vt.Y)));

                LinesFaceX = LinesFaceY.Select(x =>
                {
                    var p1 = x.StartP.RotateBy(Math.PI / 2, Vector3d.ZAxis, new Point3d());
                    var p2 = x.EndP.RotateBy(Math.PI / 2, Vector3d.ZAxis, new Point3d());
                    return new LineCad(_ts, _db, p1, p2);
                });
            }
            catch (Exception)
            {
                LinesFaceY = new List<LineCad>();
            }
        }

        private List<Point3d> GetPointsMidElectricColumn(List<Point3d> ps, double widthMin)
        {
            var results = new List<Point3d>();
            try
            {
                var pss = ps
                .OrderBy(x => Math.Round(x.Y, 4))
                .GroupBy(x => Math.Round(x.Y, 4))
                .Select(x => x.ToList())
                .Where(x => x.Count == 2)
                .Where(x =>
                {
                    var p1 = x.First();
                    var p2 = x.Last();
                    var d = p1.Distance(p2);
                    return d.IsEqual(widthMin);
                })
                .FirstOrDefault()
                .OrderBy(x => x.X);

                results.Add(pss.FirstOrDefault());
                results.Add(pss.LastOrDefault());
            }
            catch (System.Exception)
            {
            }
            return results;
        }
    }
}
