using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Lists;
using CadDev.Utils.Points;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

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

        public IEnumerable<ElectricColumnSwing> ElectricColumnTotalSwings { get; set; }

        public ElectricColumnSwing ElectricColumnSwingSelected
        {
            get => _electricColumnSwingSelected;
            set
            {
                _electricColumnSwingSelected = value;
                OnPropertyChanged();
                if (_electricColumnGeneralModel.UIElement != null)
                {
                    ElectricColumnUIElementModel.UpdateStatusSwingSelectedAtElevation(_electricColumnGeneralModel);
                    ElectricColumnUIElementModel.DrawSwingTop(_electricColumnGeneralModel.UIElement.SwingPlaneTopCanvas, _electricColumnGeneralModel);
                    ElectricColumnUIElementModel.DrawSwingBot(_electricColumnGeneralModel.UIElement.SwingPlaneBotCanvas, _electricColumnGeneralModel);
                }
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

            ElectricColumnTotalSwings = ElectricColumnSwingsRight.Concat(ElectricColumnSwingsLeft);
            var c = 1;
            foreach (var item in ElectricColumnTotalSwings)
            {
                item.Name = $"Swing{c}";
                c++;
            }
            ElectricColumnSwingSelected = ElectricColumnTotalSwings.FirstOrDefault();
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
    public class ElectricColumnSwing : ObservableObject
    {
        /// <summary>
        /// cac line canh tren mat cat.
        /// </summary>
        private Database _db;
        private Transaction _ts;
        private ElectricColumnGeneralModel _electricColumnGeneralModel;
        private int _typeShape;
        public int TypeShape {
            get => _typeShape;
            set
            {
                _typeShape = value;
                OnPropertyChanged();
                GenerateLinesRightLeft(_typeShape);
                GenerateLinesTopBot(_typeShape);
            }
        }
        public string Name { get; set; }
        public Vector3d Normal { get; set; }
        public Vector3d Direction { get; set; }
        public FaceCad FaceSwingLeft { get; set; }
        public FaceCad FaceSwingRight { get; set; }
        public FaceCad FaceSwingTop { get; set; }
        public FaceCad FaceSwingBot { get; set; }
        public List<LineCad> LinesLeft { get; set; }
        public List<LineCad> LinesLeftType1 { get; set; }
        public List<LineCad> LinesLeftType2 { get; set; }
        public List<LineCad> LinesRight { get; set; }
        public List<LineCad> LinesRightType1 { get; set; }
        public List<LineCad> LinesRightType2 { get; set; }
        public List<LineCad> LinesTop { get; set; }
        public List<LineCad> LinesTopType1 { get; set; }
        public List<LineCad> LinesTopType2 { get; set; }
        public List<LineCad> LinesTopAdd { get; set; }
        public List<LineCad> LinesBot { get; set; }
        public List<LineCad> LinesBotType1 { get; set; }
        public List<LineCad> LinesBotType2 { get; set; }
        public List<LineCad> LinesBotAdd { get; set; }
        public List<PointCad> PointsTop { get; set; }
        public List<PointCad> PointsTop1 { get; set; }
        public List<PointCad> PointsTop2 { get; set; }
        public List<PointCad> PointsBot { get; set; }
        public List<PointCad> PointsBot1 { get; set; }
        public List<PointCad> PointsBot2 { get; set; }
        public IEnumerable<LineCad> LinesSection { get; set; }
        public IEnumerable<FaceCad> FacesSubRight { get; set; }
        public IEnumerable<FaceCad> FacesSubLeft { get; set; }
        public IEnumerable<Point3d> Points { get; set; }
        public Point3d MaxXChop { get; set; }
        public Point3d MinXTop { get; set; }
        public Point3d MinXBot { get; set; }
        public Point3d Center { get; set; }
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
            _electricColumnGeneralModel = electricColumnGeneralModel;
            LinesTopAdd = new List<LineCad>();
            LinesBotAdd = new List<LineCad>();
            LinesSection = linesSection;
            FacesSubRight = facesSubRight;
            FacesSubLeft = facesSubLeft;
            ElectricColumnSwingType = electricColumnSwingType;
            Direction = electricColumnSwingType == ElectricColumnSwingType.Right
                ? Vector3d.XAxis
                : -Vector3d.XAxis;
            Normal = Vector3d.YAxis;

            GetPointControlOfSwing(
                out List<Point3d> points,
                out Point3d maxXChop,
                out Point3d minXTop,
                out Point3d minXBot);
            Points = points;
            MaxXChop = maxXChop;
            MinXTop = minXTop;
            MinXBot = minXBot;
            Center = GetCenter();

            GetFaceTopBot(MaxXChop, MinXTop, MinXBot,
            out FaceCad FaceTop,
            out FaceCad FaceBot);
            FaceSwingTop = FaceTop;
            FaceSwingBot = FaceBot;

            GetFaceSwingRightLeftTopBot(
                out FaceCad faceSwingRight,
                out FaceCad faceSwingLeft,
                out FaceCad faceSwingTop,
                out FaceCad faceSwingBot);
            FaceSwingLeft = faceSwingLeft;
            FaceSwingRight = faceSwingRight;

            LinesRightType1 = LinesSection.ToList().LinesOnFace(faceSwingRight, Normal);
            LinesLeftType1 = LinesSection.ToList().LinesOnFace(faceSwingLeft, Normal);
            LinesRightType2 = LinesSection.ToList().LinesOnFace(FacesSubRight, Normal);
            LinesLeftType2 = LinesSection.ToList().LinesOnFace(FacesSubLeft, Normal);
            
            LinesRight = LinesRightType1;
            LinesLeft = LinesLeftType1;

            LinesTopType1 = GetLinesSwingTopBot(FaceSwingTop, LinesRightType1, LinesLeftType1, true);
            LinesBotType1 = GetLinesSwingTopBot(FaceSwingBot, LinesRightType1, LinesLeftType1, false);
            LinesTopType2 = GetLinesSwingTopBot(FaceSwingTop, LinesRightType2, LinesLeftType2, true);
            LinesBotType2 = GetLinesSwingTopBot(FaceSwingBot, LinesRightType2, LinesLeftType2, false);

            LinesTop = LinesTopType1;
            LinesBot = LinesBotType1;

            PointsTop1 = LinesTopType1.GetPoints().Select(x => new PointCad(x)).ToList();
            PointsBot1 = LinesBotType1.GetPoints().Select(x => new PointCad(x)).ToList();
            PointsTop2 = LinesTopType2.GetPoints().Select(x => new PointCad(x)).ToList();
            PointsBot2 = LinesBotType2.GetPoints().Select(x => new PointCad(x)).ToList();

            PointsTop = PointsTop1;
            PointsBot = PointsBot1;

            TypeShape = 0;
        }

        private void GenerateLinesRightLeft(int typeShape)
        {
            switch (typeShape)
            {
                case (int)ElectricColumnSwingTypeShape.Type1:
                    LinesRight = LinesRightType1;
                    LinesLeft = LinesLeftType1;
                    break;
                case (int)ElectricColumnSwingTypeShape.Type2:
                    LinesRight = LinesRightType2;
                    LinesLeft = LinesLeftType2;
                    break;
            }
            OnPropertyChanged(nameof(LinesRight));
            OnPropertyChanged(nameof(LinesLeft));
        }
        private void GenerateLinesTopBot(int typeShape)
        {
            LinesTopAdd = new List<LineCad>();
            LinesBotAdd = new List<LineCad>();
            switch (typeShape)
            {
                case (int)ElectricColumnSwingTypeShape.Type1:
                    LinesTop = LinesTopType1;
                    LinesBot = LinesBotType1;
                    PointsTop = PointsTop1;
                    PointsBot = PointsBot1;
                    break;
                case (int)ElectricColumnSwingTypeShape.Type2:
                    LinesTop = LinesTopType2;
                    LinesBot = LinesBotType2;
                    PointsTop = PointsTop2;
                    PointsBot = PointsBot2;
                    break;
            }
            if (_electricColumnGeneralModel.ElectricColumnSwingModel != null)
            {
                ElectricColumnUIElementModel.UpdateStatusSwingSelectedAtElevation(_electricColumnGeneralModel);
                ElectricColumnUIElementModel.DrawSwingTop(_electricColumnGeneralModel.UIElement.SwingPlaneTopCanvas, _electricColumnGeneralModel);
                ElectricColumnUIElementModel.DrawSwingBot(_electricColumnGeneralModel.UIElement.SwingPlaneBotCanvas,_electricColumnGeneralModel);
            }
        }

        private Point3d GetCenter()
        {
            var result = new Point3d();
            try
            {
                var ps = Points.Select(x => new Point3d(x.X, x.Y, 0)).ToList();
                var psX = ps
                    .GroupBy(x => (x - new Point3d()).DotProduct(Direction))
                    .OrderBy(x => (x.FirstOrDefault() - new Point3d()).DotProduct(Direction))
                    .ToList();
                var psY = ps
                    .GroupBy(x => (x - new Point3d()).DotProduct(Normal))
                    .OrderBy(x => (x.FirstOrDefault() - new Point3d()).DotProduct(Normal))
                    .ToList();

                var pXmin = psX.FirstOrDefault().FirstOrDefault();
                var pXmax = psX.FirstOrDefault().LastOrDefault();
                var pYmin = psY.FirstOrDefault().LastOrDefault();
                var pYmax = psY.FirstOrDefault().LastOrDefault();

                result = pXmax.MidPoint(pYmin);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public List<LineCad> GetLinesSwingTopBot(FaceCad faceCad, List<LineCad> linesRight, List<LineCad> linesLeft, bool isTopFace = true)
        {
            var results = new List<LineCad>();
            var linesSwingTotal = linesRight.Concat(linesLeft).ToList();
            foreach (var line in linesSwingTotal)
            {
                try
                {
                    var pSt = line.StartP;
                    var pEn = line.EndP;

                    var pStRay = pSt.RayPointToFace(Vector3d.ZAxis, faceCad);
                    var pEnRay = pEn.RayPointToFace(Vector3d.ZAxis, faceCad);

                    if (pStRay != null && pEnRay != null)
                    {
                        var vt1 = (pSt - pStRay).GetNormal();
                        var vt2 = (pEn - pEnRay).GetNormal();
                        if (isTopFace)
                        {
                            if (pSt.Z.IsGreateOrEqual(pStRay.Z) && pEn.Z.IsGreateOrEqual(pEnRay.Z)) results.Add(line);
                        }
                        else
                        {
                            if (pSt.Z.IsLessOrEqual(pStRay.Z) && pEn.Z.IsLessOrEqual(pEnRay.Z)) results.Add(line);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return results.Distinct(new CompareLines()).ToList();
        }

        private void GetFaceTopBot(
            Point3d MaxXChop,
            Point3d MinXTop,
            Point3d MinXBot,
            out FaceCad FaceTop,
            out FaceCad FaceBot)
        {
            FaceTop = null;
            FaceBot = null;
            try
            {
                var vtTop = (MaxXChop - MinXTop).GetNormal();
                var vtBot = (MaxXChop - MinXBot).GetNormal();
                FaceTop = new FaceCad(vtTop.CrossProduct(Normal).DotProduct(Vector3d.ZAxis).IsGreate(0)
                    ? vtTop.CrossProduct(Normal)
                    : -vtTop.CrossProduct(Normal), MinXTop);
                FaceBot = new FaceCad(vtBot.CrossProduct(Normal).DotProduct(Vector3d.ZAxis).IsGreate(0)
                    ? -vtBot.CrossProduct(Normal)
                    : vtBot.CrossProduct(Normal), MinXBot);
                // tạo mặt phẳng phía trên, phía dưới
                //group các linecad nằm phía trên hoặc bên trên bề mặt mặt phẳng phía trên
                //group các linecad nằm phía dưới hoặc bên trên bề mặt mặt phẳng phía dưới
                //==> các linecad mặt trên và mặt dưới cần tìm.
            }
            catch (Exception)
            {
            }
        }

        private void GetPointControlOfSwing(
            out List<Point3d> points,
            out Point3d MaxXChop,
            out Point3d MinXTop,
            out Point3d MinXBot)
        {
            points = new List<Point3d>();
            MaxXChop = new Point3d();
            MinXTop = new Point3d();
            MinXBot = new Point3d();
            try
            {
                points = LinesSection.GetPoints().ToList();
                var pointsGrZ = points.GroupBy(x => x.Z)
                    .Select(x => x.ToList())
                    .OrderBy(x => x.FirstOrDefault().Z)
                    .ToList();
                var pointsGrZMax = pointsGrZ.LastOrDefault();
                var pointsGrZMin = pointsGrZ.FirstOrDefault();
                var pointsGrX = points.GroupBy(x => (x - new Point3d()).DotProduct(Direction))
                    .OrderBy(x => (x.FirstOrDefault() - new Point3d()).DotProduct(Direction));
                var pointsGrMaxX = pointsGrX.LastOrDefault();
                var pointsGrMinX = pointsGrX.FirstOrDefault();

                //điểm mũi của cánh
                MaxXChop = pointsGrMaxX.FirstOrDefault();
                //điểm chân cánh phía trên
                MinXTop = pointsGrZMax
                    .GroupBy(x => (x - new Point3d()).DotProduct(Direction))
                    .OrderBy(x => (x.FirstOrDefault() - new Point3d()).DotProduct(Direction))
                    .FirstOrDefault()
                    .FirstOrDefault();
                //điểm chân cánh phía dưới
                MinXBot = pointsGrZMin
                    .GroupBy(x => (x - new Point3d()).DotProduct(Direction))
                    .OrderBy(x => (x.FirstOrDefault() - new Point3d()).DotProduct(Direction))
                    .FirstOrDefault()
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                MaxXChop = new Point3d();
                MinXTop = new Point3d();
                MinXBot = new Point3d();
            }
        }

        private void GetFaceSwingRightLeftTopBot(
            out FaceCad faceSwingRight,
            out FaceCad faceSwingLeft,
            out FaceCad faceSwingTop,
            out FaceCad faceSwingBot)
        {
            faceSwingRight = null;
            faceSwingLeft = null;
            faceSwingTop = null;
            faceSwingBot = null;
            try
            {
                var pMinXMid = MinXTop.MidPoint(MinXBot);
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

                var pMinXBotRight = MinXBot.RayPointToFace(Normal, faceSubRightAction);
                var pMinXBotLeft = MinXBot.RayPointToFace(Normal, faceSubLeftAction);

                var pMinXTopRight = MinXTop.RayPointToFace(Normal, faceSubRightAction);
                var pMinXTopLeft = MinXTop.RayPointToFace(Normal, faceSubLeftAction);

                var lBotRight = new LineCad(_ts, _db, pMinXBotRight, MaxXChop);
                var lBotLeft = new LineCad(_ts, _db, pMinXBotLeft, MaxXChop);
                var lTopRight = new LineCad(_ts, _db, pMinXTopRight, MaxXChop);
                var lTopLeft = new LineCad(_ts, _db, pMinXTopLeft, MaxXChop);

                faceSwingRight = new FaceCad(MaxXChop, pMinXTopRight, pMinXBotRight);
                faceSwingLeft = new FaceCad(MaxXChop, pMinXTopLeft, pMinXBotLeft);
                faceSwingTop = new FaceCad(MaxXChop, pMinXTopLeft, pMinXBotLeft);
                faceSwingBot = new FaceCad(MaxXChop, pMinXTopLeft, pMinXBotLeft);
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
    public enum ElectricColumnSwingTypeShape
    {
        Type1 = 0,
        Type2 = 1,
    }
}
