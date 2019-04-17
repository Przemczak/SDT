using MaterialDesignThemes.Wpf;
using System;
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
        private readonly MainWindow _mainWindow;

        public ClipboardPing(MainWindow MainWindow)
        {
            _mainWindow = MainWindow;
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
        public async Task<int> ClipboardNetbiosPing(TextBox PcTextBox, ProgressBar pcProgressBar)
        {
            try
            {
                string netbios = PcTextBox.Text;
                pcProgressBar.Visibility = Visibility.Visible;

                var pingAnswer = await Task.Run(() =>
                {
                    Ping ping = new Ping();
                    return ping.Send(netbios);
                });

                if (pingAnswer.Status == IPStatus.Success)
                {
                    PcTextBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF64DD17");
                    pcProgressBar.Visibility = Visibility.Hidden;
                    return 1;
                }
                else
                {
                    PcTextBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFEF5350");
                    pcProgressBar.Visibility = Visibility.Hidden;
                    return 2;
                }
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                pcProgressBar.Visibility = Visibility.Hidden;
                return 3;
            }
        }

        /// <summary>
        /// Address IP parse + PING from Clipboard
        /// return int: 1 = online = 2, offline, 3 = error
        /// </summary>
        public async Task<int> ClipboardIPPing(TextBox PcTextBox, ProgressBar pcProgressBar)
        {
            try
            {
                pcProgressBar.Visibility = Visibility.Visible;

                if (IPAddress.TryParse(PcTextBox.Text, out IPAddress ipaddress))
                {
                    var pingAnswer = await Task.Run(() =>
                    {
                        Ping ping = new Ping();
                        return ping.Send(ipaddress);
                    });

                    if (pingAnswer.Status == IPStatus.Success)
                    {
                        PcTextBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF64DD17");
                        pcProgressBar.Visibility = Visibility.Hidden;
                        return 1;
                    }
                    else if (pingAnswer.Status == IPStatus.Success || pingAnswer.Status == IPStatus.TimedOut)
                    {
                        PcTextBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFEF5350");
                        pcProgressBar.Visibility = Visibility.Hidden;
                        return 2;
                    }
                    else
                    {
                        PcTextBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFEF5350");
                        pcProgressBar.Visibility = Visibility.Hidden;
                        return 2;
                    }
                }
                else
                {
                    var window = await DialogHost.Show("Błąd!", "Błedy zakres adresacji IPv4");
                    pcProgressBar.Visibility = Visibility.Hidden;
                    return 3;
                }

            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                pcProgressBar.Visibility = Visibility.Hidden;
                return 3;
            }
        }
    }
}
    