using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public partial class ElectricColumnGeneralViewModel : ObservableObject
    {
        public Transaction Ts { get; }
        public ElectricColumnService ElectricColumnService { get; }
        public ElectricColumnUIElementModel UIElement { get; set; }
        public ElectricColumnGeneralModel ElectricColumnGeneralModel { get; set; }
        public ElectricColumnGeneralViewModel(Transaction ts, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            Ts = ts;
            ElectricColumnGeneralModel = electricColumnGeneralModel;
            UIElement = new ElectricColumnUIElementModel(this);
            ElectricColumnGeneralModel.UIElement = UIElement;
            ElectricColumnService = new ElectricColumnService(this);
        }

        [RelayCommand]
        public void Create3D()
        {
            ElectricColumnService?.CreateElectricColumn();
            UIElement.MainView.Close();
        }
        [RelayCommand]
        public void Cancel()
        {
            UIElement.MainView.Close();
        }
        [RelayCommand]
        public void CreateDrawing()
        {
            var saveDialog = new SaveFileDialog();
            var dialogResult = saveDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                var path = saveDialog.FileName;
                //CadExt.CreateNewFileCad();
            }
        }
    }
}
