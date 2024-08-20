using Autodesk.AutoCAD.DatabaseServices;

namespace CadDev.Utils.CadBlocks
{
    public class CadBlock
    {
    }
    public static class CadBlockExt
    {
        public static void EditBlock(this Transaction ts, Database db, BlockReference blockRef, string tagAttribute = "tagAttribute", string contentChange = "Phu")
        {
            try
            {
                BlockTable acBlkTbl;
                acBlkTbl = ts.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var properties = blockRef.AttributeCollection;
                AttributeReference attributeReferenceTarget = null;
                foreach (ObjectId id in properties)
                {
                    var attRef = (AttributeReference)ts.GetObject(id, OpenMode.ForWrite);
                    if (attRef.Tag == tagAttribute)
                    {
                        attributeReferenceTarget = attRef;
                        break;
                    }
                }
                if (attributeReferenceTarget != null)
                {
                    attributeReferenceTarget.TextString = contentChange;
                }
                blockRef.SetDatabaseDefaults();
                acBlkTblRec.AppendEntity(blockRef);
                ts.AddNewlyCreatedDBObject(blockRef, true);
            }
            catch (Exception)
            {
            }
        }
    }
}
