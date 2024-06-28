using CadDev.Tools.ElectricColumnGeneral.views;
using CadDev.Utils.Messages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CadDev.Tools.ElectricColumnGeneral.viewModels
{
    public class ElectricColumnGeneralRuleInstallViewModel : ObservableObject
    {
        private bool _isOption;


        public bool IsOption
        {
            get => _isOption;
            set
            {
                _isOption = value;
                OnPropertyChanged();
                CountWork++;
                if (CountWork > 1)
                {
                    RuleInstallType = _isOption ? RuleInstallType.Option1 : RuleInstallType.Option2;
                    MainView?.Close();
                }
            }
        }

        public int CountWork { get; set; }

        public RuleInstallType RuleInstallType { get; set; }

        public ElectricColumnGeneralRuleInstallView MainView { get; set; }

        public ElectricColumnGeneralRuleInstallViewModel()
        {
            CountWork = 0;
            MainView = new ElectricColumnGeneralRuleInstallView() { DataContext = this};
            MainView.Closed += MainView_Closed;
            IsOption = true;
            RuleInstallType = RuleInstallType.None;
        }

        private void MainView_Closed(object sender, EventArgs e)
        {
        }
    }
    public enum RuleInstallType
    {
        None = 0,
        Option1 = 1,
        Option2 = 2,
        Option3 = 2,
    }
}
