using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadDev.Utils.CadTexts
{
    public class CadText
    {
    }
    public static class CadTextExt
    {
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
                result.SetDatabaseDefaults();
                acBlkTblRec.AppendEntity(result);
                ts.AddNewlyCreatedDBObject(result, true);
            }
            catch (Exception)
            {
            }
            return result;
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
