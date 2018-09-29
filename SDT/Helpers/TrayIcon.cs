using System;
using System.Windows;
using System.Windows.Forms;

namespace SDT.Helpers
{
    public static class TrayIcon
    {
        public static NotifyIcon _ni;

        /// <summary>
        /// Load Icon in Tray
        /// </summary>
        public static void Tray(MainWindow mainWindow)
        {
            NotifyIcon ni = new NotifyIcon();
            _ni = ni;

            ni.Icon = Properties.Resources.icons8_maintenance_64_W6f_icon;
            ni.Text = "SDT - Service Desk Tool";
            ni.Visible = true;
            
            ni.ContextMenuStrip = new ContextMenuStrip();
            ni.ContextMenuStrip.Items.Add("Otwórz", null, Open_Click);
            ni.ContextMenuStrip.Items.Add("Zamknij", null, Close_Click);

            void Open_Click(object sender, EventArgs e)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
            }

            void Close_Click(object sender, EventArgs e)
            {
                mainWindow.MetroWindow_Closing(sender, null);
            }

            ni.DoubleClick += delegate (object sender, EventArgs args)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
            };
        }

        /// <summary>
        /// Close tray icon when closing app
        /// </summary>
        public static void disposeico()
        {
            _ni.Visible = false;
            _ni.Icon.Dispose();
        }

        /// <summary>
        /// BalloonTip if ping true (Online)
        /// </summary>
        public static void BalloonPingOnline(string pcaddress)
        {
            _ni.BalloonTipTitle = pcaddress + " - Online!";
            _ni.BalloonTipText = "Adres " + pcaddress + " odpowiada w sieci.";
            _ni.ShowBalloonTip(7000);
              
        }

        /// <summary>
        /// BalloonTip if ping false (Offline)
        /// </summary>
        public static void BalloonPingOffline(string pcaddress)
        {
            _ni.BalloonTipTitle = pcaddress + " - Offline!";
            _ni.BalloonTipText = "Adres " + pcaddress + " nie odpowiada w sieci.";
            _ni.ShowBalloonTip(7000);
        }
    }
}
