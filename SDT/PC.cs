using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SDT
{
    class PC
    {
        private readonly MainWindow _MetroWindow;

        public PC(MainWindow MetroWindow)
        {
            _MetroWindow = MetroWindow;
        }

        /// <summary>
        /// Check PC ping
        /// </summary>
        public async Task Ping(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        { 
            try
            {
                WaitBarPC.Visibility = Visibility.Visible;
                var ipa = TextBox_PCin.Text;

                PingReply PingOdp = await Task.Run(() =>
                {
                    Ping PingZapytanie = new Ping();
                    return PingZapytanie.Send(ipa);
                });

                if (PingOdp.Status == IPStatus.Success)
                {
                    TextBox_PCin.Background = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    TextBox_PCin.Background = new SolidColorBrush(Colors.Red);
                }
            }
            catch (Exception e)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", e.Message);
                WaitBarPC.Visibility = Visibility.Hidden;
                return;
            }
        }

        /// <summary>
        /// Run > Sharing
        /// </summary>
        public void Sharing(TextBox TextBox_PCin)
        {
            try
            {
                Process.Start(@"\\" + TextBox_PCin.Text + @"\C$");
            }
            catch (Exception e)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", e.Message + "- Adres: " + TextBox_PCin.Text);
                return;
            }
        }

        /// <summary>
        /// Information about PC
        /// </summary>
        public async void PCinfo(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            string pcadress = TextBox_PCin.Text;
            WaitBarPC.Visibility = Visibility.Visible;
            try
            {
                IPAddress[] ip = await Task.Run(() =>
                {
                    IPHostEntry hostname = Dns.GetHostEntry(pcadress);
                    return hostname.AddressList;
                });

                _MetroWindow.TextBox_PCip.Text = ip[0].ToString();

                ManagementScope Scope;
                Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", TextBox_PCin.Text), null);
                Scope.Connect();

                ObjectQuery Query = new ObjectQuery("Select * From Win32_ComputerSystem");
                ManagementObjectSearcher Searcher = new ManagementObjectSearcher(Scope, Query);
                foreach (ManagementObject WmiObject in Searcher.Get())
                {
                    _MetroWindow.TextBox_PCnetbios.Text = (WmiObject["Name"].ToString());
                }
                ObjectQuery Query1 = new ObjectQuery("Select * From Win32_NetworkAdapterConfiguration where IPEnabled = 1");
                ManagementObjectSearcher Searcher1 = new ManagementObjectSearcher(Scope, Query1);
                foreach (ManagementObject WmiObject in Searcher1.Get())
                {
                    _MetroWindow.TextBox_PCmac.Text = (WmiObject["MacAddress"].ToString());
                }
                ObjectQuery Query2 = new ObjectQuery("Select * From Win32_BIOS");
                ManagementObjectSearcher Searcher2 = new ManagementObjectSearcher(Scope, Query2);
                foreach (ManagementObject WmiObject in Searcher2.Get())
                {
                    _MetroWindow.TextBox_PCns.Text = (WmiObject["SerialNumber"].ToString());
                }
                ObjectQuery Query3 = new ObjectQuery("Select * From Win32_LogicalDisk where DeviceID = 'C:'");
                ManagementObjectSearcher Searcher3 = new ManagementObjectSearcher(Scope, Query3);
                foreach (ManagementObject WmiObject in Searcher3.Get())
                {
                    UInt64 miejsce = UInt64.Parse(WmiObject["FreeSpace"].ToString());
                    UInt64 wolne = (miejsce / (1024 * 1024 * 1024));
                    _MetroWindow.TextBox_PCspace.Text = wolne + " GB".ToString();
                }
                ObjectQuery Query4 = new ObjectQuery("Select * from Win32_ComputerSystem");
                ManagementObjectSearcher Searcher4 = new ManagementObjectSearcher(Scope, Query4);
                foreach (ManagementObject WmiObject in Searcher4.Get())
                {
                    var zalo = (WmiObject["UserName"].ToString());
                    _MetroWindow.TextBox_PCloguser.Text = zalo.Substring(zalo.IndexOf("\\") + 1);
                }
                WaitBarPC.Visibility = Visibility.Hidden;
            }
            catch (Exception e)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", e.Message);
                WaitBarPC.Visibility = Visibility.Hidden;
                return;
            }
}

        void PsExecRUN()
        {
            //TODO: URUCHOMIENIE PSEXEC - CZYSTE OKNO
        }

        void PsExcecInstall()
        {
            //TODO: Instalator aplikacji
        }

        /// <summary>
        /// Check PsExec installation
        /// </summary>
        void PsExecCheck()
        {
            if (!Directory.Exists(@"C:\My Program Files\PSTools"))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Czy chcesz zainstalować PsExec?", "Instalacja PsExec", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    PsExecInstall();
                }
            }
        }

        public void PsExecInstall()
        {
            Directory.CreateDirectory(@"C:\My Program Files\PSTools");

            string sourcePath = @"\\dsb192\$SDT\Resources\PSTools";
            string targetPath = @"C:\My Program Files\PSTools";
            string fileName = string.Empty;
            string destFile = string.Empty;

                string[] files = Directory.GetFiles(sourcePath);

                foreach (string s in files)
                {
                    fileName = Path.GetFileName(s);
                    destFile = Path.Combine(targetPath, fileName);
                    File.Copy(s, destFile, true);
                }

            //TODO: INSTALACJA PSEXEC NA STACJI
            // Async
        }



    }
}
