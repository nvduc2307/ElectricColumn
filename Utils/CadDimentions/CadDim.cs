using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CadDev.Utils.CadDimentions
{
    public class CadDim
    {
    }
    public static class CadDimExt
    {
        public static AlignedDimension CreateDim(
            this Transaction ts, Database db,
            Point3d line1Point, Point3d line2Point,
            Point3d dimensionLinePoint,
            string dimensionText = "")
        {
            AlignedDimension result = null;
            try
            {
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                result = new AlignedDimension(line1Point, line2Point, dimensionLinePoint, dimensionText, db.Dimstyle);
                result.SetDatabaseDefaults();
                //result.TransformBy(ed.CurrentUserCoordinateSystem);
                acBlkTblRec.AppendEntity(result);
                ts.AddNewlyCreatedDBObject(result, true);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static void OverWriteTextDim(this Transaction ts, Database db, Dimension dim, string textOverWrite = " ")
        {
            try
            {
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                dim.DimensionText = textOverWrite;
                acBlkTblRec.AppendEntity(dim);
                ts.AddNewlyCreatedDBObject(dim, true);
            }
            catch (Exception)
            {
            }
        }
    }
}
