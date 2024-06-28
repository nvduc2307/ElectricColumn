using CadDev.Tools.ElectricColumnGeneral.models;

namespace CadDev.Tools.ElectricColumnGeneral.iservices
{
    public interface IElectricColumnService
    {
        public ElectricColumnGeneralModel ElectricColumnGeneralModel { get; set; }
        public void CreateElectricColumn();
    }
}
