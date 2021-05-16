using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SDT.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SDT.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ApplicationViewModel appViewModel;

        public MainWindow()
        {
            InitializeComponent();

            appViewModel = new ApplicationViewModel(DialogCoordinator.Instance);
            DataContext = appViewModel;
        }

        private void ScriptButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }
    }
}
