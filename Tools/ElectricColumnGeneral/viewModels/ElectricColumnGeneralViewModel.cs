using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.services;
using CadDev.Tools.ElectricColumnGeneral.views;
using CadDev.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public partial class ElectricColumnGeneralViewModel : ObservableObject
    {
        public Transaction Ts { get; }
        public ElectricColumnService ElectricColumnService { get; }
        public ElectricColumnModel ElectricColumnModel { get; set; }
        public ElectricColumnUIElementModel UIElement { get; set; }
        public ElectricColumnGeneralModel ElectricColumnGeneralModel { get; set; }
        public ElectricColumnSwingModel ElectricColumnSwingModel { get; set; }
        public ElectricColumnGeneralViewModel(Transaction ts, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            Ts = ts;
            ElectricColumnGeneralModel = electricColumnGeneralModel;
            ElectricColumnModel = new ElectricColumnModel(ts, AC.Database, this);
            UIElement = new ElectricColumnUIElementModel(this);
            ElectricColumnService = new ElectricColumnService(this);
            ElectricColumnSwingModel = new ElectricColumnSwingModel(this);
            //ElectricColumnService.CreateElectricColumn();
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
    }
}
