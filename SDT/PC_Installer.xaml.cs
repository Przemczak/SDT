using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            TextBox_PCadress.Text = TextBox_PCin.Text;
        }

        private void Button_InstaServ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog expl = new OpenFileDialog();
                expl.Title = "Wybierz plik instalacyjny";
                expl.Filter = "Pliki CMD (*.cmd)|*.cmd|Pliki BAT (*.bat)|*.bat|Wszystkie pliki (*.)|*.";
                switch (Combo_Instal.Text)
                {
                    case "OPSCCM2012BY1":
                        expl.InitialDirectory = @"\\OPSCCM2012BY1\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012GD1":
                        expl.InitialDirectory = @"\\OPSCCM2012GD1\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012KA1":
                        expl.InitialDirectory = @"\\OPSCCM2012KA1\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012KR1":
                        expl.InitialDirectory = @"\\OPSCCM2012KR1\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012OL1":
                        expl.InitialDirectory = @"\\OPSCCM2012OL1\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012PO1":
                        expl.InitialDirectory = @"\\OPSCCM2012PO1\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012WA1":
                        expl.InitialDirectory = @"\\OPSCCM2012WA1\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012WA2":
                        expl.InitialDirectory = @"\\OPSCCM2012WA2\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012WR1":
                        expl.InitialDirectory = @"\\OPSCCM2012WR1\tpbin";
                        expl.ShowDialog();
                        break;
                    case "OPSCCM2012SZ1":
                        expl.InitialDirectory = @"\\OPSCCM2012SZ1\tpbin";
                        expl.ShowDialog();
                        break;
                }
                string batfullpath = expl.FileName;
                TextBox_Instapath.Text = batfullpath;
            }
            catch (Exception er)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", er.Message);
                return;
            }
        }

        public void Button_InstaExplo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog expl = new OpenFileDialog();
            expl.Title = "Wybierz plik instalacyjny";
            expl.Filter = "Pliki CMD (*.cmd)|*.cmd|Pliki BAT (*.bat)|*.bat|Wszystkie pliki (*.)|*.";
            expl.InitialDirectory = @"C:\";
            expl.RestoreDirectory = true;
            expl.ShowDialog();

            string batfullpath = expl.FileName;
            TextBox_Instapath.Text = batfullpath;
        }

        public async void Button_Insta_Click(object sender, RoutedEventArgs e)
        {
            PC pec = new PC();
            await CopyDirec();

            if (!string.IsNullOrWhiteSpace(TextBox_Instapath.Text))
            {
                var pingcheck = await pec.Ping(TextBox_PCadress);
                if (pingcheck)
                {
                    var psexcheck = pec.PsExecCheck();
                    if (psexcheck)
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
                            var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                            if (window != null)
                                await window.ShowMessageAsync("Błąd!", ex.Message);
                            return;

                        }
                    }
                }
            }
            else
            {
                return;
            }

        }

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
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
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
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Błąd!", ex.Message);
                    return;
                }
            }
            else
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
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
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Błąd!", ex.Message);
                    return;
                }
            }
            else
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
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
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
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
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Bład!", ex.Message);
                    WaitBarCopy.IsIndeterminate = false;
                    return;
                }
            }
        }
    }
}
