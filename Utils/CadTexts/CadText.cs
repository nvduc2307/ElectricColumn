using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CadDev.Utils.CadTexts
{
    public class CadText
    {
    }
    public static class CadTextExt
    {
        private static double _textHeight = 2.5;
        public static DBText CreateText(this Transaction ts, Database db, Point3d position = new Point3d(), string content = "content")
        {
            DBText result = null;
            try
            {
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                result = new DBText();
                result.TextString = content;
                result.Position = position;
                //result.Height = _textHeight * db.Cannoscale.Scale;
                result.SetDatabaseDefaults();
                acBlkTblRec.AppendEntity(result);
                ts.AddNewlyCreatedDBObject(result, true);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static void EditHeightText(this Transaction ts, Database db, DBText dBText, double heightText = 2.5)
        {
            try
            {
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                dBText.Height = heightText / db.Cannoscale.Scale;
                dBText.SetDatabaseDefaults();
                acBlkTblRec.AppendEntity(dBText);
                ts.AddNewlyCreatedDBObject(dBText, true);
            }
            catch (Exception)
            {
            }
        }
        public static void EditText(this Transaction ts, Database db, DBText dBText, string contentChange = null)
        {
            try
            {
                contentChange = contentChange == null ? dBText.TextString : contentChange;
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                dBText.TextString = contentChange;
                dBText.SetDatabaseDefaults();
                acBlkTblRec.AppendEntity(dBText);
                ts.AddNewlyCreatedDBObject(dBText, true);
            }
            catch (Exception)
            {
            }
        }
    }
}
