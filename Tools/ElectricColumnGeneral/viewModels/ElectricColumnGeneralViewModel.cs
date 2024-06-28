using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Utils;
using CadDev.Utils.Selections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public partial class ElectricColumnGeneralViewModel : ObservableObject
    {
        public Transaction Ts { get; }
        public ElectricColumnGeneralViewModel(Transaction ts)
        {
            Ts = ts;
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

            foreach (var l in electricColumnGeneralModel.LinesSouth)
            {
                l.Create();
            }
            foreach (var l in electricColumnGeneralModel.LinesEarth)
            {
                l.Create();
            }
            foreach (var l in electricColumnGeneralModel.LinesNorth)
            {
                l.Create();
            }
            foreach (var l in electricColumnGeneralModel.LinesWest)
            {
                l.Create();
            }
        }
    }
}
