using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
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
        /// Check computer ping
        /// </summary>
        public async Task Ping(TextBox TextBox_PCin, ProgressBar WaitBar)
        {
            try
            {
                WaitBar.Visibility = Visibility.Visible;

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

                WaitBar.IsEnabled = false;

                WaitBar.Visibility = Visibility.Hidden;
            }
            catch (Exception e)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", "Błedy adres stacji.");
                WaitBar.Visibility = Visibility.Hidden;
                return;
            }
        }

        /// <summary>
        /// Run SCCM - Sharing
        /// </summary>
        public void Sharing(TextBox TextBox_PCin)
        {

        }

    }
}
