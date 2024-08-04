using CadDev.Tools.ElectricColumnGeneral.iservices;
using CadDev.Tools.ElectricColumnGeneral.viewModels;

namespace CadDev.Tools.ElectricColumnGeneral.services
{
    public class ElectricColumnService : IElectricColumnService
    {
        public ElectricColumnGeneralViewModel ElectricColumnGeneralViewModel { get; set; }

        public ElectricColumnService(ElectricColumnGeneralViewModel electricColumnGeneralViewModel)
        {
            ElectricColumnGeneralViewModel = electricColumnGeneralViewModel;
        }

        public void CreateElectricColumn()
        {
            //draw main line
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesSouth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesEarth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesNorth)
            {
                l.Create();
            }
            foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.LinesWest)
            {
                l.Create();
            }

            foreach (var electricColumnSwingsLeft in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingsLeft)
            {
                foreach (var l in electricColumnSwingsLeft.LinesRight)
                {
                    l.Create();
                }
                foreach (var l in electricColumnSwingsLeft.LinesLeft)
                {
                    l.Create();
                }
            }

            foreach (var electricColumnSwingsRight in ElectricColumnGeneralViewModel.ElectricColumnGeneralModel.ElectricColumnSwingModel.ElectricColumnSwingsRight)
            {
                foreach (var l in electricColumnSwingsRight.LinesLeft)
                {
                    l.Create();
                }
                foreach (var l in electricColumnSwingsRight.LinesRight)
                {
                    l.Create();
                }
            }
        }
    }
}
