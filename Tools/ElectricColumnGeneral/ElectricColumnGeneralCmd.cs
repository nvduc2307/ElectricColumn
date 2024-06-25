using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumnGeneral.exceptions;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Tools.ElectricColumnGeneral.views;
using CadDev.Utils;
using CadDev.Utils.Messages;

namespace CadDev.Tools.ElectricColumnGeneral
{
    public class ElectricColumnGeneralCmd : ICadCommand
    {
        private ElectricColumnGeneralRuleInstallViewModel _ruleInstall;
        private ElectricColumnGeneralViewModel _electricColumnGeneralViewModel;
        [CommandMethod("22")]
        public void Execute()
        {
            try
            {
                var _ruleInstall = new ElectricColumnGeneralRuleInstallViewModel();
                _ruleInstall.MainView.ShowDialog();
                if (_ruleInstall.RuleInstallType != RuleInstallType.None)
                {
                    AC.GetInfomation();
                    using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
                    {
                        try
                        {
                            using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                            {
                                _electricColumnGeneralViewModel = new ElectricColumnGeneralViewModel(ts);
                            }
                            ts.Commit();
                        }
                        catch (System.Exception ex)
                        {
                            if (ex.Message != ObjectNotFoundException.MessageError) IO.ShowException(ex);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }
    }
}
