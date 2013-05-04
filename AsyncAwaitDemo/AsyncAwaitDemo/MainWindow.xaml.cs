using System.Windows;

namespace AsyncAwaitDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(
                targetState => VisualStateManager.GoToElementState(App.Current.MainWindow, targetState, false));
        }
    }
}