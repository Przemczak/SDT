using System;
using System.Windows;
using System.Windows.Forms;

namespace SDT.Services
{
    public class TrayIconService
    {
        private NotifyIcon notifyIcon;
        private readonly App app;

        public TrayIconService(App App)
        {
            app = App;

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.App_icon;
            notifyIcon.Text = "SDT - Service Desk Tool";
            notifyIcon.Visible = true;

            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Otwórz", null, AppShow_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Zamknij", null, AppClose_Click);
            notifyIcon.DoubleClick += AppShow_Click;
        }

        public void DisposeTrayIcon()
        {
            notifyIcon.Dispose();
        }

        void AppClose_Click(object sender, EventArgs e)
        {
            app.Shutdown();
        }

        void AppShow_Click(object sender, EventArgs e)
        {
            app.MainWindow.Show();
            app.MainWindow.WindowState = WindowState.Normal;
        }
    }   
}
