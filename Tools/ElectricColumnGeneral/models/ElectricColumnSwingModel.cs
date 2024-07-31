using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class ElectricColumnSwingModel : ObservableObject
    {
        public Database _db { get; set; }

        public Transaction _ts { get; set; }

        private ElectricColumnGeneralModel _electricColumnGeneralModel;

        private ElectricColumnSwing _electricColumnSwingSelected;

        public IEnumerable<ElectricColumnSwing> ElectricColumnSwingsRight { get; set; }

        public IEnumerable<ElectricColumnSwing> ElectricColumnSwingsLeft { get; set; }

        public IEnumerable<ElectricColumnSwing> ElectricColumnTotalSwingsLeft { get; set; }

        public ElectricColumnSwing ElectricColumnSwingSelected
        {
            get => _electricColumnSwingSelected;
            set
            {
                _electricColumnSwingSelected = value;
                OnPropertyChanged();
                ElectricColumnUIElementModel.UpdateStatusSwingSelectedAtElevation(_viewModel.UIElement.SectionPlaneCanvas, _viewModel);
            }
        }

        public IEnumerable<LineCad> SwingRights { get; set; }

        public IEnumerable<LineCad> SwingLefts { get; set; }

        public ElectricColumnSwingModel(ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            _electricColumnGeneralModel = electricColumnGeneralModel;
            _ts = _electricColumnGeneralModel._ts;
            _db = _electricColumnGeneralModel._db;
            GetLinesSwing(out List<LineCad> swingRights, out List<LineCad> swingLefts);
            SwingRights = swingRights;
            SwingLefts = swingLefts;
            ElectricColumnSwingsRight = GroupSwing(swingRights, ElectricColumnSwingType.Right, _electricColumnGeneralModel.FacesMainFaceRight);
            ElectricColumnSwingsLeft = GroupSwing(swingLefts, ElectricColumnSwingType.Left, _electricColumnGeneralModel.FacesMainFaceLeft);

            ElectricColumnTotalSwingsLeft = ElectricColumnSwingsRight.Concat(ElectricColumnSwingsLeft);
            var c = 1;
            foreach (var item in ElectricColumnTotalSwingsLeft)
            {
                item.Name = $"Swing{c}";
                c++;
            }
            ElectricColumnSwingSelected = ElectricColumnTotalSwingsLeft.FirstOrDefault();
        }

        private List<ElectricColumnSwing> GroupSwing(
            List<LineCad> linesSwing,
            ElectricColumnSwingType electricColumnSwingType,
            List<FaceCad> faceCads)
        {
            var pointsVoidOnFace = GetPointOnFace(linesSwing, faceCads);
            var linesSwingNew = linesSwing.Concat(new List<LineCad>()).ToList();
            var results = new List<ElectricColumnSwing>();
            linesSwingNew = linesSwingNew.OrderBy(x => x.MidP.Z).ToList();
            var c = linesSwingNew.Count();
            foreach (var l in linesSwingNew)
            {
                try
                {
                    if (!results.Any(x => x.LinesSection.Any(x => x.IsSeem(l))))
                    {
                        var family = new List<LineCad>();
                        var parents = new List<LineCad>() { l };
                        var run = 0;
                        do
                        {
                            family.AddRange(parents);
                            var childrent = parents
                                .Select(x => GetChildrents(x, linesSwingNew, pointsVoidOnFace).Where(x => !family.Any(y => y.IsSeem(x))).ToList())
                                .Where(x => x.Count > 0);
                            var childrentCount = childrent.Count();
                            if (childrentCount > 0)
                                parents = childrent
                                        .Aggregate((a, b) => a.Concat(b).ToList());
                            else
                                run = 1;

                        } while (run == 0);
                        if (family.Count > 0)
                            results.Add(new ElectricColumnSwing(
                                _electricColumnGeneralModel,
                                family.Distinct(new CompareLines()),
                                _electricColumnGeneralModel.FacesSubFaceRight,
                                _electricColumnGeneralModel.FacesSubFaceLeft,
                                electricColumnSwingType));
                    }
                }
                catch (Exception)
                {
                }
            }
            return results;
        }

        private List<Point3d> GetPointOnFace(List<LineCad> lineCads, IEnumerable<FaceCad> faces)
        {
            var points = lineCads.GetPoints();
            var results = new List<Point3d>();
            try
            {
                foreach (var p in points)
                {
                    foreach (var f in faces)
                    {
                        var fl = f.BaseLine;
                        var fMax = Math.Max(fl.StartP.Z, fl.EndP.Z);
                        var fMin = Math.Min(fl.StartP.Z, fl.EndP.Z);
                        if (p.Z.IsGreateOrEqual(fMin) && p.Z.IsLessOrEqual(fMax))
                        {
                            var pray = p.RayPointToFace(f.Normal, f);
                            if (pray != null)
                                if (pray.IsSeem(p)) results.Add(p);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return results.Distinct(new ComparePoints()).ToList();
        }

        private List<LineCad> GetChildrents(LineCad parent, List<LineCad> people, List<Point3d> psVoid)
        {
            var results = new List<LineCad>();
            foreach (var person in people)
            {
                try
                {
                    if (parent.IsContinuteLine(person, psVoid) && !parent.IsSeem(person))
                        results.Add(person);
                }
                catch (Exception)
                {
                }
            }
            return results;
        }

        private void GetLinesSwing(out List<LineCad> swingRights, out List<LineCad> swingLefts)
        {
            swingRights = new List<LineCad>();
            swingLefts = new List<LineCad>();
            //SectionSwingLeft = new List<ElectricColumnSwing>();
            var lines = _electricColumnGeneralModel.LinesMain;
            var facesRight = _electricColumnGeneralModel.FacesMainFaceRight;
            var facesLeft = _electricColumnGeneralModel.FacesMainFaceLeft;
            var vectorBase = _electricColumnGeneralModel.VectorBaseMain;
            foreach (var line in lines)
            {
                var mid = line.MidP;
                foreach (var face in facesRight)
                {
                    if (!facesRight.Any(x => x.BaseLine.IsSeem(line)))
                    {
                        var p = mid.RayPointToFace(vectorBase, face);
                        if (p != null && p.X != double.NaN)
                        {
                            var minz = Math.Round(Math.Min(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z), 2);
                            var maxz = Math.Round(Math.Max(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z), 2);
                            var dk1 = p.Z.IsGreateOrEqual(minz);
                            var dk2 = p.Z.IsLessOrEqual(maxz);
                            if (dk1 && dk2)
                            {
                                var vt = (mid - p).GetNormal();
                                if (vt.DotProduct(vectorBase).IsGreate(0)) swingRights.Add(line);
                            }
                        }
                    }
                }

                foreach (var face in facesLeft)
                {
                    if (!facesLeft.Any(x => x.BaseLine.IsSeem(line)))
                    {
                        var p = mid.RayPointToFace(vectorBase, face);
                        if (p != null && p.X != double.NaN)
                        {
                            var minz = Math.Round(Math.Min(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z), 2);
                            var maxz = Math.Round(Math.Max(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z), 2);
                            var dk1 = p.Z.IsGreateOrEqual(minz);
                            var dk2 = p.Z.IsLessOrEqual(maxz);
                            if (dk1 && dk2)
                            {
                                var vt = (mid - p).GetNormal();
                                if (vt.DotProduct(vectorBase).IsLess(0) && !vt.Distance().IsEqual(0)) swingLefts.Add(line);
                            }
                        }
                    }
                }
            }
            swingRights = swingRights
                .Where(x => !x.Dir.Distance().IsEqual(0))
                .Distinct(new CompareLines()).ToList();
            swingLefts = swingLefts
                .Where(x => !x.Dir.Distance().IsEqual(0))
                .Distinct(new CompareLines()).ToList();
        }

    }
    public class ElectricColumnSwing
    {
        /// <summary>
        /// cac line canh tren mat cat.
        /// </summary>
        private Database _db;
        private Transaction _ts;

        public string Name { get; set; }
        public Vector3d Normal { get; set; }
        public Vector3d Direction { get; set; }
        public FaceCad FaceSwingLeft { get; set; }
        public FaceCad FaceSwingRight { get; set; }
        public List<LineCad> LinesLeft { get; set; }
        public List<LineCad> LinesRight { get; set; }
        public IEnumerable<LineCad> LinesSection { get; set; }
        public IEnumerable<FaceCad> FacesSubRight { get; set; }
        public IEnumerable<FaceCad> FacesSubLeft { get; set; }
        public ElectricColumnSwingType ElectricColumnSwingType { get; set; }
        public ElectricColumnSwing(
            ElectricColumnGeneralModel electricColumnGeneralModel,
            IEnumerable<LineCad> linesSection,
            IEnumerable<FaceCad> facesSubRight,
            IEnumerable<FaceCad> facesSubLeft,
            ElectricColumnSwingType electricColumnSwingType)
        {
            _ts = linesSection.FirstOrDefault()._ts;
            _db = linesSection.FirstOrDefault()._db;
            LinesSection = linesSection;
            FacesSubRight = facesSubRight;
            FacesSubLeft = facesSubLeft;
            ElectricColumnSwingType = electricColumnSwingType;
            Direction = electricColumnSwingType == ElectricColumnSwingType.Right
                ? Vector3d.XAxis
                : -Vector3d.XAxis;
            Normal = Vector3d.YAxis;
            GetFaceSwingRightLeft(out FaceCad faceSwingRight, out FaceCad faceSwingLeft);
            FaceSwingLeft = faceSwingLeft;
            FaceSwingRight = faceSwingRight;

            LinesRight = LinesSection.ToList().LinesOnFace(faceSwingRight, Normal);
            LinesLeft = LinesSection.ToList().LinesOnFace(faceSwingLeft, Normal);
        }

        private void GetFaceSwingRightLeft(out FaceCad faceSwingRight, out FaceCad faceSwingLeft)
        {
            faceSwingRight = null;
            faceSwingLeft = null;
            try
            {
                var points = LinesSection.GetPoints();
                var pointsGrZ = points.GroupBy(x => x.Z).OrderBy(x => x.FirstOrDefault().Z);
                var pointsGrZMax = pointsGrZ.LastOrDefault();
                var pointsGrZMin = pointsGrZ.FirstOrDefault();
                var pointsGrX = points.GroupBy(x => (x - new Point3d()).DotProduct(Direction))
                    .OrderBy(x => (x.FirstOrDefault() - new Point3d()).DotProduct(Direction));
                var pointsGrMaxX = pointsGrX.LastOrDefault();
                var pointsGrMinX = pointsGrX.FirstOrDefault();

                //điểm mũi của cánh
                var pMaxX = pointsGrMaxX.FirstOrDefault();
                //điểm chân cánh phía trên
                var pMinXTop = pointsGrZMax
                    .GroupBy(x => (x - new Point3d()).DotProduct(Direction))
                    .OrderBy(x => (x.FirstOrDefault() - new Point3d()).DotProduct(Direction))
                    .FirstOrDefault()
                    .FirstOrDefault();
                //điểm chân cánh phía dưới
                var pMinXBot = pointsGrZMin
                    .GroupBy(x => (x - new Point3d()).DotProduct(Direction))
                    .OrderBy(x => (x.FirstOrDefault() - new Point3d()).DotProduct(Direction))
                    .FirstOrDefault()
                    .FirstOrDefault();

                var pMinXMid = pMinXTop.MidPoint(pMinXBot);
                var faceSubRightAction = FacesSubRight.FirstOrDefault(x =>
                {
                    var lb = x.BaseLine;
                    var p1 = lb.StartP;
                    var p2 = lb.EndP;
                    var zMax = Math.Max(p1.Z, p2.Z);
                    var zMin = Math.Min(p1.Z, p2.Z);
                    return zMin <= pMinXMid.Z && pMinXMid.Z <= zMax;
                });

                var faceSubLeftAction = FacesSubLeft.FirstOrDefault(x =>
                {
                    var lb = x.BaseLine;
                    var p1 = lb.StartP;
                    var p2 = lb.EndP;
                    var zMax = Math.Max(p1.Z, p2.Z);
                    var zMin = Math.Min(p1.Z, p2.Z);
                    return zMin <= pMinXMid.Z && pMinXMid.Z <= zMax;
                });

                var pMinXBotRight = pMinXBot.RayPointToFace(Normal, faceSubRightAction);
                var pMinXBotLeft = pMinXBot.RayPointToFace(Normal, faceSubLeftAction);

                var pMinXTopRight = pMinXTop.RayPointToFace(Normal, faceSubRightAction);
                var pMinXTopLeft = pMinXTop.RayPointToFace(Normal, faceSubLeftAction);

                var lBotRight = new LineCad(_ts, _db, pMinXBotRight, pMaxX);
                var lBotLeft = new LineCad(_ts, _db, pMinXBotLeft, pMaxX);
                var lTopRight = new LineCad(_ts, _db, pMinXTopRight, pMaxX);
                var lTopLeft = new LineCad(_ts, _db, pMinXTopLeft, pMaxX);

                faceSwingRight = new FaceCad(pMaxX, pMinXTopRight, pMinXBotRight);
                faceSwingLeft = new FaceCad(pMaxX, pMinXTopLeft, pMinXBotLeft);
            }
            catch (Exception)
            {
            }
        }

        private void IsActiveInCanvas()
        {

        }

    }
    public enum ElectricColumnSwingType
    {
        Right = 1,
        Left = 2,
    }
}
