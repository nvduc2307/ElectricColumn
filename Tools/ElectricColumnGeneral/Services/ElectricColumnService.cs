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
            //foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnModel.LinesSouth)
            //{
            //    l.Create();
            //}
            //foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnModel.LinesEarth)
            //{
            //    l.Create();
            //}
            //foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnModel.LinesNorth)
            //{
            //    l.Create();
            //}
            //foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnModel.LinesWest)
            //{
            //    l.Create();
            //}

            //foreach (var l in ElectricColumnGeneralViewModel.ElectricColumnSwingModel.SwingRights)
            //{
            //    l.Create();
            //}
            //draw line sub (line design on ui)
            //foreach (var item in ElectricColumnGeneralViewModel.ElectricColumnModel.SectionPlanes)
            //{
            //    foreach (var l in item.LinesAdd)
            //    {
            //        l.Create();
            //    }
            //}
        }
    }
}
