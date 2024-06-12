using CadDev.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace CadDev.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(MainViewModel mainViewModel)
        {
            InitializeComponent();
            InitializeWindow();
            DataContext = mainViewModel;
        }
        private void InitializeWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            new System.Windows.Interop.WindowInteropHelper(this) { Owner = Autodesk.Windows.ComponentManager.ApplicationWindow };
            this.PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
        }
    }
}
