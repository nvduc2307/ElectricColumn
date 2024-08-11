using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.CanvasUtils;
using CadDev.Utils.CanvasUtils.Utils;
using CadDev.Utils.Compares;
using CadDev.Utils.Faces;
using CadDev.Utils.Geometries;

namespace CadDev.Utils.Lines
{
    public static class Lines
    {
        public static Line CreateLine(this Transaction ts, Database database, List<Point3d> ps)
        {
            Line result = null;
            try
            {
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(database.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                for (int i = 0; i < ps.Count(); i++)
                {
                    var j = i == 0 ? ps.Count() - 1 : i - 1;
                    result = new Line(ps[j], ps[i]);
                    result.SetDatabaseDefaults();
                    acBlkTblRec.AppendEntity(result);
                    ts.AddNewlyCreatedDBObject(result, true);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static Line CreateLine(this Transaction ts, Database database, Point3d p1, Point3d p2)
        {
            Line result = null;
            if (p1.IsSeem(p2)) return result;
            try
            {
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(database.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                result = new Line(p1, p2);
                result.SetDatabaseDefaults();
                acBlkTblRec.AppendEntity(result);
                ts.AddNewlyCreatedDBObject(result, true);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static IEnumerable<Point3d> GetPoints(this Line line)
        {
            var results = new List<Point3d>();
            try
            {
                results.Add(line.StartPoint.Round());
                results.Add(line.EndPoint.Round());
            }
            catch (Exception)
            {
            }
            return results;
        }

        public static IEnumerable<Point3d> GetPoints(this IEnumerable<LineCad> lines)
        {
            var results = new List<Point3d>();
            try
            {
                foreach (var l in lines)
                {
                    results.Add(l.StartP.Round());
                    results.Add(l.EndP.Round());
                }
                results = results.Distinct(new ComparePoints()).ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }

        public static IEnumerable<Point3d> GetPoints(this IEnumerable<Line> ls)
        {
            var results = new List<Point3d>();
            var ps = new List<Point3d>();
            try
            {
                foreach (var l in ls)
                {
                    ps.AddRange(l.GetPoints());
                }
                results = ps.Distinct(new ComparePoints()).ToList();
            }
            catch (System.Exception)
            {
            }
            return results;
        }

        public static bool IsContinuteLine(this LineCad l1, LineCad l2)
        {
            var result = false;
            var dk1 = l1.StartP.IsSeem(l2.StartP) || l1.StartP.IsSeem(l2.StartP);
            var dk2 = l1.EndP.IsSeem(l2.StartP) || l1.EndP.IsSeem(l2.StartP);
            var dk3 = l1.StartP.IsSeem(l2.EndP) || l1.StartP.IsSeem(l2.EndP);
            var dk4 = l1.EndP.IsSeem(l2.EndP) || l1.EndP.IsSeem(l2.EndP);
            if (dk1 || dk2 || dk3 || dk4) result = true;
            return result;
        }

        public static bool IsContinuteLine(this LineCad l1, LineCad l2, List<Point3d> psVoid)
        {
            var result = false;
            var dk1 = l1.StartP.IsSeem(l2.StartP) && !psVoid.Any(x => x.IsSeem(l1.StartP)) || l1.StartP.IsSeem(l2.StartP) && !psVoid.Any(x => x.IsSeem(l1.StartP));
            var dk2 = l1.EndP.IsSeem(l2.StartP) && !psVoid.Any(x => x.IsSeem(l1.EndP)) || l1.EndP.IsSeem(l2.StartP) && !psVoid.Any(x => x.IsSeem(l1.EndP));
            var dk3 = l1.StartP.IsSeem(l2.EndP) && !psVoid.Any(x => x.IsSeem(l1.StartP)) || l1.StartP.IsSeem(l2.EndP) && !psVoid.Any(x => x.IsSeem(l1.StartP));
            var dk4 = l1.EndP.IsSeem(l2.EndP) && !psVoid.Any(x => x.IsSeem(l1.EndP)) || l1.EndP.IsSeem(l2.EndP) && !psVoid.Any(x => x.IsSeem(l1.EndP));
            if (dk1 || dk2 || dk3 || dk4) result = true;
            return result;
        }

        public static bool IsSeem(this LineCad l1, LineCad l2)
        {
            return l1.StartP.IsSeem(l2.StartP) && l1.EndP.IsSeem(l2.EndP);
        }

        public static List<LineCad> LinesOnFace(this List<LineCad> lines, FaceCad faceCad, Vector3d vtRay)
        {
            var results = new List<LineCad>();
            foreach (var l in lines)
            {
                try
                {
                    var p1 = l.StartP.RayPointToFace(vtRay, faceCad);
                    var p2 = l.EndP.RayPointToFace(vtRay, faceCad);
                    var lc = new LineCad(l._ts, l._db, p1, p2);
                    results.Add(lc);
                }
                catch (Exception)
                {
                }
            }
            return results;
        }
        public static List<LineCad> LinesOnFace(this List<LineCad> lines, IEnumerable<FaceCad> faceCads, Vector3d vtRay)
        {
            var results = new List<LineCad>();
            foreach (var l in lines)
            {
                foreach (var face in faceCads)
                {
                    try
                    {
                        var fmaxZ = Math.Max(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z); 
                        var fminZ = Math.Min(face.BaseLine.StartP.Z, face.BaseLine.EndP.Z);
                        if (l.MidP.Z.IsGreateOrEqual(fminZ, 0.1) && l.MidP.Z.IsLessOrEqual(fmaxZ, 0.1)) {
                            var p1 = l.StartP.RayPointToFace(vtRay, face);
                            var p2 = l.EndP.RayPointToFace(vtRay, face);
                            var lc = new LineCad(l._ts, l._db, p1, p2);
                            results.Add(lc);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return results.Distinct(new CompareLines()).ToList();
        }
    }
    public class LineCad
    {
        public Transaction _ts { get; set; }
        public Database _db { get; set; }
        public Point3d EndP { get; set; }
        public Point3d MidP { get; set; }
        public Point3d StartP { get; set; }
        public Vector3d Dir { get; set; }
        public CanvasBase CanvasBase { get; set; }
        public InstanceInCanvasLine CanvasLine { get; set; }
        public OptionStyleInstanceInCanvas OptionStyleInstanceInCanvas { get; set; }
        public LineCad(Transaction ts, Database database, Point3d p1, Point3d p2)
        {
            _ts = ts;
            _db = database;
            StartP = p1;
            EndP = p2;
            MidP = p1.MidPoint(p2);
            Dir = (p2 - p1).GetNormal();
        }

        public Line Create()
        {
            Line result = null;
            if (StartP.IsSeem(EndP)) return result;
            try
            {
                BlockTable acBlkTbl;
                acBlkTbl = _ts.GetObject(_db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = _ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                result = new Line(StartP, EndP);
                result.SetDatabaseDefaults();
                acBlkTblRec.AppendEntity(result);
                _ts.AddNewlyCreatedDBObject(result, true);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public void CreateInCanvas()
        {
            if (CanvasLine != null) CanvasLine.DrawInCanvas();
        }
    }
}
