using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Utils.Compares;
using CadDev.Utils.Geometries;
using CadDev.Utils.CanvasUtils;
using CadDev.Utils.CanvasUtils.Utils;

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
    }
    public class LineCad
    {
        public Transaction _ts { get; set; }
        public Database _db { get; set; }
        public Point3d EndP { get; set; }
        public Point3d MidP { get; set; }
        public Point3d StartP { get; set; }
        public Vector3d Dir {  get; set; }
        public CanvasBase CanvasBase { get; set; }
        public InstanceInCanvasLine CanvasLine {  get; set; }
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
