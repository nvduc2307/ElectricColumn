using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumn.models;
using CadDev.Tools.ElectricColumn.views;
using CadDev.Utils;
using CadDev.Utils.Selections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadDev.Tools.ElectricColumn.viewModels
{
    public partial class ElectricColumnViewModel : ObservableObject
    {
        private Transaction _ts;
        private Database _db;

        public ElectricColumnView MainView { get; set; }
        public models.ElectricColumn ElectricColumn { get; set; }
        public UiElement UiElement { get; set; }

        public ElectricColumnViewModel(Transaction ts, Database db)
        {
            _ts = ts;
            _db = db;
            var linesNoEars = ts.SelectObjs<Line>(AC.Editor);
            var linesHasEars = ts.SelectObjs<Line>(AC.Editor);
            ElectricColumn = new models.ElectricColumn(ts, AC.Database, linesNoEars, linesHasEars);
            UiElement = new UiElement();
            MainView = new ElectricColumnView() { DataContext = this };
        }

        [RelayCommand]
        public void Create3D()
        {
            ElectricColumn.CreateBody();
            ElectricColumn.CreateEars(ElectricColumnEarDirectionType.DirX);
            MainView.Close();
        }
    }
}
