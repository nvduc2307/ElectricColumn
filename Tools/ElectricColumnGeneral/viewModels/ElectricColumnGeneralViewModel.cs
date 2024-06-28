using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public partial class ElectricColumnGeneralViewModel : ObservableObject
    {
        public Transaction Ts { get; }
        public ElectricColumnService ElectricColumnService { get; }
        public ElectricColumnGeneralViewModel(Transaction ts, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            Ts = ts;
            ElectricColumnService = new ElectricColumnService(electricColumnGeneralModel);
            ElectricColumnService.CreateElectricColumn();
        }
    }
}
