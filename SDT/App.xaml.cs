using SDT.Helpers;
using SDT.Services;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace SDT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly TrayIconService trayIconService;

        public App()
        {
            trayIconService = new TrayIconService(this);
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            //if (!IsRunAsAdministrator())
            //{
            //    var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

            //    processInfo.UseShellExecute = true;
            //    processInfo.Verb = "runas";

            //    try
            //    {
            //        Process.Start(processInfo);
            //    }
            //    catch (Exception)
            //    {
            //        MessageBox.Show("Aplikacja musi być uruchomiona jako administrator.");
            //    }
            //    Current.Shutdown();
            //}

            //bool IsRunAsAdministrator()
            //{
            //    var wi = WindowsIdentity.GetCurrent();
            //    var wp = new WindowsPrincipal(wi);

            //    return wp.IsInRole(WindowsBuiltInRole.Administrator);
            //}
            //base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIconService.DisposeTrayIcon();
            base.OnExit(e);
        }
    }
}
