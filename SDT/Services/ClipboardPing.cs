using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SDT.Services
{
    public sealed class ClipboardPing
    {
        public static ClipboardPing _instance;
        private readonly MainWindow _metroWindow;

        public ClipboardPing(MainWindow MetroWindow)
        {
            _metroWindow = MetroWindow;
        }

        public static ClipboardPing Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClipboardPing();
                }
                return _instance;
            }
        }

        private ClipboardPing()
        {
        }

        /// <summary>
        /// PING from Clipboard (Netbios address)
        /// return int: 1 = online = 2, offline, 3 = error
        /// </summary>
        public async Task<int> ClipboardNetbiosPing(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            try
            {
                string netbios = TextBox_PCin.Text;
                WaitBarPC.Visibility = Visibility.Visible;

                var pingAnswer = await Task.Run(() =>
                {
                    Ping ping = new Ping();
                    return ping.Send(netbios);
                });

                if (pingAnswer.Status == IPStatus.Success)
                {
                    TextBox_PCin.Background = new SolidColorBrush(Colors.Green);
                    WaitBarPC.Visibility = Visibility.Hidden;
                    return 1;
                }
                else
                {
                    TextBox_PCin.Background = new SolidColorBrush(Colors.Red);
                    WaitBarPC.Visibility = Visibility.Hidden;
                    return 2;
                }
            }
            catch (Exception e)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", e.Message);
                WaitBarPC.Visibility = Visibility.Hidden;
                return 3;
            }
        }

        /// <summary>
        /// Address IP parse + PING from Clipboard
        /// return int: 1 = online = 2, offline, 3 = error
        /// </summary>
        public async Task<int> ClipboardIPPing(TextBox TextBox_PCin, ProgressBar WaitBarPC)
        {
            try
            {
                WaitBarPC.Visibility = Visibility.Visible;

                if (IPAddress.TryParse(TextBox_PCin.Text, out IPAddress ipaddress))
                {
                    var pingAnswer = await Task.Run(() =>
                    {
                        Ping ping = new Ping();
                        return ping.Send(ipaddress);
                    });

                    if (pingAnswer.Status == IPStatus.Success)
                    {
                        TextBox_PCin.Background = new SolidColorBrush(Colors.Green);
                        WaitBarPC.Visibility = Visibility.Hidden;
                        return 1;
                    }
                    else
                    {
                        TextBox_PCin.Background = new SolidColorBrush(Colors.Red);
                        WaitBarPC.Visibility = Visibility.Hidden;
                        return 2;
                    }
                }
                else
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        await window.ShowMessageAsync("Błąd!", "Błedy zakres adresacji IPv4");
                    WaitBarPC.Visibility = Visibility.Hidden;
                    return 3;
                }

            }
            catch (Exception e)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", e.Message);
                WaitBarPC.Visibility = Visibility.Hidden;
                return 3;
            }
        }
    }
}
    