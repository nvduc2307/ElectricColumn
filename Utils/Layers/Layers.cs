using Autodesk.AutoCAD.DatabaseServices;

namespace CadDev.Utils.Layers
{
    public static class Layers
    {
        public static List<LayerTableRecord> GetLayers(this Transaction ts, Database database)
        {
            var results = new List<LayerTableRecord>();
            try
            {
                var acLyrTbl = ts.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;

                foreach (var item in acLyrTbl)
                {
                    var layer = ts.GetObject(item, OpenMode.ForRead) as LayerTableRecord;
                    results.Add(layer);
                }
            }
            catch (Exception)
            {
            }
            return results;
        }
    }
}
