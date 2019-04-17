using Microsoft.Win32;
using SDT.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SDT.Services
{
    public class PC
    {
        private readonly MainWindow _mainWindow;    

        public PC(MainWindow MainWindow)
        {
            _mainWindow = MainWindow;
        }

        /// <summary>
        /// Ping PC address
        /// </summary>
        public async Task<bool> Ping()
        { 
            try
            {
                _mainWindow.pcProgressBar.Visibility = Visibility.Visible;
                var address = _mainWindow.pcTextBox.Text;

                var pingAnswer = await Task.Run(() =>
                {
                    Ping ping = new Ping();
                    return ping.Send(address);
                });
                if (pingAnswer.Status == IPStatus.Success)
                {
                    _mainWindow.pcTextBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF64DD17");
                    _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;
                    return true;
                }
                else
                {
                    _mainWindow.pcTextBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFEF5350");
                    _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;
                    return false;
                }
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;
                return false;
            }
        }

        /// <summary>
        /// Ping address for Installer
        /// </summary>
        public async Task<bool> PingInstaller()
        {
            try
            {
                var address = _mainWindow.pcTextBox.Text;

                var pingAnswer = await Task.Run(() =>
                {
                    Ping ping = new Ping();
                    return ping.Send(address);
                });
                if (pingAnswer.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return false;
            }
        }

        /// <summary>
        /// Run > Sharing
        /// </summary>
        public async void Sharing()
        {
            var pingCheck = await Ping();
            if (pingCheck)
            {
                try
                {
                    Process.Start(@"\\" + _mainWindow.pcTextBox.Text + @"\C$");
                }
                catch (Exception e)
                {
                    _mainWindow.popupText.Text = e.Message + "- Adres: " + _mainWindow.pcTextBox.Text;
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    return;
                }
            }
            else
            {
                _mainWindow.popupText.Text = "Stacja nie odpowiada w sieci.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Information about PC
        /// </summary>
        public async void PCinfo()
        {
            var pingCheck = await Ping();
            if (pingCheck)
            {
                string address = _mainWindow.pcTextBox.Text;
                _mainWindow.pcProgressBar.Visibility = Visibility.Visible;
                try
                {
                    IPAddress[] ip = await Task.Run(() =>
                    {
                        IPHostEntry hostname = Dns.GetHostEntry(address);
                        return hostname.AddressList;
                    });

                    _mainWindow.pcIpTextBox.Text = ip[0].ToString();

                    ManagementScope Scope;
                    Scope = new ManagementScope(string.Format("\\\\{0}\\root\\CIMV2", _mainWindow.pcTextBox.Text), null);
                    Scope.Connect();

                    ObjectQuery Query1 = new ObjectQuery("Select * From Win32_NetworkAdapterConfiguration where IPEnabled = 1");
                    ManagementObjectSearcher Searcher1 = new ManagementObjectSearcher(Scope, Query1);
                    foreach (ManagementObject WmiObject in Searcher1.Get())
                    {
                        _mainWindow.pcMacTextBox.Text = WmiObject["MacAddress"].ToString();
                    }
                    ObjectQuery Query2 = new ObjectQuery("Select * From Win32_BIOS");
                    ManagementObjectSearcher Searcher2 = new ManagementObjectSearcher(Scope, Query2);
                    foreach (ManagementObject WmiObject in Searcher2.Get())
                    {
                        _mainWindow.pcSerialTextBox.Text = (WmiObject["SerialNumber"].ToString());
                    }
                    ObjectQuery Query3 = new ObjectQuery("Select * From Win32_LogicalDisk where DeviceID = 'C:'");
                    ManagementObjectSearcher Searcher3 = new ManagementObjectSearcher(Scope, Query3);
                    foreach (ManagementObject WmiObject in Searcher3.Get())
                    {
                        ulong miejsce = ulong.Parse(WmiObject["FreeSpace"].ToString());
                        ulong wolne = (miejsce / (1024 * 1024 * 1024));
                        _mainWindow.pcSpaceTextBox.Text = wolne + " GB".ToString();
                    }
                    ObjectQuery Query4 = new ObjectQuery("Select * from Win32_ComputerSystem");
                    ManagementObjectSearcher Searcher4 = new ManagementObjectSearcher(Scope, Query4);
                    foreach (ManagementObject WmiObject in Searcher4.Get())
                    {
                        var userLogged = WmiObject["UserName"].ToString();
                        _mainWindow.pcNetbiosTextBox.Text = WmiObject["Name"].ToString();
                        if (string.IsNullOrWhiteSpace(userLogged))
                        {
                            _mainWindow.pcUserLoggedTextBox.Text = "Nikt nie jest zalogowany na stacji";
                        }
                        else
                        {
                            _mainWindow.pcUserLoggedTextBox.Text = userLogged.Substring(userLogged.IndexOf("\\") + 1);
                        }
                    }
                    ObjectQuery Query5 = new ObjectQuery("Select * from Win32_OperatingSystem");
                    ManagementObjectSearcher Searcher5 = new ManagementObjectSearcher(Scope, Query5);
                    foreach (ManagementObject WmiObject in Searcher5.Get())
                    {
                        string os = WmiObject["Caption"].ToString();
                        string osversion = WmiObject["Version"].ToString(); 
                        _mainWindow.pcSystem.Text = os;

                        if (string.IsNullOrWhiteSpace(osversion)) { _mainWindow.pcSystemVersion.Text = "Brak informacji"; }
                        else if (osversion.Contains("10.0.10240")) { _mainWindow.pcSystemVersion.Text = "10.0.10240 - 1507"; }
                        else if (osversion.Contains("10.0.10586")) { _mainWindow.pcSystemVersion.Text = "10.0.10586 - 1511"; }
                        else if (osversion.Contains("10.0.14393")) { _mainWindow.pcSystemVersion.Text = "10.0.14393 - 1607"; }
                        else if (osversion.Contains("10.0.15063")) { _mainWindow.pcSystemVersion.Text = "10.0.15063 - 1703"; }
                        else if (osversion.Contains("10.0.16299")) { _mainWindow.pcSystemVersion.Text = "10.0.16299 - 1709"; }
                        else if (osversion.Contains("10.0.17134")) { _mainWindow.pcSystemVersion.Text = "10.0.17134 - 1803"; }
                        else if (osversion.Contains("10.0.17763")) { _mainWindow.pcSystemVersion.Text = "10.0.17763 - 1809"; }
                        else if (osversion.Contains("10.0.18309")) { _mainWindow.pcSystemVersion.Text = "10.0.18309 - 1903"; }
                    }

                    List<string> output = await Task.Run(() =>
                    {
                        var ctx = new PrincipalContext(ContextType.Domain);
                        ComputerPrincipal computer = ComputerPrincipal.FindByIdentity(ctx, address);

                        return output = computer.GetGroups().Select(x => x.SamAccountName).ToList();
                    });

                    _mainWindow.joulex_CheckBox.IsChecked = output.Contains("Comp-Joulex-wyjatki-GD");
                    _mainWindow.infonoc_CheckBox.IsChecked = output.Contains("Komp-Info-Noc-GD");
                    _mainWindow.alterbrow_CheckBox.IsChecked = output.Contains("Comp-AlterBrow-GD");

                    _mainWindow.printDenyCheckBox.IsChecked = output.Contains("ProxyBSTBlokada");

                    if (output.Contains("ProxyBSTBlokada"))
                    { _mainWindow.bstb_CheckBoxx.IsChecked = true; _mainWindow.bstb_CheckBoxx.Background = Brushes.Red; }

                    _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;


                    ObjectQuery Query6 = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                    ManagementObjectSearcher Searcher6 = new ManagementObjectSearcher(Scope, Query6);
                    foreach (ManagementObject WmiObject in Searcher6.Get())
                    {
                        DateTime updateTime = ManagementDateTimeConverter.ToDateTime(WmiObject["InstallDate"].ToString());
                        _mainWindow.pcSystemVersionUpdate.Text = updateTime.ToString();
                    }

                }
                catch (Exception e)
                {
                    _mainWindow.popupText.Text = e.Message;
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;
                    return;
                }
            }
            else
            {
                _mainWindow.popupText.Text = "Stacja nie odpowiada w sieci.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;
                return;
            }
        }

        /// <summary>
        /// Remote run PsExec (CMD)
        /// </summary>
        public async Task PsExecRUN()
        {
            var pingCheck = await Ping();
            if (pingCheck)
            {
                var psExecCheck = PsExecCheck();
                if (psExecCheck)
                {
                    var test1 = _mainWindow.pcTextBox.Text;
                    await Task.Run(() =>
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = @"C:\My Program Files\PsExec64.exe";
                        process.StartInfo.Arguments = String.Format(@"\\{0} CMD", test1);
                        process.Start();
                        process.WaitForExit();
                        return;
                    });

                }
                else
                {
                    _mainWindow.popupText.Text = "Nie można uruchomić PsExec, ponieważ nie jest zainstalowany na stacji.";
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    return;
                }
            }
            else
            {
                _mainWindow.popupText.Text = "Stacja nie odpowiada w sieci.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Check PsExec installation
        /// </summary>
        public bool PsExecCheck()
        {
            if (!File.Exists(@"C:\My Program Files\PsExec64.exe"))
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
        /// Install PsExec
        /// </summary>
        void PsExecInstall()
        {
            try
            {
                File.WriteAllBytes(@"C:\My Program Files\PsExec64.exe", Resources.PsExec64);

                _mainWindow.popupText.Text = "Zainstalowano PsExec na stacji.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        ///  Run Cmrcviewer.exe with ip/netbios parameter
        /// </summary>
        public async void Cmrcviewer()
        {
            if (File.Exists(@"C:\Program Files (x86)\Microsoft Configuration Manager\AdminConsole\bin\i386\CmRcViewer.exe"))
            {
                var pingcheck = await Ping();
                if (pingcheck)
                    Process.Start(@"C:\Program Files (x86)\Microsoft Configuration Manager\AdminConsole\bin\i386\CmRcViewer.exe", _mainWindow.pcTextBox.Text);
                else
                {
                    _mainWindow.popupText.Text = "Stacja nie odpowiada w sieci.";
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    return;
                }
            }
            else
            {
                _mainWindow.popupText.Text = "Brak zainstalowanego narzędzia RCV";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        ///  Remote check ports
        /// </summary>
        public async Task PortCheck()
        {
            var pingCheck = await Ping();
            if (pingCheck)
            {
                try
                {
                    _mainWindow.pcProgressBar.Visibility = Visibility.Visible;
                    await Port135();
                    await Port445();
                    await Port2701();
                    await Port8081();
                    _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;
                }
                catch (Exception ex)
                {
                    _mainWindow.popupText.Text = ex.Message;
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;
                    return;
                }
            }
            else
            {
                _mainWindow.popupText.Text = "Stacja nie odpowiada w sieci.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        ///  Tasks for check ports
        /// </summary>
        async Task Port135()
        {
            var cancelcts = new TaskCompletionSource<bool>();
            using (var cts = new CancellationTokenSource(5000))
            {
                using (var tcpClient = new TcpClient())
                {
                    var task = tcpClient.ConnectAsync(_mainWindow.pcTextBox.Text, 135);
                    using (cts.Token.Register(() => cancelcts.TrySetResult(true)))
                    {
                        if (task != await Task.WhenAny(task, cancelcts.Task))
                            _mainWindow.port135_CheckBox.IsChecked = false;
                        else
                            _mainWindow.port135_CheckBox.IsChecked = true;
                    }
                }
            }
        }
        async Task Port445()
        {
            var cancelcts = new TaskCompletionSource<bool>();
            using (var cts = new CancellationTokenSource(5000))
            {
                using (var tcpClient = new TcpClient())
                {
                    var task = tcpClient.ConnectAsync(_mainWindow.pcTextBox.Text, 445);
                    using (cts.Token.Register(() => cancelcts.TrySetResult(true)))
                    {
                        if (task != await Task.WhenAny(task, cancelcts.Task))
                            _mainWindow.port445_CheckBox.IsChecked = false;
                        else
                            _mainWindow.port445_CheckBox.IsChecked = true;
                    }
                }
            }
        }
        async Task Port2701()
        {
            var cancelcts = new TaskCompletionSource<bool>();
            using (var cts = new CancellationTokenSource(5000))
            {
                using (var tcpClient = new TcpClient())
                {
                    var task = tcpClient.ConnectAsync(_mainWindow.pcTextBox.Text, 2701);
                    using (cts.Token.Register(() => cancelcts.TrySetResult(true)))
                    {
                        if (task != await Task.WhenAny(task, cancelcts.Task))
                            _mainWindow.port2701_CheckBox.IsChecked = false;
                        else
                            _mainWindow.port2701_CheckBox.IsChecked = true;
                    }
                }
            }
        }
        async Task Port8081()
        {
            var cancelcts = new TaskCompletionSource<bool>();
            using (var cts = new CancellationTokenSource(5000))
            {
                using (var tcpClient = new TcpClient())
                {
                    var task = tcpClient.ConnectAsync(_mainWindow.pcTextBox.Text, 8081);
                    using (cts.Token.Register(() => cancelcts.TrySetResult(true)))
                    {
                        if (task != await Task.WhenAny(task, cancelcts.Task))
                            _mainWindow.port8081_CheckBox.IsChecked = false;
                        else
                            _mainWindow.port8081_CheckBox.IsChecked = true;
                    }
                }
            }
        }

        /// <summary>
        /// CMD ping with -t
        /// </summary>
        public void PingT()
        {
            string cmdText = "/C ping " + _mainWindow.pcTextBox.Text + " -t";
            Process.Start("CMD.exe", cmdText);
        }


        /// <summary>
        /// Scripts
        /// </summary>

        public async void GPUpdate()
        {
            var pingcheck = await Ping();
            if (pingcheck)
            {
                var psexcheck = PsExecCheck();
                if (psexcheck)
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                        process.StartInfo.Arguments = String.Format(@"/k ""C:\My Program Files\PsExec64.exe"" \\{0} gpupdate /force", _mainWindow.pcTextBox.Text);
                        process.EnableRaisingEvents = true;
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        _mainWindow.popupText.Text = e.Message;
                        _mainWindow.mainPopupBox.IsPopupOpen = true;
                        return;
                    }
                }
            }
            else
            {
                _mainWindow.popupText.Text = "Stacja nie odpowiada w sieci.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        public async void BitLocker()
        {
            var pingcheck = await Ping();
            if (pingcheck)
            {
                var psexcheck = PsExecCheck();
                if (psexcheck)
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                        process.StartInfo.Arguments = String.Format(@"/k ""C:\My Program Files\PsExec64.exe"" \\{0} manage-bde -status", _mainWindow.pcTextBox.Text);
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        _mainWindow.popupText.Text = e.Message;
                        _mainWindow.mainPopupBox.IsPopupOpen = true;
                        return;
                    }
                }
            }
            else
            {
                _mainWindow.popupText.Text = "Stacja nie odpowiada w sieci.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        public void IEFix()
        {
            string ips = _mainWindow.pcTextBox.Text;
            string subkey = @"SYSTEM\CurrentControlSet\Services\\NlaSvc\Parameters\Internet";

            try
            {
                RegistryKey myKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, ips, RegistryView.Registry64)
                        .OpenSubKey(subkey, true);
                {
                    myKey.SetValue("EnableActiveProbing", "0", RegistryValueKind.DWord);
                    myKey.Close();

                    _mainWindow.popupText.Text = "Zmieniono wpis w rejestrze.";
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                _mainWindow.popupText.Text = ex.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        public async void SpoolerReset()
        {
            _mainWindow.pcProgressBar.Visibility = Visibility.Visible;
            string ips = _mainWindow.pcTextBox.Text;

            try
            {
                ServiceController sc = new ServiceController("Spooler", ips);
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    await Task.Run(() =>
                    {
                        sc.Start();
                    });
                    _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;

                    _mainWindow.popupText.Text = "Uruchomiono Bufor Wydruku na stacji.";
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    return;
                }
                else
                {
                    await Task.Run(() =>
                    {
                        sc.Stop();
                        Thread.Sleep(3000);
                        sc.Start();
                    });
                    _mainWindow.pcProgressBar.Visibility = Visibility.Hidden;

                    _mainWindow.popupText.Text = "Zrestartowano Bufor Wydruku na stacji.";
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                _mainWindow.popupText.Text = ex.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

    }
}
