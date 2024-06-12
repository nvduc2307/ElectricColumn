using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadDev.Utils.Colors
{
    public static class Colors
    {
        //public static List<LayerTableRecord> GetColors(this Transaction ts, Database database)
        //{
        //    var results = new List<LayerTableRecord>();
        //    try
        //    {
        //        var acLyrTbl = ts.GetObject(database.ColorDictionaryId, OpenMode.ForRead) as ColorCollection;

        //        foreach (var item in acLyrTbl)
        //        {
        //            var layer = ts.GetObject(item, OpenMode.ForRead) as LayerTableRecord;
        //            results.Add(layer);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return results;
        //}
    }
}
