using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.CustomAttribute;
using CadDev.Extension.ICommand;
using CadDev.Utils;
using CadDev.Utils.Selections;

namespace CadDev.Tools.ElectricColumn
{
    public class ElectricColumnCmd : ICadCommand
    {
        private models.ElectricColumn _electricColumn;

        [CommandMethod("11")]
        [RibbonButton("NVD", "My Tools", "Electric Column", "Create 3d electric column")]
        public void Execute()
        {
            AC.GetInfomation();
            using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                    {
                        var linesNoEars = ts.SelectObjs<Line>(AC.Editor);
                        var linesHasEars = ts.SelectObjs<Line>(AC.Editor);
                        _electricColumn = new models.ElectricColumn(ts, AC.Database, linesNoEars, linesHasEars);
                        _electricColumn.CreateBody();
                        _electricColumn.CreateEars(models.ElectricColumnEarDirectionType.DirX);
                    }
                    ts.Commit();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
