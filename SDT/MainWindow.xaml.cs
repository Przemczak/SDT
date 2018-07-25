using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Windows;


namespace SDT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Button_Info_Click(object sender, RoutedEventArgs e)
        {
            Information sh = new Information();
            sh.Owner = this;
            sh.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            sh.ShowDialog();
        }

        private void Button_UserCheck_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_UserLoginIn.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj login użytkownika.");
                return;
            }

            User us = new User(this);
            us.CheckAD(TextBox_UserLoginIn);
        }
    }
}
