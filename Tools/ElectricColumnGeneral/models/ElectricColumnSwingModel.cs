using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Messages;
using CadDev.Utils.Points;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class ElectricColumnSwingModel
    {
        public Database _db { get; set; }

        public Transaction _ts { get; set; }

        private ElectricColumnGeneralModel _electricColumnGeneralModel;

        public IEnumerable<ElectricColumnSwing> SectionSwingRight { get; set; }

        public IEnumerable<ElectricColumnSwing> SectionSwingLeft { get; set; }

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
            SectionSwingRight = GroupSwing(swingRights, ElectricColumnSwingType.Right);
            SectionSwingLeft = GroupSwing(swingLefts, ElectricColumnSwingType.Left);
        }

        private List<ElectricColumnSwing> GroupSwing(
            List<LineCad> linesSwing, 
            ElectricColumnSwingType electricColumnSwingType)
        {
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
                                .Select(x => GetChildrents(x, linesSwingNew).Where(x => !family.Any(y => y.IsSeem(x))).ToList())
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
                                electricColumnSwingType));
                    }
                }
                catch (Exception)
                {
                }
            }
            return results;
        }

        private List<LineCad> GetChildrents(LineCad parent, List<LineCad> people)
        {
            var results = new List<LineCad>();
            foreach (var person in people)
            {
                try
                {
                    if (parent.IsContinuteLine(person) && !parent.IsSeem(person))
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
            SectionSwingLeft = new List<ElectricColumnSwing>();
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
        private ElectricColumnGeneralModel _electricColumnGeneralModel;
        public IEnumerable<LineCad> LinesSection { get; set; }
        public IEnumerable<PointCad> PointsSection { get; set; }
        public IEnumerable<PointCad> PointsSectionOnPlane { get; set; }
        public IEnumerable<PointCad> PointsSectionOnSlope { get; set; }
        public Vector3d Direction { get; set; }
        public ElectricColumnSwingType ElectricColumnSwingType { get; set; }
        public FaceCad FaceSwingUp {  get; set; } 
        public FaceCad FaceSwingDown {  get; set; } 
        public ElectricColumnSwing(ElectricColumnGeneralModel electricColumnGeneralModel,
            IEnumerable<LineCad> linesSection,
            ElectricColumnSwingType electricColumnSwingType)
        {
            _electricColumnGeneralModel = electricColumnGeneralModel;
            _ts = _electricColumnGeneralModel._ts;
            _db = _electricColumnGeneralModel._db;
            LinesSection = linesSection;
            ElectricColumnSwingType = electricColumnSwingType;
            Direction = electricColumnSwingType == ElectricColumnSwingType.Right 
                ?_electricColumnGeneralModel.VectorBaseMain
                : -_electricColumnGeneralModel.VectorBaseMain;
            PointsSection = LinesSection.GetPoints().Select(x=> new PointCad(x));
            PointsSectionOnPlane = GetPointsSectionOnPlane();
        }
        public IEnumerable<PointCad> GetPointsSectionOnSlope ()
        {
            var results = new List<PointCad>();
            foreach (var p in PointsSection) { 
                try
                {
                    if (!PointsSectionOnPlane.Any(x => x.P.IsSeem(p.P))) results.Add(p);
                }
                catch (Exception)
                {
                }
            }
            results.Add(PointsSectionOnPlane.LastOrDefault());
            return results.Where(x=>x !=null)
                .OrderBy(x=>(x.P - new Point3d()).DotProduct(Direction));
        }
        public IEnumerable<PointCad> GetPointsSectionOnPlane()
        {
            var results = new List<PointCad>();
            try
            {
                results = PointsSection
                .GroupBy(x => x.P.Z)
                .OrderBy(x => x.Count())
                .LastOrDefault()
                .OrderBy(x=>(x.P - new Point3d()).DotProduct(Direction))
                .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        private List<LineCad> LinesOnFace(List<LineCad> lines, List<FaceCad> faces, ElectricColumnFaceType electricColumnFaceType)
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
    }
    public enum ElectricColumnSwingType
    {
        Right = 1, 
        Left = 2,
    }
}
