using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace CadDev.MVVM.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string message = "HC_TECH_SO";

        [RelayCommand]
        public void ShowMessage()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
        }
    }
}
