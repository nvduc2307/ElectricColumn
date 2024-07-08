using Autodesk.AutoCAD.DatabaseServices;
using CadDev.MVVM.View;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.services;
using CadDev.Tools.ElectricColumnGeneral.views;
using CadDev.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public partial class ElectricColumnGeneralViewModel : ObservableObject
    {
        public Transaction Ts { get; }
        public ElectricColumnGeneralView MainView { get; set; }
        public ElectricColumnService ElectricColumnService { get; }
        public ElectricColumnModel ElectricColumnModel { get; set; }
        public ElectricColumnUIElementModel UIElement { get; set; }
        public ElectricColumnGeneralModel ElectricColumnGeneralModel {  get; set; }
        public ElectricColumnGeneralViewModel(Transaction ts, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            Ts = ts;
            ElectricColumnGeneralModel = electricColumnGeneralModel;
            ElectricColumnService = new ElectricColumnService(electricColumnGeneralModel);
            ElectricColumnModel = new ElectricColumnModel(ts, AC.Database, this);
            UIElement = new ElectricColumnUIElementModel(this);
            //ElectricColumnService.CreateElectricColumn();
        }
    }
}
