using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CadDev.Extension.ICommand;
using CadDev.Tools.ElectricColumnGeneral.exceptions;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.viewModels;
using CadDev.Tools.ElectricColumnGeneral.views;
using CadDev.Utils;
using CadDev.Utils.Messages;
using CadDev.Utils.Selections;

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
                switch (_ruleInstall.RuleInstallType)
                {
                    case RuleInstallType.Option1:
                        AC.GetInfomation();
                        using (Transaction ts = AC.Database.TransactionManager.StartTransaction())
                        {
                            try
                            {
                                using (DocumentLock documentLock = AC.DocumentCollection.MdiActiveDocument.LockDocument())
                                {
                                    Line axisMainFace = null;
                                    Line axisSubFace = null;
                                    var axisMainFacePic = ts.PickObject(AC.Editor, "Pick Axis MainFace");
                                    if (axisMainFacePic != null && axisMainFacePic is Line) axisMainFace = axisMainFacePic as Line;

                                    var axisSubFacePic = ts.PickObject(AC.Editor, "Pick Axis SubFace");
                                    if (axisSubFacePic != null && axisSubFacePic is Line) axisSubFace = axisSubFacePic as Line;

                                    var linesMain = ts.SelectObjs<Line>(AC.Editor);
                                    var linesFaceMainPerSide = ts.SelectObjs<Line>(AC.Editor);
                                    var linesSub = ts.SelectObjs<Line>(AC.Editor);
                                    var linesFaceSubPerSide = ts.SelectObjs<Line>(AC.Editor);

                                    var electricColumnGeneralModel = new ElectricColumnGeneralModel(
                                        ts, AC.Database, axisMainFace, axisSubFace,
                                        linesMain, linesFaceMainPerSide,
                                        linesSub, linesFaceSubPerSide);
                                    _electricColumnGeneralViewModel = new ElectricColumnGeneralViewModel(ts, electricColumnGeneralModel);
                                }
                                ts.Commit();
                            }
                            catch (System.Exception ex)
                            {
                                if (ex.Message != ObjectNotFoundException.MessageError) IO.ShowException(ex);
                            }
                        }
                        break;
                    case RuleInstallType.Option2:
                        break;
                    case RuleInstallType.Option3:
                        break;
                }
            }
            catch (System.Exception)
            {
            }
        }
    }
}
