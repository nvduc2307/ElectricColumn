using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Tools.ElectricColumn.viewModels;
using CadDev.Utils;
using CadDev.Utils.Compares;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumn.models
{
    public class ElectricColumn : ObservableObject
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
        /// ElectricColumnFaceBase là các danh sách điểm của từng mặt cắt ngang (mặt bằng) start (chân cột), mid (giữa cột), end (đỉnh cột),
        /// </summary>
        private Transaction _ts;
        private Database _db;
        private ElectricColumnViewModel _electricColumnViewModel;
        private ElectricColumnShortSection _electricColumnShortSectionSelected;

        public Point3d PointBase { get; set; }
        public Point3d PointBaseInstall { get; set; }
        public IEnumerable<Line> LinesNoEars { get; set; }
        public IEnumerable<Line> LinesHasEars { get; set; }
        public IEnumerable<Line> LinesEar { get; set; }
        public IEnumerable<LineCad> LinesBody { get; set; }
        public IEnumerable<LineCad> LineFaceYs { get; set; }
        public IEnumerable<LineCad> LineFaceXs { get; set; }
        public IEnumerable<LineCad> LineFaceEarYs { get; set; }
        public IEnumerable<LineCad> LineFaceEarXs { get; set; }
        public List<LineCad> LineFaceEars { get; set; } = new List<LineCad>();
        public FacesElectricColumn FacesElectricColumn { get; set; }
        public ElectricColumnFaceBase ElectricColumnFaceBase { get; set; }
        public ElectricColumnControlPoints ElectricColumnControlPoints { get; set; }
        public List<ElectricColumnShortSection> ElectricColumnShortSections { get; set; } = new List<ElectricColumnShortSection>();
        public ElectricColumnShortSection ElectricColumnShortSectionSelected
        {
            get => _electricColumnShortSectionSelected;
            set
            {
                _electricColumnShortSectionSelected = value;
                OnPropertyChanged();
            }
        }

        public ElectricColumn(Transaction ts, Database db, IEnumerable<Line> linesNoEars, IEnumerable<Line> linesHasEars)
        {
            _ts = ts;
            _db = db;
            LinesNoEars = linesNoEars;
            LinesHasEars = linesHasEars;

            var ps = LinesNoEars.GetPoints().ToList();
            ElectricColumnControlPoints = new ElectricColumnControlPoints();
            GetControlPoints(ps, out Point3d MaxXMinY, out Point3d MinXMinY, out Point3d MaxXMaxY, out Point3d MinXMaxY);

            ElectricColumnControlPoints.MaxXMinY = MaxXMinY;
            ElectricColumnControlPoints.MinXMinY = MinXMinY;
            ElectricColumnControlPoints.MaxXMaxY = MaxXMaxY;
            ElectricColumnControlPoints.MinXMaxY = MinXMaxY;

            var cp = MaxXMinY.MidPoint(MinXMinY);
            PointBase = new Point3d(cp.X, 0, cp.Y - MinXMinY.Y);
            PointBaseInstall = new Point3d();

            ElectricColumnFaceBase = new ElectricColumnFaceBase();
            GetElectricColumnFaceBase(
                PointBaseInstall,
                ps,
                MaxXMinY,
                MinXMinY,
                MaxXMaxY,
                MinXMaxY,
                out double widthMax,
                out double widthMin,
                out List<Point3d> startFace,
                out List<Point3d> midFace,
                out List<Point3d> endFace);
            ElectricColumnFaceBase.StartFace = startFace;
            ElectricColumnFaceBase.MidFace = midFace;
            ElectricColumnFaceBase.EndFace = endFace;

            FacesElectricColumn = new FacesElectricColumn(startFace, midFace, endFace);

            GetLinesFace(
                LinesNoEars,
                MinXMinY.Y,
                PointBase,
                PointBaseInstall,
                out IEnumerable<LineCad> lineFaceYs,
                out IEnumerable<LineCad> lineFaceXs);
            LineFaceYs = lineFaceYs;
            LineFaceXs = lineFaceXs;

            GetLinesBody(out List<LineCad> linesBody);
            LinesBody = linesBody;

            GetLinesEars(out List<Line> linesEar, out List<LineCad> lineFaceEarYs, out List<LineCad> lineFaceEarXs);
            LinesEar = linesEar;
            LineFaceEarYs = lineFaceEarYs;
            LineFaceEarXs = lineFaceEarXs;
            GetLinesEars(ElectricColumnEarDirectionType.DirX);
            ReConfigLinesBody();
            GetElectricColumnShortSections();
            ElectricColumnShortSectionSelected = ElectricColumnShortSections.FirstOrDefault();
        }

        public void CreateElectric()
        {
            foreach (var l in LinesBody)
            {
                l.Create();
            }
            foreach (var l in LineFaceEars)
            {
                l.Create();
            }
        }

        private void ReConfigLinesBody()
        {
            var linesExisted = new List<LineCad>();
            foreach(var l in LineFaceEars)
            {
                var check = LinesBody.Any(x =>
                {
                    var dk1 = x.Dir.IsParallelTo(l.Dir);
                    var dk2 = l.StartP.Distance(x).IsEqual(0);
                    return dk1 && dk2;
                });
                if (check) linesExisted.Add(l);
            }
            LinesBody = LinesBody.Where(x => !linesExisted.Any(y => y.MidP.IsSeem(x.MidP)));
        }

        private void GetElectricColumnShortSections()
        {
            var linesElectricColumn = LinesBody.Concat(LineFaceEars);
            var gps = linesElectricColumn.GetPoints()
                .OrderBy(x=>x.Z)
                .GroupBy(x => x.Z)
                .Select(x=>x.ToList());
            var id = 1;
            foreach (var gr in gps)
            {
                var z = gr.First().Z;
                var lines = linesElectricColumn.Where(x => x.StartP.Z.IsEqual(z) && x.MidP.Z.IsEqual(z)).ToList();
                ElectricColumnShortSections.Add(new ElectricColumnShortSection(id, lines, gr.Select(x=> new Utils.Points.PointCad(x)).ToList()));
                id++;
            }
        }

        private void GetLinesEars(ElectricColumnEarDirectionType earDirType)
        {
            switch (earDirType)
            {
                case ElectricColumnEarDirectionType.DirX:
                    foreach (var line in LineFaceEarYs)
                    {
                        if (line.MidP.Z <= ElectricColumnFaceBase.MidFace.First().Z)
                        {
                            var p1 = line.StartP.RayPointToFace(-Vector3d.YAxis, FacesElectricColumn.South1Face);
                            var p2 = line.EndP.RayPointToFace(-Vector3d.YAxis, FacesElectricColumn.South1Face);

                            var p11 = line.StartP.RayPointToFace(Vector3d.YAxis, FacesElectricColumn.North1Face);
                            var p22 = line.EndP.RayPointToFace(Vector3d.YAxis, FacesElectricColumn.North1Face);

                            var l1 = new LineCad(_ts, AC.Database, p1, p2);
                            var l2 = new LineCad(_ts, AC.Database, p11, p22);

                            var l11 = new LineCad(_ts, AC.Database, p1, p11);
                            var l22 = new LineCad(_ts, AC.Database, p2, p22);

                            LineFaceEars.Add(l1);
                            LineFaceEars.Add(l2);
                            LineFaceEars.Add(l11);
                            LineFaceEars.Add(l22);
                        }
                        else
                        {
                            var p1 = line.StartP.RayPointToFace(-Vector3d.YAxis, FacesElectricColumn.South2Face);
                            var p2 = line.EndP.RayPointToFace(-Vector3d.YAxis, FacesElectricColumn.South2Face);

                            var p11 = line.StartP.RayPointToFace(Vector3d.YAxis, FacesElectricColumn.North2Face);
                            var p22 = line.EndP.RayPointToFace(Vector3d.YAxis, FacesElectricColumn.North2Face);

                            var l1 = new LineCad(_ts, AC.Database, p1, p2);
                            var l2 = new LineCad(_ts, AC.Database, p11, p22);

                            var l11 = new LineCad(_ts, AC.Database, p1, p11);
                            var l22 = new LineCad(_ts, AC.Database, p2, p22);

                            LineFaceEars.Add(l1);
                            LineFaceEars.Add(l2);
                            LineFaceEars.Add(l11);
                            LineFaceEars.Add(l22);
                        }
                    }
                    break;
                case ElectricColumnEarDirectionType.DirY:
                    foreach (var line in LineFaceEarXs)
                    {
                        if (line.MidP.Z <= ElectricColumnFaceBase.MidFace.First().Z)
                        {
                            var p1 = line.StartP.RayPointToFace(Vector3d.XAxis, FacesElectricColumn.Earth1Face);
                            var p2 = line.EndP.RayPointToFace(Vector3d.XAxis, FacesElectricColumn.Earth1Face);

                            var p11 = line.StartP.RayPointToFace(-Vector3d.XAxis, FacesElectricColumn.West1Face);
                            var p22 = line.EndP.RayPointToFace(-Vector3d.XAxis, FacesElectricColumn.West1Face);

                            var l1 = new LineCad(_ts, AC.Database, p1, p2);
                            var l2 = new LineCad(_ts, AC.Database, p11, p22);

                            var l11 = new LineCad(_ts, AC.Database, p1, p11);
                            var l22 = new LineCad(_ts, AC.Database, p2, p22);

                            LineFaceEars.Add(l1);
                            LineFaceEars.Add(l2);
                            LineFaceEars.Add(l11);
                            LineFaceEars.Add(l22);
                        }
                        else
                        {
                            var p1 = line.StartP.RayPointToFace(Vector3d.XAxis, FacesElectricColumn.Earth2Face);
                            var p2 = line.EndP.RayPointToFace(Vector3d.XAxis, FacesElectricColumn.Earth2Face);

                            var p11 = line.StartP.RayPointToFace(-Vector3d.XAxis, FacesElectricColumn.West2Face);
                            var p22 = line.EndP.RayPointToFace(-Vector3d.XAxis, FacesElectricColumn.West2Face);

                            var l1 = new LineCad(_ts, AC.Database, p1, p2);
                            var l2 = new LineCad(_ts, AC.Database, p11, p22);

                            var l11 = new LineCad(_ts, AC.Database, p1, p11);
                            var l22 = new LineCad(_ts, AC.Database, p2, p22);

                            LineFaceEars.Add(l1);
                            LineFaceEars.Add(l2);
                            LineFaceEars.Add(l11);
                            LineFaceEars.Add(l22);
                        }
                    }
                    break;
            }
            LineFaceEars = LineFaceEars.Distinct(new CompareLines()).ToList();
        }

        private void GetLinesEars(out List<Line> linesEar, out List<LineCad> lineFaceEarYs, out List<LineCad> lineFaceEarXs)
        {
            linesEar = new List<Line>();
            var isExisted = new List<Line>();
            lineFaceEarYs = new List<LineCad>();
            lineFaceEarXs = new List<LineCad>();

            var pointNoEars = LinesNoEars.GetPoints()
                .OrderBy(x => Math.Round(x.Y, 4))
                .GroupBy(x => Math.Round(x.Y, 4))
                .Select(x => x.ToList())
                .FirstOrDefault();

            var pointHasEars = LinesHasEars.GetPoints()
                .OrderBy(x => Math.Round(x.Y, 4))
                .GroupBy(x => Math.Round(x.Y, 4))
                .Select(x => x.ToList())
                .FirstOrDefault();

            if (pointNoEars.Count() == 2 && pointHasEars.Count() == 2)
            {
                var midNoEars = pointNoEars.First().MidPoint(pointNoEars.Last());
                var midHasEars = pointHasEars.First().MidPoint(pointHasEars.Last());

                var d = midNoEars.Distance(midHasEars);
                var vtMove = (midNoEars - midHasEars).VectorNormal();

                foreach (var l in LinesHasEars)
                {
                    var check = LinesNoEars.Any(x =>
                    {
                        var dk1 = (l.StartPoint + vtMove * d).IsSeem(x.StartPoint) || (l.StartPoint + vtMove * d).IsSeem(x.EndPoint);
                        var dk2 = (l.EndPoint + vtMove * d).IsSeem(x.StartPoint) || (l.EndPoint + vtMove * d).IsSeem(x.EndPoint);
                        return dk1 && dk2;
                    });
                    if (check) isExisted.Add(l);
                }
                foreach (var l in LinesHasEars)
                {
                    if (isExisted.Any(x => x.Id == l.Id)) continue;
                    linesEar.Add(l);
                }
                foreach (var l in linesEar)
                {
                    var p1 = l.StartPoint + (PointBaseInstall - midHasEars);
                    var p2 = l.EndPoint + (PointBaseInstall - midHasEars);
                    lineFaceEarYs.Add(new LineCad(_ts, _db, new Point3d(p1.X, 0, p1.Y), new Point3d(p2.X, 0, p2.Y)));
                }
                foreach (var l in lineFaceEarYs)
                {
                    var p1 = l.StartP.RotateBy(Math.PI / 2, new Vector3d(0, 0, 1), PointBaseInstall);
                    var p2 = l.EndP.RotateBy(Math.PI / 2, new Vector3d(0, 0, 1), PointBaseInstall);
                    lineFaceEarXs.Add(new LineCad(_ts, _db, p1, p2));
                }
            }
        }

        private void GetLinesBody(out List<LineCad> linesBody)
        {
            linesBody = new List<LineCad>();

            foreach (var line in LineFaceYs)
            {
                if (line.MidP.Z <= ElectricColumnFaceBase.MidFace.First().Z)
                {
                    var p1 = line.StartP.RayPointToFace(-Vector3d.YAxis, FacesElectricColumn.South1Face);
                    var p2 = line.EndP.RayPointToFace(-Vector3d.YAxis, FacesElectricColumn.South1Face);

                    var p11 = line.StartP.RayPointToFace(Vector3d.YAxis, FacesElectricColumn.North1Face);
                    var p22 = line.EndP.RayPointToFace(Vector3d.YAxis, FacesElectricColumn.North1Face);

                    var l1 = new LineCad(_ts, AC.Database, p1, p2);
                    var l2 = new LineCad(_ts, AC.Database, p11, p22);

                    linesBody.Add(l1);
                    linesBody.Add(l2);
                }
                else
                {
                    var p1 = line.StartP.RayPointToFace(-Vector3d.YAxis, FacesElectricColumn.South2Face);
                    var p2 = line.EndP.RayPointToFace(-Vector3d.YAxis, FacesElectricColumn.South2Face);

                    var p11 = line.StartP.RayPointToFace(Vector3d.YAxis, FacesElectricColumn.North2Face);
                    var p22 = line.EndP.RayPointToFace(Vector3d.YAxis, FacesElectricColumn.North2Face);

                    var l1 = new LineCad(_ts, AC.Database, p1, p2);
                    var l2 = new LineCad(_ts, AC.Database, p11, p22);

                    linesBody.Add(l1);
                    linesBody.Add(l2);
                }
            }

            foreach (var line in LineFaceXs)
            {
                if (line.MidP.Z <= ElectricColumnFaceBase.MidFace.First().Z)
                {
                    var p1 = line.StartP.RayPointToFace(Vector3d.XAxis, FacesElectricColumn.Earth1Face);
                    var p2 = line.EndP.RayPointToFace(Vector3d.XAxis, FacesElectricColumn.Earth1Face);

                    var p11 = line.StartP.RayPointToFace(-Vector3d.XAxis, FacesElectricColumn.West1Face);
                    var p22 = line.EndP.RayPointToFace(-Vector3d.XAxis, FacesElectricColumn.West1Face);

                    var l1 = new LineCad(_ts, AC.Database, p1, p2);
                    var l2 = new LineCad(_ts, AC.Database, p11, p22);

                    linesBody.Add(l1);
                    linesBody.Add(l2);
                }
                else
                {
                    var p1 = line.StartP.RayPointToFace(Vector3d.XAxis, FacesElectricColumn.Earth2Face);
                    var p2 = line.EndP.RayPointToFace(Vector3d.XAxis, FacesElectricColumn.Earth2Face);

                    var p11 = line.StartP.RayPointToFace(-Vector3d.XAxis, FacesElectricColumn.West2Face);
                    var p22 = line.EndP.RayPointToFace(-Vector3d.XAxis, FacesElectricColumn.West2Face);

                    var l1 = new LineCad(_ts, AC.Database, p1, p2);
                    var l2 = new LineCad(_ts, AC.Database, p11, p22);

                    linesBody.Add(l1);
                    linesBody.Add(l2);
                }
            }

            linesBody = linesBody.Distinct(new CompareLines()).ToList();
        }

        private void GetElectricColumnFaceBase(
            Point3d basePointInstall,
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
                var startFace_basePoint = new Point3d(basePointInstall.X, basePointInstall.Y, basePointInstall.Z);
                var startFace_p1 = startFace_basePoint - dir * widthMax / 2 - nor * widthMax / 2;
                var startFace_p2 = startFace_basePoint - dir * widthMax / 2 + nor * widthMax / 2;
                var startFace_p3 = startFace_basePoint + dir * widthMax / 2 + nor * widthMax / 2;
                var startFace_p4 = startFace_basePoint + dir * widthMax / 2 - nor * widthMax / 2;

                startFace.Add(startFace_p1);
                startFace.Add(startFace_p2);
                startFace.Add(startFace_p3);
                startFace.Add(startFace_p4);

                //MidFace
                var midFace_basePoint = new Point3d(basePointInstall.X, basePointInstall.Y, basePointInstall.Z + pss.First().Y - MaxXMinY.Y);
                var midFace_p1 = midFace_basePoint - dir * widthMin / 2 - nor * widthMin / 2;
                var midFace_p2 = midFace_basePoint - dir * widthMin / 2 + nor * widthMin / 2;
                var midFace_p3 = midFace_basePoint + dir * widthMin / 2 + nor * widthMin / 2;
                var midFace_p4 = midFace_basePoint + dir * widthMin / 2 - nor * widthMin / 2;

                midFace.Add(midFace_p1);
                midFace.Add(midFace_p2);
                midFace.Add(midFace_p3);
                midFace.Add(midFace_p4);

                //EndFace
                var endFace_basePoint = new Point3d(basePointInstall.X, basePointInstall.Y, basePointInstall.Z + midFace_basePoint.Z + MaxXMaxY.Y - pss.First().Y);
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

        private void GetLinesFace(
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

    public class ElectricColumnFaceBase
    {
        public IEnumerable<Point3d> StartFace { get; set; }
        public IEnumerable<Point3d> MidFace { get; set; }
        public IEnumerable<Point3d> EndFace { get; set; }
    }
    public enum ElectricColumnEarDirectionType
    {
        DirX = 1,
        DirY = 2,
    }
}
