﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SDT.Properties;
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
    public class PC
    {
        private readonly MainWindow _MetroWindow;

        public PC(MainWindow MetroWindow)
        {
            _MetroWindow = MetroWindow;
        }

        public PC()
        {

        }

        /// <summary>
        /// Check PC ping
        /// </summary>
        public async Task<bool> Ping(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        { 
            try
            {
                WaitBarPC.Visibility = Visibility.Visible;
                var ipa = TextBox_PCin.Text;

                var PingOdp = await Task.Run(() =>
                {
                    Ping PingZapytanie = new Ping();
                    return PingZapytanie.Send(ipa);
                });
                if (PingOdp.Status == IPStatus.Success)
                {
                    TextBox_PCin.Background = new SolidColorBrush(Colors.Green);
                    WaitBarPC.Visibility = Visibility.Hidden;
                    return true;
                }
                else
                {
                    TextBox_PCin.Background = new SolidColorBrush(Colors.Red);
                    WaitBarPC.Visibility = Visibility.Hidden;
                    return false;
                }
            }
            catch (Exception e)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", e.Message);
                WaitBarPC.Visibility = Visibility.Hidden;
                return false;
            }
        }

        /// <summary>
        /// Run > Sharing
        /// </summary>
        public async void Sharing(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            var pingcheck = await Ping(TextBox_PCin, WaitBarPC);
            if (pingcheck)
            {
                try
                {
                    Process.Start(@"\\" + TextBox_PCin.Text + @"\C$");
                }
                catch (Exception e)
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Błąd!", e.Message + "- Adres: " + TextBox_PCin.Text);
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
        /// Information about PC
        /// </summary>
        public async void PCinfo(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            var pingcheck = await Ping(TextBox_PCin, WaitBarPC);
            if (pingcheck)
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
            else
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Bład!", "Stacja nie odpowiada w sieci.");
                return;
            }

}

        /// <summary>
        /// Remote run PsExec (CMD)
        /// </summary>
        public async void PsExecRUN(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            var pingcheck = await Ping(TextBox_PCin, WaitBarPC);
            if (pingcheck)
            {
                var pscheck = PsExecCheck();
                if (pscheck)
                {
                    Process process = new Process();
                    process.StartInfo.FileName = @"C:\Windows\SysWoW64\PsExec64.exe";
                    process.StartInfo.Arguments = String.Format(@"\\{0} CMD", TextBox_PCin.Text);
                    process.Start();
                    process.WaitForExit();
                }
                else
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("PsExec", "Nie można uruchomić PsExec, ponieważ nie jest zainstalowany na stacji.");
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
        /// Check PsExec installation
        /// </summary>
        public bool PsExecCheck()
        {
            if (!File.Exists(@"C:\Windows\SysWOW64\PsExec64.exe"))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Czy chcesz zainstalować?", "Brak PsExec na stacji.", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    PsExecInstall();
                    return true;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Install PsExec on PC
        /// </summary>
        void PsExecInstall()
        {
            try
            {
                File.WriteAllBytes(@"C:\Windows\SysWOW64\PsExec64.exe", Resources.PsExec64);

                Process.Start("cmd", @"/c ""C:\Windows\SysWOW64\PsExec64.exe""");

                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                   window.ShowMessageAsync("PsExec", "Zainstalowano PsExec na stacji.");
                return;
            }
            catch (Exception e)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", e.Message);
                return;
            }
        }

        /// <summary>
        ///  Run Cmrcviewer.exe with ip/netbios parameter
        /// </summary>
        public async void Cmrcviewer(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            if (File.Exists(@"C:\Program Files (x86)\Microsoft Configuration Manager\AdminConsole\bin\i386\CmRcViewer.exe"))
            {
                var pingcheck = await Ping(TextBox_PCin, WaitBarPC);
                if (pingcheck)
                {
                    Process.Start(@"C:\Program Files (x86)\Microsoft Configuration Manager\AdminConsole\bin\i386\CmRcViewer.exe", TextBox_PCin.Text);
                }
                else
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Bład!", "Stacja nie odpowiada w sieci.");
                    return;
                }
            }
            else
            {
                TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Błąd!", "Brak zainstalowanego narzędzia RCV.");
                    return;
                }
            }
        }
    }
}
