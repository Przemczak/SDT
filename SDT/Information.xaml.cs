using System;
using System.Reflection;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows.Navigation;

namespace SDT
{
    /// <summary>
    /// Interaction logic for Information.xaml
    /// </summary>
    public partial class Information : MetroWindow
    {
        public Information()
        {
            InitializeComponent();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            InfoVer.Text = "Version:" + " " + version.Major + "." + version.Minor + "." + version.Build;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

