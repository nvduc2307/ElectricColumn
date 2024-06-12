using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;

namespace CadDev.Tools.ElectricColumn.models
{
    public class ElectricColumn
    {
        /// <summary>
        /// Mặt cắt phải được vẽ trong mặt phẳng OXY
        /// Hướng cột điện hướng theo trục OY
        /// Chọn các đối tượng line của mặt cắt không có tai trước.
        /// LinesNoEars là các line ở mặt cắt không có tai.
        /// LinesHasEars là các line ở mặt cắt có tai.
        /// ElectricColumnControlPoints là các điểm khống chế của cột điện mặt cắt không có tai (xét trên mặt phẳng OXY);
        /// PointBase là điểm đặt cột điện ở vị trí mặt cắt (trọng tâm mặt cắt ngang thấp nhất).
        /// PointBaseInstall là điểm đặt cột điện khi vẽ 3d.
        /// FacesElectricColumn 8 face tương ứng với 4 hướng Đông Tây Nam Bắc (EWSN).
        /// LineFaceYs là các line dùng để chiếu lên mặt phẳng OXZ (dùng vt OY để chiếu)
        /// LineFaceXs là các line dùng để chiếu lên mặt phẳng OYZ (dùng vt OX để chiếu)
        /// LinesBody là tất cả các line của cột điện bao gồm LineFaceYs và LineFaceXs (line duy nhất).
        /// </summary>
        private Transaction _ts;
        private Database _db;

        public Point3d PointBase {  get; set; }
        public Point3d PointBaseInstall {  get; set; }
        public IEnumerable<Line> LinesNoEars { get; set; }
        public IEnumerable<Line> LinesHasEars { get; set; }
        public IEnumerable<LineCad> LinesBody { get; set; }
        public IEnumerable<LineCad> LineFaceYs { get; set; }
        public IEnumerable<LineCad> LineFaceXs { get; set; }
        public FacesElectricColumn FacesElectricColumn { get; set; }
        public ElectricColumnControlPoints ElectricColumnControlPoints { get; set; }

        public ElectricColumn(Transaction ts, Database db, IEnumerable<Line> linesNoEars, IEnumerable<Line> linesHasEars)
        {
            _ts = ts;
            _db = db;
            LinesNoEars = linesNoEars;
            LinesHasEars = linesHasEars;

            var ps = new List<Point3d>();
            ElectricColumnControlPoints = new ElectricColumnControlPoints();
            GetControlPoints(ps, out Point3d MaxXMinY, out Point3d MinXMinY, out Point3d MaxXMaxY, out Point3d MinXMaxY);

            ElectricColumnControlPoints.MaxXMinY = MaxXMinY;
            ElectricColumnControlPoints.MinXMinY = MinXMinY;
            ElectricColumnControlPoints.MaxXMaxY = MaxXMaxY;
            ElectricColumnControlPoints.MinXMaxY = MinXMaxY;

            var cp = MaxXMinY.MidPoint(MinXMinY);
            PointBase = new Point3d(cp.X, 0, cp.Y - MinXMinY.Y);
            PointBaseInstall = new Point3d();

        }
        private void GetControlPoints(List<Point3d> ps, out Point3d MaxXMinY, out Point3d MinXMinY, out Point3d MaxXMaxY, out Point3d MinXMaxY)
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
    }
    public class ElectricColumnControlPoints
    {
        public Point3d MaxXMinY { get; set; }
        public Point3d MinXMinY { get; set; }
        public Point3d MaxXMaxY { get; set; }
        public Point3d MinXMaxY { get; set; }
        public Point3d MaxXMidY { get; set; }
        public Point3d MinXMidY { get; set; }
    }
}
