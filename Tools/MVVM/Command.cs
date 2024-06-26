using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Utils;
using CadDev.Utils.Geometries;
using CadDev.Utils.Lines;
using CadDev.Utils.Selections;

namespace CadDev.Tools.MVVM
{
    public class Command : ICadCommand
    {
        [CommandMethod("abc")]
        public void Execute()
        {
            AC.GetInfomation();
            using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
            {
                using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                {
                    Line l = null;
                    Line axis = null;
                    var linef = ts.PickObject(AC.Editor, "Pick Line");
                    if (linef != null && linef is Line) l = linef as Line;

                    var axisf = ts.PickObject(AC.Editor, "Pick Axis");
                    if (axisf != null && axisf is Line) axis = axisf as Line;

                    var p1 = l.StartPoint;
                    var p2 = l.EndPoint;

                    var p11 = p1.Mirror(axis);
                    var p22 = p2.Mirror(axis);

                    var ll1 = new LineCad(ts, AC.Database, p11, p22);
                    ll1.Create();
                }
                ts.Commit();
            }
        }
    }
}