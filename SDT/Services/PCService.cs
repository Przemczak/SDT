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
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using SDT.Properties;
using SDT.ViewModels;
using SDT.Models;

namespace SDT.Services
{
    class PCService
    {
        private PC pcModel;
        private ApplicationViewModel applicationVM;
        private IDialogCoordinator dialogCoordinator;
        private string PcName;

        public PCService(PC PCModel, ApplicationViewModel ApplicationVM, IDialogCoordinator DialogCoordinator)
        {
            pcModel = PCModel; 
            applicationVM = ApplicationVM;
            dialogCoordinator = DialogCoordinator;
        }

        public async Task CheckPc()
        {
            PcName = pcModel.PcName;

            if (await PingValidation())
            {
                try
                {
                    await IpAddressCheck();
                    await ComputerInfoCheck();
                    await AdGroupsCheck();
                    await PcCheckPorts();

                    async Task IpAddressCheck()
                    {
                        IPAddress[] ip = await Task.Run(() =>
                        {
                            IPHostEntry hostname = Dns.GetHostEntry(PcName);
                            return hostname.AddressList;
                        });
                        pcModel.PcIP = ip[0].ToString();
                    }

                    async Task ComputerInfoCheck()
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                ManagementScope mScope = new ManagementScope(string.Format("\\\\{0}\\root\\CIMV2", PcName), null);
                                mScope.Connect();

                                if (mScope != null)
                                {
                                    ObjectQuery oQuery = new ObjectQuery("Select * From Win32_NetworkAdapterConfiguration where IPEnabled = 1");
                                    ObjectQuery oQuery1 = new ObjectQuery("Select * From Win32_BIOS");
                                    ObjectQuery oQuery2 = new ObjectQuery("Select * From Win32_LogicalDisk where DeviceID = 'C:'");
                                    ObjectQuery oQuery3 = new ObjectQuery("Select * from Win32_ComputerSystem");
                                    ObjectQuery oQuery4 = new ObjectQuery("Select * from Win32_OperatingSystem");

                                    using (ManagementObjectSearcher mObjectSearcher = new ManagementObjectSearcher(mScope, oQuery))
                                    {
                                        foreach (ManagementObject mObject in mObjectSearcher.Get())
                                        {
                                            pcModel.PcMAC = mObject["MacAddress"].ToString();
                                        }
                                    }
                                    using (ManagementObjectSearcher mObjectSearcher = new ManagementObjectSearcher(mScope, oQuery1))
                                    {
                                        foreach (ManagementObject mObject in mObjectSearcher.Get())
                                        {
                                            pcModel.PcNS = mObject["SerialNumber"].ToString();
                                        }
                                    }
                                    using (ManagementObjectSearcher mObjectSearcher = new ManagementObjectSearcher(mScope, oQuery2))
                                    {
                                        foreach (ManagementObject mObject in mObjectSearcher.Get())
                                        {
                                            ulong miejsce = ulong.Parse(mObject["FreeSpace"].ToString());
                                            ulong wolne = (miejsce / (1024 * 1024 * 1024));
                                            pcModel.PcFreeSpace = wolne + " GB".ToString();
                                        }
                                    }
                                    using (ManagementObjectSearcher mObjectSearcher = new ManagementObjectSearcher(mScope, oQuery3))
                                    {
                                        foreach (ManagementObject mObject in mObjectSearcher.Get())
                                        {
                                            string userLogin = mObject["UserName"].ToString();
                                            if (string.IsNullOrWhiteSpace(userLogin))
                                            { pcModel.PcLoggedUser = "Nikt nie jest zalogowany na stacji"; }
                                            else
                                            { pcModel.PcLoggedUser = userLogin.Substring(userLogin.IndexOf("\\") + 1); }
                                        }
                                    }
                                    using (ManagementObjectSearcher mObjectSearcher = new ManagementObjectSearcher(mScope, oQuery4))
                                    {
                                        foreach (ManagementObject mObject in mObjectSearcher.Get())
                                        {
                                            //OS 
                                            pcModel.PcSystem = mObject["Caption"].ToString();

                                            // OS Version
                                            string systemVersion = mObject["Version"].ToString();

                                            if (string.IsNullOrWhiteSpace(systemVersion)) { pcModel.PcSystemVersion = "Brak informacji"; }
                                            else if (systemVersion.Contains("10.0.10240")) { pcModel.PcSystemVersion = "10.0.10240 - 1507"; }
                                            else if (systemVersion.Contains("10.0.10586")) { pcModel.PcSystemVersion = "10.0.10586 - 1511"; }
                                            else if (systemVersion.Contains("10.0.14393")) { pcModel.PcSystemVersion = "10.0.14393 - 1607"; }
                                            else if (systemVersion.Contains("10.0.15063")) { pcModel.PcSystemVersion = "10.0.15063 - 1703"; }
                                            else if (systemVersion.Contains("10.0.16299")) { pcModel.PcSystemVersion = "10.0.16299 - 1709"; }
                                            else if (systemVersion.Contains("10.0.17134")) { pcModel.PcSystemVersion = "10.0.17134 - 1803"; }
                                            else if (systemVersion.Contains("10.0.17763")) { pcModel.PcSystemVersion = "10.0.17763 - 1809"; }
                                            else if (systemVersion.Contains("10.0.18362")) { pcModel.PcSystemVersion = "10.0.18362 - 1903"; }
                                            else if (systemVersion.Contains("10.0.18363")) { pcModel.PcSystemVersion = "10.0.18363 - 1909"; }

                                            //OS Update Time
                                            DateTime updateTime = ManagementDateTimeConverter.ToDateTime(mObject["InstallDate"].ToString());
                                            pcModel.PcSystemUpdate = updateTime.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    dialogCoordinator.ShowMessageAsync(applicationVM, "Komputer Info", "Błąd połaczenia WMI."); 
                                    return;
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            await dialogCoordinator.ShowMessageAsync(applicationVM, "Komputer Info", ex.ToString()); 
                            return;
                        }
                    }

                    async Task AdGroupsCheck()
                    {
                        await Task.Run(() =>
                        {
                            using (PrincipalContext plContext = new PrincipalContext(ContextType.Domain))
                            {
                                ComputerPrincipal computerPrincipal = ComputerPrincipal.FindByIdentity(plContext, PcName);

                                List<string> groupsList = computerPrincipal.GetGroups().Select(x => x.SamAccountName).ToList();

                                pcModel.PcJoulex = groupsList.Contains("Comp-Joulex-wyjatki-GD");
                                pcModel.PcInfoNoc = groupsList.Contains("Komp-Info-Noc-GD");
                                pcModel.PcAlterBrowser = groupsList.Contains("Comp-AlterBrow-GD");
                                pcModel.PcProxyBSTB = groupsList.Contains("ProxyBSTBlokada");
                            };
                        });
                    }
                }
                catch (Exception ex)
                {
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "Ping", ex.ToString());
                    return;
                }
            }
        }

