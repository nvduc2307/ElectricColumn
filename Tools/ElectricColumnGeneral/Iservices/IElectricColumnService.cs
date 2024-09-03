using Autodesk.AutoCAD.DatabaseServices;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.viewModels;

namespace CadDev.Tools.ElectricColumnGeneral.iservices
{
    public interface IElectricColumnService
    {
        public ElectricColumnGeneralViewModel ElectricColumnGeneralViewModel { get; set; }
        public void CreateElectricColumn3D(Transaction ts, Database db);
        public void CreateElectricColumnPlan(Transaction ts, Database db);
    }
}
