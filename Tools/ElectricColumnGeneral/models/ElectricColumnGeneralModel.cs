using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Messages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class ElectricColumnGeneralModel : ObservableObject
    {
        /// <summary>
        /// các tiết diện ban đầu phải được vẽ ở mặt phẳng OXY (chiều OY là hướng chiều cao của cột)
        /// LinesMainFace là tất cả các line ở mặt phẳng chính (chứa tai)
        /// LinesSubFace là tất cả các line ở mặt phẳng phụ (không có tai)
        /// LinesMainFaceRight là các line chứa các mặt phẳng chiếu bên phải của MainFace
        /// LinesMainFaceLeft là các line chứa các mặt phẳng chiếu bên trái của MainFace
        /// LinesSubFaceRight là các line chứa các mặt phẳng chiếu bên phải của SubFace
        /// LinesSubFaceLeft là các line chứa các mặt phẳng chiếu bên trái của SubFace
        /// Axis là trục của cột điện
        /// </summary>
        public Transaction _ts { get; set; }
        public Database _db { get; set; }

        private ElectricColumnSectionPlane _sectionPlaneSelected;

        public ElectricColumnUIElementModel UIElement { get; set; }

        public Point3d BasePointInstall { get; }
        public Point3d BasePointCurrentMainFace { get; }
        public Point3d BasePointCurrentSubFace { get; }
        public Vector3d VectorMoveMainFace { get; }
        public Vector3d VectorMoveSubFace { get; }

        public Vector3d VectorBaseMain { get; } // vtX(1, 0, 0)
        public Vector3d VectorBaseSub { get; }// vtX(0, 1, 0)

        public LineCad AxisMainFace { get; set; }
        public LineCad AxisSubFace { get; set; }

        public List<LineCad> LinesMain { get; set; } // bao gom ca swing
        public List<LineCad> LinesBodyMain { get; set; } // khong bao gom ca swing
        public List<LineCad> LinesSub { get; set; }
        public List<LineCad> LinesSwing { get; set; }

        public List<LineCad> LinesMainFaceRight { get; set; }
        public List<LineCad> LinesMainFaceLeft { get; set; }
        public List<LineCad> LinesSubFaceRight { get; set; }
        public List<LineCad> LinesSubFaceLeft { get; set; }

        public List<FaceCad> FacesMainFaceRight { get; set; }
        public List<FaceCad> FacesMainFaceLeft { get; set; }
        public List<FaceCad> FacesSubFaceRight { get; set; }
        public List<FaceCad> FacesSubFaceLeft { get; set; }

        public List<LineCad> LinesSouth { get; set; }
        public List<LineCad> LinesEarth { get; set; }
        public List<LineCad> LinesNorth { get; set; }
        public List<LineCad> LinesWest { get; set; }

        public List<ElectricColumnSectionPlane> SectionPlanes { get; set; }
        public ElectricColumnSectionPlane SectionPlaneSelected
        {
            get { return _sectionPlaneSelected; }
            set
            {
                _sectionPlaneSelected = value;
                OnPropertyChanged();
                if (UIElement != null)
                {
                    ElectricColumnUIElementModel.DrawSectionPlan(UIElement.SectionPlaneCanvas, this);
                    ElectricColumnUIElementModel.UpdateStatusSectionSelectedAtElevation(UIElement.SectionElevationCanvas, this);
                }
            }
        }

        public ElectricColumnSwingModel ElectricColumnSwingModel { get; set; }

        public ElectricColumnGeneralModel(
            Transaction ts,
            Database db,
            Line axisMainFace,
            Line axisSubFace,
            IEnumerable<Line> linesMain,
            IEnumerable<Line> linesFaceMainPerSide,
            IEnumerable<Line> linesSub,
            IEnumerable<Line> linesFaceSubPerSide)
        {
            _ts = ts;
            _db = db;
            VectorBaseMain = new Vector3d(1, 0, 0);
            VectorBaseSub = new Vector3d(0, 1, 0);
            BasePointInstall = new Point3d();
            AxisMainFace = GetAxis(axisMainFace);
            AxisSubFace = GetAxis(axisSubFace);
            BasePointCurrentMainFace = GetCenterCurrent(linesMain, AxisMainFace);
            BasePointCurrentSubFace = GetCenterCurrent(linesMain, AxisSubFace);
            VectorMoveMainFace = BasePointInstall - BasePointCurrentMainFace;
            VectorMoveSubFace = BasePointInstall - BasePointCurrentSubFace;
            LinesMain = GetLines(linesMain, VectorMoveMainFace);
            LinesSub = GetLines(linesSub, VectorMoveSubFace, Math.PI / 2);

            GetLinesFace(
                linesFaceMainPerSide,
                axisMainFace,
                VectorMoveMainFace,
                out List<LineCad> linesFace1,
                out List<LineCad> linesFace2);
            LinesMainFaceRight = linesFace1;
            LinesMainFaceLeft = linesFace2;

            GetLinesFace(
                linesFaceSubPerSide,
                axisSubFace,
                VectorMoveSubFace,
                out List<LineCad> linesFace11,
                out List<LineCad> linesFace22, Math.PI / 2);
            LinesSubFaceRight = linesFace11;
            LinesSubFaceLeft = linesFace22;

            FacesMainFaceRight = GetFaces(linesFace1, ElectricColumnFaceType.MainFace);
            FacesMainFaceLeft = GetFaces(linesFace2, ElectricColumnFaceType.MainFace);
            FacesSubFaceRight = GetFaces(linesFace11, ElectricColumnFaceType.SubFace);
            FacesSubFaceLeft = GetFaces(linesFace22, ElectricColumnFaceType.SubFace);

            ElectricColumnSwingModel = new ElectricColumnSwingModel(this);
            LinesSwing = ElectricColumnSwingModel.SwingRights.Concat(ElectricColumnSwingModel.SwingLefts).ToList();
            LinesBodyMain = LinesMain
                .Where(x => !LinesSwing.Any(y => y.IsSeem(x)))
                .ToList();

            LinesSouth = GetLinesBody(LinesBodyMain, FacesSubFaceLeft, ElectricColumnFaceType.MainFace);
            LinesNorth = GetLinesBody(LinesBodyMain, FacesSubFaceRight, ElectricColumnFaceType.MainFace);
            LinesWest = GetLinesBody(LinesSub, FacesMainFaceLeft, ElectricColumnFaceType.SubFace);
            LinesEarth = GetLinesBody(LinesSub, FacesMainFaceRight, ElectricColumnFaceType.SubFace);

            SectionPlanes = GetSectionPlanes();
            SectionPlaneSelected = SectionPlanes.FirstOrDefault();
        }

        private List<LineCad> GetLinesBody(List<LineCad> lines, List<FaceCad> faces, ElectricColumnFaceType electricColumnFaceType)
        {
            var results = new List<LineCad>();
            try
            {
                var vtRay = electricColumnFaceType == ElectricColumnFaceType.MainFace ? new Vector3d(0, 1, 0) : new Vector3d(1, 0, 0);
                foreach (var f in faces)
                {
                    var maxZF = Math.Max(f.BaseLine.StartP.Z, f.BaseLine.EndP.Z);
                    var minZF = Math.Min(f.BaseLine.StartP.Z, f.BaseLine.EndP.Z);
                    var ls = lines.Where(x =>
                    {
                        var dk1 = x.MidP.Z < maxZF && x.MidP.Z > minZF;
                        var dk2 = x.MidP.Z.IsEqual(maxZF);
                        var dk3 = x.MidP.Z.IsEqual(minZF);
                        return dk1 || dk2 || dk3;
                    });
                    foreach (var l in ls)
                    {
                        var p1 = l.StartP.RayPointToFace(vtRay, f);
                        var p2 = l.EndP.RayPointToFace(vtRay, f);
                        results.Add(new LineCad(_ts, _db, p1, p2));
                    }
                }
            }
            catch (Exception ex)
            {
                IO.ShowException(ex);
            }
            return results.Distinct(new CompareLines()).ToList();
        }

        private List<FaceCad> GetFaces(List<LineCad> linesFace, ElectricColumnFaceType electricColumnFaceType)
        {
            var norFace = electricColumnFaceType == ElectricColumnFaceType.MainFace
                ? new Vector3d(0, 1, 0)
                : new Vector3d(1, 0, 0);
            var results = new List<FaceCad>();
            try
            {
                foreach (LineCad line in linesFace)
                {
                    var dir = line.Dir;
                    var nor = dir.CrossProduct(norFace);
                    results.Add(new FaceCad(nor, line));
                }
            }
            catch (Exception)
            {
                results = new List<FaceCad>();
            }
            return results;
        }

        private void GetLinesFace(
            IEnumerable<Line> lines,
            Line axis,
            Vector3d vtMove,
            out List<LineCad> linesFace1,
            out List<LineCad> linesFace2,
            double angle = 0)
        {
            linesFace1 = new List<LineCad>();
            linesFace2 = new List<LineCad>();

            foreach (var l in lines)
            {
                var p1b = l.StartPoint;
                var p2b = l.EndPoint;

                var p11b = p1b.Mirror(axis);
                var p22b = p2b.Mirror(axis);

                var p1 = new Point3d(p1b.X, p1b.Z, p1b.Y) + vtMove;
                var p2 = new Point3d(p2b.X, p2b.Z, p2b.Y) + vtMove;

                var p11 = new Point3d(p11b.X, p11b.Z, p11b.Y) + vtMove;
                var p22 = new Point3d(p22b.X, p22b.Z, p22b.Y) + vtMove;

                linesFace1.Add(new LineCad(_ts, _db,
                    p1.RotateBy(angle, new Vector3d(0, 0, 1), BasePointInstall),
                    p2.RotateBy(angle, new Vector3d(0, 0, 1), BasePointInstall)));

                linesFace2.Add(new LineCad(_ts, _db,
                    p11.RotateBy(angle, new Vector3d(0, 0, 1), BasePointInstall),
                    p22.RotateBy(angle, new Vector3d(0, 0, 1), BasePointInstall)));
            }
        }

        private LineCad GetAxis(Line axis)
        {
            LineCad result = null;
            try
            {
                var p1 = new Point3d(axis.StartPoint.X, axis.StartPoint.Z, axis.StartPoint.Y);
                var p2 = new Point3d(axis.EndPoint.X, axis.EndPoint.Z, axis.EndPoint.Y);
                result = new LineCad(_ts, _db, p1, p2);
            }
            catch (Exception)
            {
            }
            return result;
        }

        private Point3d GetCenterCurrent(IEnumerable<Line> lines, LineCad axis)
        {
            var result = new Point3d();
            var pointYMin = lines.GetPoints().OrderBy(x => x.Y).FirstOrDefault();
            if (pointYMin != null)
            {
                var mid = axis.MidP;
                result = new Point3d(mid.X, pointYMin.Z, mid.Y);
            }
            return result;
        }

        private List<LineCad> GetLines(IEnumerable<Line> lines, Vector3d vtMove, double angle = 0)
        {
            var result = new List<LineCad>();
            try
            {
                result = lines
                .Select(x =>
                {
                    var p1 = new Point3d(x.StartPoint.X, x.StartPoint.Z, x.StartPoint.Y) + vtMove;
                    var p2 = new Point3d(x.EndPoint.X, x.EndPoint.Z, x.EndPoint.Y) + vtMove;
                    return new LineCad(_ts, _db,
                        p1.RotateBy(angle, new Vector3d(0, 0, 1), BasePointInstall),
                        p2.RotateBy(angle, new Vector3d(0, 0, 1), BasePointInstall));
                })
                .ToList();
            }
            catch (Exception)
            {
                result = new List<LineCad>();
            }
            return result;
        }

        private List<ElectricColumnSectionPlane> GetSectionPlanes()
        {
            var result = new List<ElectricColumnSectionPlane>();
            var allLines = new List<LineCad>();
            allLines.AddRange(LinesSouth);
            allLines.AddRange(LinesEarth);
            allLines.AddRange(LinesNorth);
            allLines.AddRange(LinesWest);
            try
            {
                var dms = allLines
                    .Where(x => Math.Round(x.Dir.Distance(), 0) > 0)
                    .Where(x => x.Dir.DotProduct(Vector3d.ZAxis).IsEqual(0))
                    .GroupBy(x => x, new CompareLinesOnPlane())
                    .OrderBy(x => Math.Round(x.First().MidP.Z, 2));

                result = allLines
                    .Where(x => Math.Round(x.Dir.Distance(), 0) > 0)
                    .Where(x => x.Dir.DotProduct(Vector3d.ZAxis).IsEqual(0))
                    .GroupBy(x => x, new CompareLinesOnPlane())
                    .Where(x => x.Count() >= 2)
                    .OrderBy(x => x.First().MidP.Z)
                    .Select((x, index) => new ElectricColumnSectionPlane(index, x.ToList()))
                    .ToList();
            }
            catch (Exception)
            {
            }
            return result;
        }

    }

    public enum ElectricColumnFaceType
    {
        MainFace = 1,
        SubFace = 2
    }
}
