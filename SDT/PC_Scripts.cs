using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SDT
{
    class PC_Scripts
    {
        private readonly MainWindow _MetroWindow;

        public PC_Scripts(MainWindow MetroWindow)
        {
            _MetroWindow = MetroWindow;
        }

        PC pec = new PC();

        /// <summary>
        /// PsExec remote GPupdate
        /// </summary>
        public async void GPUpdate(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            var pingcheck = await pec.Ping(TextBox_PCin, WaitBarPC);
            if (pingcheck)
            {
                var psexcheck = pec.PsExecCheck();
                if (psexcheck)
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                        process.StartInfo.Arguments = String.Format(@"/k ""C:\My Program Files\PsExec64.exe"" \\{0} gpupdate /force", TextBox_PCin.Text);
                        process.EnableRaisingEvents = true;
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                        if (window != null)
                            await window.ShowMessageAsync("Błąd!", e.Message);
                        return;
                    }
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
        /// PsExec remote Bitlocker status
        /// </summary>
        public async void BitLocker(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            var pingcheck = await pec.Ping(TextBox_PCin, WaitBarPC);
            if (pingcheck)
            {
                var psexcheck = pec.PsExecCheck();
                if (psexcheck)
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                        process.StartInfo.Arguments = String.Format(@"/k ""C:\My Program Files\PsExec64.exe"" \\{0} manage-bde -status", TextBox_PCin.Text);
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                        if (window != null)
                            await window.ShowMessageAsync("Błąd!", e.Message);
                        return;
                    }
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
    }
}