        private async Task PcCheckPorts()
        {
            if(await PingValidation())
            {
                try
                {
                    pcModel.PcPort135 = Port135Check();
                    pcModel.PcPort445 = Port445Check();
                    pcModel.PcPort2701 = Port2701Check();
                    pcModel.PcPort8081 = Port8081Check();

                    bool Port135Check()
                    {
                        using (TcpClient tcpClient = new TcpClient())
                        {
                            var result = tcpClient.BeginConnect(PcName, 135, null, null);
                            var checkResult = result.AsyncWaitHandle.WaitOne(5000);
                            tcpClient.EndConnect(result);
                            return checkResult;
                        }
                    }

                    bool Port445Check()
                    {
                        using (TcpClient tcpClient = new TcpClient())
                        {
                            var result = tcpClient.BeginConnect(PcName, 445, null, null);
                            var checkResult = result.AsyncWaitHandle.WaitOne(5000);
                            tcpClient.EndConnect(result);
                            return checkResult;
                        }
                    }

                    bool Port2701Check()
                    {
                        using (TcpClient tcpClient = new TcpClient())
                        {
                            var result = tcpClient.BeginConnect(PcName, 2701, null, null);
                            var checkResult = result.AsyncWaitHandle.WaitOne(5000);
                            tcpClient.EndConnect(result);
                            return checkResult;
                        }
                    }

                    bool Port8081Check()
                    {
                        using (TcpClient tcpClient = new TcpClient())
                        {
                            var result = tcpClient.BeginConnect(PcName, 8081, null, null);
                            var checkResult = result.AsyncWaitHandle.WaitOne(5000);
                            tcpClient.EndConnect(result);
                            return checkResult;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "Ping", ex.ToString());
                    return;
                }
            }
        }

        public async Task RunRCV()
        {
            if (await PingValidation())
            {
                try
                {
                    if (File.Exists(@"C:\Program Files (x86)\Microsoft Configuration Manager\AdminConsole\bin\i386\CmRcViewer.exe"))
                    { Process.Start(@"C:\Program Files (x86)\Microsoft Configuration Manager\AdminConsole\bin\i386\CmRcViewer.exe", PcName); }
                    else if (File.Exists(@"C:\Program Files (x86)\Microsoft Endpoint Manager\AdminConsole\bin\i386\CmRcViewer.exe"))
                    { Process.Start(@"C:\Program Files (x86)\Microsoft Endpoint Manager\AdminConsole\bin\i386\CmRcViewer.exe", PcName); }
                    else
                    {
                        await dialogCoordinator.ShowMessageAsync(applicationVM, "Bład - RCV", "Brak zainstalowanej aplikacji RCV"); 
                        return;
                    }
                }
                catch (Exception exceptionText)
                {
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "Bład - RCV", exceptionText.ToString()); 
                    return;
                }
            }
        }

