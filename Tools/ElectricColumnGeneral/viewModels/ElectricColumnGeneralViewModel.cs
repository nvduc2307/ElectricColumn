using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Utils;
using CadDev.Utils.Compares;
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
            if(axisMainFacePic != null && axisMainFacePic is Line) axisMainFace = axisMainFacePic as Line;
            if (axisMainFace == null) throw new Exception("Axis phải là line");

            var axisSubFacePic = ts.PickObject(AC.Editor, "Pick Axis SubFace");
            if (axisSubFacePic != null && axisSubFacePic is Line) axisSubFace = axisSubFacePic as Line;
            if (axisSubFace == null) throw new Exception("Axis phải là line");

            var mainFaces = ts.SelectObjs<Line>(AC.Editor);
            var sideFaces = ts.SelectObjs<Line>(AC.Editor);

            var electricColumnGeneralModel = new ElectricColumnGeneralModel(ts, AC.Database, axisMainFace, axisSubFace, mainFaces, sideFaces);

            foreach(var l in electricColumnGeneralModel.LinesMainFace)
            {
                l.Create();
            }
            foreach (var l in electricColumnGeneralModel.LinesSubFace)
            {
                l.Create();
            }
        }
    }
}
