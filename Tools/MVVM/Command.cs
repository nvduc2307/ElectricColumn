using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Utils;
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
                    var lines = ts.SelectObjs<Line>(AC.Editor);
                    MessageBox.Show(lines.Count().ToString());
                }
                ts.Commit();
            }
        }
    }
}