        public async Task RunSharing()
        {
            if (await PingValidation())
            {
                try 
                { 
                    Process.Start(@"\\" + PcName + @"\C$"); 
                }
                catch (Exception exceptionText)
                {
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "Bład - Sharing", exceptionText.ToString()); return;
                }
            }
        }

        public async Task RunPsExec()
        {
            if (await PingValidation())
            {
                if (await PsExecValidation())
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = Directory.GetCurrentDirectory() + @"\Resources\PsExec64.exe";
                        process.StartInfo.Arguments = string.Format(@"\\{0} CMD", PcName);
                        process.Start();
                        process.WaitForExit();
                        return;
                    }
                    catch (Exception)
                    {
                        await dialogCoordinator.ShowMessageAsync(applicationVM, "Bład - PsExec", "Bład uruchomienia PsExec"); return;
                    }
                }
            }
        }

        public async Task RunPingT()
        {
            try
            {
                string pingCommand = "/C ping " + PcName + " -t";
                Process.Start("CMD.exe", pingCommand);
            }
            catch (Exception exceptionText)
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Bład - Ping", exceptionText.ToString());
                return;
            }
        }

        public async Task RunGPUUpdate()
        {
            if (await PingValidation())
            {
                if (await PsExecValidation())
                {
                    if (await PsExecValidation())
                    {
                        try
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                            process.StartInfo.Arguments = String.Format(@"/k" + Directory.GetCurrentDirectory() + @"\Resources\PsExec64.exe"" \\{0} gpupdate /force", PcName);
                            process.EnableRaisingEvents = true;
                            process.Start();
                        }
                        catch (Exception exceptionText)
                        {
                            await dialogCoordinator.ShowMessageAsync(applicationVM, "GpUpdate", exceptionText.ToString());
                            return;
                        }
                    }
                }
            }
        }

        public async Task CheckBitLocker()
        {
            if (await PingValidation())
            {
                if (await PsExecValidation())
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                        process.StartInfo.Arguments = String.Format(@"/k" + Directory.GetCurrentDirectory() + @"\Resources\PsExec64.exe"" \\{ 0} manage-bde -status", PcName);
                        process.Start();
                    }
                    catch (Exception exceptionText)
                    {
                        await dialogCoordinator.ShowMessageAsync(applicationVM, "BitLocker", exceptionText.ToString()); 
                        return;
                    }
                }
            }
        }

        public async Task RunSpoolReset()
        {
            if (await PingValidation())
            {
                try
                {
                    await Task.Run(async () =>
            {
                using (ServiceController serviceC = new ServiceController("Spooler", PcName))
                {
                    if (serviceC.Status == ServiceControllerStatus.Stopped || serviceC.Status == ServiceControllerStatus.Paused)
                    {
                        serviceC.Start();
                    }
                    else
                    {
                        serviceC.Stop();
                        await Task.Delay(3000);
                        serviceC.Start();
                    }
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "Spooler", "Uruchomiono usługę.");
                    return;
                }
            });
                }
                catch (Exception ex)
                {
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "Spooler", ex.ToString()); 
                    return;
                }
            }
        }

        private async Task<bool> PingValidation()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PcName))
                {
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "Błąd", "Podaj adres stacji.");
                    return false;
                }
                else
                {
                    PcName = PcName.TrimStart().TrimEnd();

                    using (Ping ping = new Ping())
                    {
                        PingReply pingReply = await ping.SendPingAsync(PcName);
                        if(pingReply.Status == IPStatus.Success)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Ping", ex.ToString());
                return false;
            }
        }

        private async Task<bool> PsExecValidation()
        {
            try
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\Resources\PsExec64.exe"))
                { return true; }
                else
                { 
                    File.WriteAllBytes(Directory.GetCurrentDirectory(), Resources.PsExec64);
                    await dialogCoordinator.ShowMessageAsync(applicationVM, "PsExec", "Zainstalowano PsExec.");
                    return true; 
                }
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Bład - PsExec", ex.ToString()); 
                return false;
            }
        }
    }
}
