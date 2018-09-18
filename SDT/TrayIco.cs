using System;
using System.Drawing;
using System.Windows.Forms;

namespace SDT.Settings
{
    public class TrayIco
    {
        private MainWindow MetroWindow;

        public TrayIco(MainWindow MetroWindow)
        {
            this.MetroWindow = MetroWindow;

            NotifyIcon ni = new NotifyIcon();
            ni.Icon = SystemIcons.Application;
            ni.Text = "SDT - Service Desk Tool";
            ni.Visible = true;
            
            ni.DoubleClick += delegate (object sender, EventArgs args)
            {
                this.MetroWindow.WindowState = System.Windows.WindowState.Normal;
            };
        }
    }
}
