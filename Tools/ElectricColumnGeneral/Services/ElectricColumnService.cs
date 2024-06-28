using CadDev.Tools.ElectricColumnGeneral.iservices;
using CadDev.Tools.ElectricColumnGeneral.models;

namespace CadDev.Tools.ElectricColumnGeneral.services
{
    public class ElectricColumnService : IElectricColumnService
    {
        public ElectricColumnGeneralModel ElectricColumnGeneralModel { get; set; }

        public ElectricColumnService(ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            ElectricColumnGeneralModel = electricColumnGeneralModel;
        }

        public void CreateElectricColumn()
        {
            foreach (var l in ElectricColumnGeneralModel.LinesSouth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralModel.LinesEarth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralModel.LinesNorth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralModel.LinesWest)
            {
                l.Create();
            }
        }
    }
}
