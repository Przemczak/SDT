using System;
using System.Windows;
using System.Windows.Forms;

namespace SDT.Helpers
{
    public static class TrayIcon
    {
        public static NotifyIcon _notifyIcon;

        /// <summary>
        /// Load Icon in Tray
        /// </summary>
        public static void Tray(MainWindow mainWindow)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            _notifyIcon = notifyIcon;

            notifyIcon.Icon = Properties.Resources.icons8_maintenance_64_W6f_icon;
            notifyIcon.Text = "SDT - Service Desk Tool";
            notifyIcon.Visible = true;
            
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Otwórz", null, Open_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Zamknij", null, Close_Click);

            void Open_Click(object sender, EventArgs e)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
            }

            void Close_Click(object sender, EventArgs e)
            {
                mainWindow.MetroWindow_Closing(sender, null);
            }

            notifyIcon.DoubleClick += delegate (object sender, EventArgs args)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
            };
        }

        /// <summary>
        /// Close tray icon when closing app
        /// </summary>
        public static void Disposeico()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Icon.Dispose();
        }

        /// <summary>
        /// BalloonTip if ping true (Online)
        /// </summary>
        public static void BalloonPingOnline(string pcaddress)
        {
            _notifyIcon.BalloonTipTitle = pcaddress + " - Online!";
            _notifyIcon.BalloonTipText = "Adres odpowiada w sieci.";
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.ShowBalloonTip(7000);
        }

        /// <summary>
        /// BalloonTip if ping false (Offline)
        /// </summary>
        public static void BalloonPingOffline(string pcaddress)
        {
            _notifyIcon.BalloonTipTitle = pcaddress + " - Offline!";
            _notifyIcon.BalloonTipText = "Adres nie odpowiada w sieci.";
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
            _notifyIcon.ShowBalloonTip(7000);
        }
    }
}
