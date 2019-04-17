using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace SDT.Pages
{
    /// <summary>
    /// Interaction logic for Installer.xaml
    /// </summary>
    public partial class Installer : Window
    {
        private readonly Services.PC _pc;
        private readonly MainWindow _mainWindow;

        public Installer(MainWindow MainWindow)
        {
            InitializeComponent();

            List<KeyValuePair<string, string>> servers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Lokalny folder C", @"C:\"),
                new KeyValuePair<string, string>("OPSCCM2012BY1", @"\\OPSCCM2012BY1\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012GD1", @"\\OPSCCM2012GD1\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012KA1", @"\\OPSCCM2012KA1\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012KR1", @"\\OPSCCM2012KR1\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012OL1", @"\\OPSCCM2012OL1\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012PO1", @"\\OPSCCM2012PO1\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012WA1", @"\\OPSCCM2012WA1\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012WA2", @"\\OPSCCM2012WA2\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012WR1", @"\\OPSCCM2012WR1\tpbin"),
                new KeyValuePair<string, string>("OPSCCM2012SZ1", @"\\OPSCCM2012SZ1\tpbin"),
            };

            serversListComboBox.ItemsSource = servers;
            serversListComboBox.DisplayMemberPath = "Key";
            serversListComboBox.SelectedValue = "Value";

            _mainWindow = MainWindow;
            _pc = new Services.PC(MainWindow);

            packPathGroupBox.Header = "Instalator dla: " + _mainWindow.pcTextBox.Text;
        }

        private void MinimizeInstallerButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseInstallerButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void PopupBox_OnOpened(object sender, RoutedEventArgs e) { }
        private void PopupBox_OnClosed(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// Installer - Copy pack and install 
        /// </summary>
        private async void InstallerInstallButton_Click(object sender, RoutedEventArgs e)
        {
            await CopyDirec();

            if (!string.IsNullOrWhiteSpace(packPathTextBlock.Text))
            {
                var pingcheck = await _pc.PingInstaller();
                if (pingcheck)
                {
                    try
                    {
                        var path0 = packPathTextBlock.Text.Substring(0, packPathTextBlock.Text.LastIndexOf("\\"));
                        string path1 = packPathTextBlock.Text.Substring(path0.LastIndexOf("\\"));
                        string path2 = @" C:\TEMP" + path1;

                        Process process = new Process();
                        process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                        process.StartInfo.Arguments = String.Format(@"/k ""C:\My Program Files\PsExec64.exe"" \\{0} {1}", _mainWindow.pcTextBox.Text, path2);
                        process.EnableRaisingEvents = true;
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        installerPopupText.Text = ex.Message;
                        installerPopupBox.IsPopupOpen = true;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Installer - Copy pack
        /// </summary>
        private async void InstallerCopyButton_Click(object sender, RoutedEventArgs e)
        {
            var pingcheck = await _pc.PingInstaller();
            if (pingcheck)
            {
                await CopyDirec();
            }
            else
            {
                installerPopupText.Text = "Stacja nie odpowiada w sieci.";
                installerPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Installer - Open TEMP folder
        /// </summary>
        private async void InstallerOpenTemp_Click(object sender, RoutedEventArgs e)
        {
            var pingcheck = await _pc.PingInstaller();
            if (pingcheck)
            {
                try
                {
                    Process.Start(@"\\" + _mainWindow.pcTextBox.Text + @"\c$\TEMP");
                }
                catch (Exception ex)
                {
                    installerPopupText.Text = ex.Message;
                    installerPopupBox.IsPopupOpen = true;
                    return;
                }
            }
            else
            {
                installerPopupText.Text = "Stacja nie odpowiada w sieci.";
                installerPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Installer - Clear TEMP folder
        /// </summary>
        private async void InstallerClearTemp_Click(object sender, RoutedEventArgs e)
        {
            var pingcheck = await _pc.PingInstaller();
            if (pingcheck)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(@"\\" + _mainWindow.pcTextBox.Text + @"\c$\TEMP");

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
                catch (Exception ex)
                {
                    installerPopupText.Text = ex.Message;
                    installerPopupBox.IsPopupOpen = true;
                    return;
                }
            }
            else
            {
                installerPopupText.Text = "Stacja nie odpowiada w sieci.";
                installerPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Installer - Selection of pack
        /// </summary>
        private void ServerSelectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedserver = serversListComboBox.SelectedItem as KeyValuePair<string, string>?;
                if (selectedserver.HasValue)
                {
                    var selserver = selectedserver.Value.ToString();
                    var selserver2 = selserver.Split(',', ']')[1];
                    selserver2 = string.Join("", selserver2.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    OpenFileDialog _opendialog = new OpenFileDialog
                    {
                        Title = "Wybierz plik instalacyjny",
                        Filter = "Pliki CMD (*.cmd)|*.cmd|Pliki BAT (*.bat)|*.bat|Wszystkie pliki (*.)|*.",
                        InitialDirectory = selserver2
                    };
                    _opendialog.ShowDialog();

                    string packfullpath = _opendialog.FileName;
                    packPathTextBlock.Text = packfullpath;
                }
            }
            catch (Exception er)
            {
                installerPopupText.Text = er.Message;
                installerPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Copy to TEMP folder
        /// </summary>
        private async Task CopyDirec()
        {
            if (string.IsNullOrWhiteSpace(packPathTextBlock.Text))
            {
                installerPopupText.Text = "Nie wybrano pliku instalacyjnego";
                installerPopupBox.IsPopupOpen = true;
                return;
            }
            else
            {
                try
                {
                    string SourceDir = Path.GetDirectoryName(packPathTextBlock.Text);
                    string foldername = SourceDir.Substring(SourceDir.LastIndexOf("\\") + 1);
                    string TargetDir = @"\\" + _mainWindow.pcTextBox.Text + @"\c$\TEMP\" + foldername;

                    if (!Directory.Exists(TargetDir))
                    {
                        installerProgressBar.Visibility = Visibility.Visible;
                        await Task.Run(() =>
                        {
                            foreach (string dirPath in Directory.GetDirectories(SourceDir, "*",
                                SearchOption.AllDirectories))
                                Directory.CreateDirectory(dirPath.Replace(SourceDir, TargetDir));
                        });

                        await Task.Run(() =>
                        {
                            foreach (string newPath in Directory.GetFiles(SourceDir, "*.*",
                                SearchOption.AllDirectories))
                                File.Copy(newPath, newPath.Replace(SourceDir, TargetDir));
                        });
                        installerProgressBar.Visibility = Visibility.Hidden;
                    }
                }
                catch (Exception ex)
                {
                    installerPopupText.Text = ex.Message;
                    installerPopupBox.IsPopupOpen = true;
                    installerProgressBar.Visibility = Visibility.Hidden;
                    return;
                }
            }
        }
    }
}
