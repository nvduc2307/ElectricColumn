using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Geometries;

namespace CadDev.Utils.Polylines
{
    public static class Polylines
    {
        public static Polyline CreatePolyline(this Transaction ts, Database database, IEnumerable<Point2d> ps)
        {
            Polyline result = null;
            try
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a polyline with two segments (3 points)
                result = new Polyline();
                result.SetDatabaseDefaults();
                var c = 0;
                foreach (Point2d p in ps)
                {
                    result.AddVertexAt(c, p, 0, 0, 0);
                    c++;
                }
                // Add the new object to the block table record and the transaction
                acBlkTblRec.AppendEntity(result);
                ts.AddNewlyCreatedDBObject(result, true);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static List<Point3d> GetPoints(this Polyline polyline)
        {
            var results = new List<Point3d>();
            try
            {
                var Cp = polyline.NumberOfVertices;
                for (int i = 0; i < Cp; i++)
                {
                    var p = polyline.GetPoint3dAt(i);
                    results.Add(p);
                }
            }
            catch (Exception)
            {
            }
            return results;
        }

        public static Point3d GetCenter(this Polyline polyline)
        {
            var result = new Point3d();
            try
            {
                var points = polyline.GetPoints();
                var minx = points.Min(x => x.X);
                var miny = points.Min(x => x.Y);
                var minz = points.Min(x => x.Z);
                var maxx = points.Max(x => x.X);
                var maxy = points.Max(x => x.Y);
                var maxz = points.Max(x => x.Z);
                var p1 = new Point3d(minx, miny, minz);
                var p2 = new Point3d(maxx, maxy, maxz);
                result = p1.MidPoint(p2);
            }
            catch (Exception)
            {
                result = new Point3d();
            }
            return result;
        }
    }
}
