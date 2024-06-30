using Autodesk.AutoCAD.DatabaseServices;
using CadDev.MVVM.View;
using CadDev.Tools.ElectricColumnGeneral.models;
using CadDev.Tools.ElectricColumnGeneral.services;
using CadDev.Tools.ElectricColumnGeneral.views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public partial class ElectricColumnGeneralViewModel : ObservableObject
    {
        public Transaction Ts { get; }
        public ElectricColumnGeneralView MainView { get; set; }
        public ElectricColumnService ElectricColumnService { get; }
        public ElectricColumnGeneralViewModel(Transaction ts, ElectricColumnGeneralModel electricColumnGeneralModel)
        {
            Ts = ts;
            ElectricColumnService = new ElectricColumnService(electricColumnGeneralModel);
            MainView = new ElectricColumnGeneralView() { DataContext = this};
            MainView.Loaded += MainView_Loaded;
            MainView.ShowDialog();
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }
    }
}
