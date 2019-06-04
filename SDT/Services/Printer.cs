using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SDT.Services
{
    class Printer
    {
        private readonly MainWindow _mainWindow;

        public Printer(MainWindow MainWindow)
        {
            _mainWindow = MainWindow;
        }

        public async void PingPrinter()
        {
            _mainWindow.printerProgressBar.Visibility = Visibility.Visible;
            var address = _mainWindow.printerIpTextBox.Text;

            if (IPAddress.TryParse(address, out IPAddress ipaddress))
            {
                var pingAnswer = await Task.Run(() =>
                {
                    Ping ping = new Ping();
                    return ping.Send(ipaddress);
                });
                if (pingAnswer.Status == IPStatus.Success)
                {
                    _mainWindow.printerIpTextBox.Foreground = Brushes.ForestGreen;
                    _mainWindow.printerProgressBar.Visibility = Visibility.Hidden;
                }
                else
                {
                    _mainWindow.printerIpTextBox.Foreground = Brushes.Red;
                    _mainWindow.printerProgressBar.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                _mainWindow.printerIpTextBox.Foreground = Brushes.Red;
                _mainWindow.printerProgressBar.Visibility = Visibility.Hidden;
                return;
            }
        }

        public void CheckPrinter()
        {
            _mainWindow.printerProgressBar.Visibility = Visibility.Visible;

            if (!File.Exists(@"\\Dsb192\sdt_resources$\Printers\Drukarki.csv"))
            {
                _mainWindow.popupText.Text = "Brak dostępu do danych";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                _mainWindow.printerProgressBar.Visibility = Visibility.Hidden;
            }
            else
            {
                try
                {
                    TextFieldParser _fieldParser = new TextFieldParser(@"\\Dsb192\sdt_resources$\Printers\Drukarki.csv");
                    string currentLine;
                    _fieldParser.TextFieldType = FieldType.Delimited;
                    _fieldParser.Delimiters = new string[] { "|" };
                    _fieldParser.TrimWhiteSpace = true;
                    bool printerFound = false;

                    do
                    {
                        currentLine = _fieldParser.ReadLine();
                        if (currentLine != null)
                        {
                            string file = currentLine;

                            string serialNumber = file.Split('|')[0].Trim();
                            string adresIP = file.Split('|')[1].Trim();
                            string model = file.Split('|')[2].Trim();
                            string status = file.Split('|')[3].Trim();

                            string address = file.Split('|')[4].Trim();
                            string floor = file.Split('|')[8].Trim();
                            string room = file.Split('|')[9].Trim();
                            string addressOther = file.Split('|')[10].Trim();

                            string lan = file.Split('|')[11].Trim();
                            string guardian = file.Split('|')[12].Trim();
                            string server = file.Split('|')[13].Trim();
                            string queue = file.Split('|')[14].Trim();
                            string share = file.Split('|')[15].Trim();
                            string locationType = file.Split('|')[16].Trim();

                            if (adresIP == _mainWindow.printerTextBox.Text || serialNumber == _mainWindow.printerTextBox.Text)
                            {
                                printerFound = true;

                                _mainWindow.printerNsTextBox.Text = serialNumber;
                                _mainWindow.printerIpTextBox.Text = adresIP;
                                _mainWindow.printerModelTextBox.Text = model;
                                _mainWindow.printerServerTextBox.Text = server;
                                _mainWindow.printerStatusTextBox.Text = status;
                                _mainWindow.printerGuardianTextBox.Text = guardian;
                                _mainWindow.printerShareTextBox.Text = share;
                                _mainWindow.printerConnectionTextBox.Text = lan;

                                if(locationType.Contains("PTK-SALON") || locationType.Contains("TP-SALON") || locationType.Contains("TP-SALON7"))
                                { _mainWindow.salonCheckBox.IsChecked = true; }
                                else
                                { _mainWindow.salonCheckBox.IsChecked = false; }

                                if(server.Contains("oprint"))
                                { _mainWindow.terminalCheckBox.IsChecked = true; }
                                else
                                { _mainWindow.terminalCheckBox.IsChecked = false; }

                                _mainWindow.printerLocationTextBox.Text = address
                                    + Environment.NewLine + "Piętro: " + floor
                                    + Environment.NewLine + "Pokój: " + room
                                    + Environment.NewLine + "Inne: " + addressOther;

                                _mainWindow.printerProgressBar.Visibility = Visibility.Hidden;
                                return;
                            }
                        }
                    } while (currentLine != null);
                    if (printerFound == false)
                    {
                        _mainWindow.popupText.Text = "Brak drukarki o podanym IP lub NS";
                        _mainWindow.mainPopupBox.IsPopupOpen = true;
                        _mainWindow.printerProgressBar.Visibility = Visibility.Hidden;
                        return;
                    }
                }
                catch (Exception e)
                {
                    _mainWindow.popupText.Text = e.Message;
                    _mainWindow.mainPopupBox.IsPopupOpen = true;
                    _mainWindow.printerProgressBar.Visibility = Visibility.Hidden;
                    return;
                }
            }
        }
    }
}
