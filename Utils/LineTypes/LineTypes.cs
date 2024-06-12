using Autodesk.AutoCAD.DatabaseServices;

namespace CadDev.Utils.LineTypes
{
    public static class LineTypes
    {
        public static IEnumerable<LinetypeTableRecord> GetLineTypes(this Transaction ts, Database database)
        {
            var lineTypes = new List<LinetypeTableRecord>();
            try
            {
                var linetypeTable = ts.GetObject(database.LinetypeTableId,
                                                    OpenMode.ForRead) as LinetypeTable;
                foreach (var item in linetypeTable)
                {
                    lineTypes.Add(ts.GetObject(item, OpenMode.ForRead) as LinetypeTableRecord);
                }
            }
            catch (Exception)
            {
            }
            return lineTypes;
        }
    }
}
