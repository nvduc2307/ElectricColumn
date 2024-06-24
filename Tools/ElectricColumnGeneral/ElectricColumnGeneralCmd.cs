using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Tools.ElectricColumnGeneral.views;
using CadDev.Utils;

namespace CadDev.Tools.ElectricColumnGeneral
{
    public class ElectricColumnGeneralCmd : ICadCommand
    {
        private ElectricColumnGeneralViewModel _electricColumnGeneralViewModel;
        [CommandMethod("22")]
        public void Execute()
        {
            try
            {
                var v = new ElectricColumnGeneralRuleInstallView();
                v.ShowDialog();
                AC.GetInfomation();
                using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                        {
                            //_electricColumnGeneralViewModel = new ElectricColumnGeneralViewModel(ts);
                        }
                        ts.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }
    }
}
