using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SDT
{
    /// <summary>
    /// Interaction logic for PC_Installer.xaml
    /// </summary>
    public partial class PC_Installer : MetroWindow
    {
        public PC_Installer(TextBox TextBox_PCin)
        {
            InitializeComponent();

            List<KeyValuePair<string, string>> servs = new List<KeyValuePair<string, string>>
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

            Combo_Instal.ItemsSource = servs;
            Combo_Instal.DisplayMemberPath = "Key";
            Combo_Instal.SelectedValue = "Value";

            TextBox_PCadress.Text = TextBox_PCin.Text;
        }

        /// <summary>
        /// Installer - Selection of pack from servers/windows explorer
        /// </summary>
        private void ServerSelectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedserver = Combo_Instal.SelectedItem as KeyValuePair<string, string>?;
                if (selectedserver.HasValue)
                {
                    var selserver = selectedserver.Value.ToString();
                    var selserver2 = selserver.Split(',', ']')[1];
                    selserver2 = string.Join("", selserver2.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    OpenFileDialog expl = new OpenFileDialog();
                    expl.Title = "Wybierz plik instalacyjny";
                    expl.Filter = "Pliki CMD (*.cmd)|*.cmd|Pliki BAT (*.bat)|*.bat|Wszystkie pliki (*.)|*.";
                    expl.InitialDirectory = selserver2;
                    expl.ShowDialog();

                    string batfullpath = expl.FileName;
                    TextBox_Instapath.Text = batfullpath;
                }
            }
            catch (Exception er)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", er.Message);
                return;
            }
        }

        /// <summary>
        /// Installer - Copy and install pack
        /// </summary>
        public async void Button_Insta_Click(object sender, RoutedEventArgs e)
        {
            PC pec = new PC();
            await CopyDirec();

            if (!string.IsNullOrWhiteSpace(TextBox_Instapath.Text))
            {
                var pingcheck = await pec.Ping(TextBox_PCadress);
                if (pingcheck)
                {
                    try
                    {
                        var path0 = TextBox_Instapath.Text.Substring(0, TextBox_Instapath.Text.LastIndexOf("\\"));
                        string path1 = TextBox_Instapath.Text.Substring(path0.LastIndexOf("\\"));
                        string path2 = @" C:\TEMP" + path1;

                        Process process = new Process();
                        process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                        process.StartInfo.Arguments = String.Format(@"/k ""C:\My Program Files\PsExec64.exe"" \\{0} {1}", TextBox_PCadress.Text, path2);
                        process.EnableRaisingEvents = true;
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                        if (window != null)
                            await window.ShowMessageAsync("Błąd!", ex.Message);
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Installer - Copy install pack
        /// </summary>
        private async void Button_InstaCopy_Click(object sender, RoutedEventArgs e)
        {
            PC pec = new PC();
            var pingcheck = await pec.Ping(TextBox_PCadress);
            if (pingcheck)
            {
                await CopyDirec();
            }
            else
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Bład!", "Stacja nie odpowiada w sieci.");
                return;
            }
        }

        /// <summary>
        /// Installer - Open remote TEMP
        /// </summary>
        private async void Button_InstaTemp_Click(object sender, RoutedEventArgs e)
        {
            PC pec = new PC();
            var pingcheck = await pec.Ping(TextBox_PCadress);
            if (pingcheck)
            {
                try
                {
                    Process.Start(@"\\" + TextBox_PCadress.Text + @"\c$\TEMP");
                }
                catch (Exception ex)
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Błąd!", ex.Message);
                    return;
                }
            }
            else
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Bład!", "Stacja nie odpowiada w sieci.");
                return;
            }
        }

        /// <summary>
        /// Installer - Clear remote TEMP
        /// </summary>
        private async void Button_InstaClearTemp_Click(object sender, RoutedEventArgs e)
        {
            PC pec = new PC();
            var pingcheck = await pec.Ping(TextBox_PCadress);
            if (pingcheck)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(@"\\" + TextBox_PCadress.Text + @"\c$\TEMP");

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
                    var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Błąd!", ex.Message);
                    return;
                }
            }
            else
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Bład!", "Stacja nie odpowiada w sieci.");
                return;
            }
        }

        /// <summary>
        /// Copy installation folder to TEMP
        /// </summary>
        private async Task CopyDirec()
        {
            if (string.IsNullOrWhiteSpace(TextBox_Instapath.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault(); //(PC_Installer => PC_Installer.IsActive);
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", "Wybierz plik instalacyjny");
                return;
            }
            else
            {
                try
                {
                    string SourceDir = Path.GetDirectoryName(TextBox_Instapath.Text);
                    string foldername = SourceDir.Substring(SourceDir.LastIndexOf("\\") + 1);
                    string TargetDir = @"\\" + TextBox_PCadress.Text + @"\c$\TEMP\" + foldername;

                    if (!Directory.Exists(TargetDir))
                    {
                        WaitBarCopy.IsIndeterminate = true;
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
                        WaitBarCopy.IsIndeterminate = false;
                    }
                }
                catch (Exception ex)
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Bład!", ex.Message);
                    WaitBarCopy.IsIndeterminate = false;
                    return;
                }
            }
        }
    }
}
