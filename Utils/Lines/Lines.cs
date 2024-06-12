﻿using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Utils.Compares;
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
                results.Add(line.StartPoint);
                results.Add(line.EndPoint);
            }
            catch (Exception)
            {
            }
            return results;
        }
    }
    public class LineCad
    {
        private Transaction _ts;
        private Database _db;
        public Point3d StartP { get; set; }
        public Point3d EndP { get; set; }
        public Point3d MidP { get; set; }
        public LineCad(Transaction ts, Database database, Point3d p1, Point3d p2)
        {
            _ts = ts;
            _db = database;
            StartP = p1;
            EndP = p2;
            MidP = p1.MidPoint(p2);
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
    }
}
