using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SDT.Helpers
{
    class Updater
    {
        private readonly MainWindow _mainWindow;

        public Updater(MainWindow MainWindow)
        {
            _mainWindow = MainWindow;
        }

        WebClient _webClient = new WebClient();

        public async void CheckUpdate()
        {
            try
            {
                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(5000);
                });
                if (!_webClient.DownloadString(@"\\Dsb192\sdt_resources$\app_version.txt").Contains("1.5.3.0"))
                {
                    _mainWindow.updateButton.Visibility = Visibility.Visible;
                    _mainWindow.checkUpdateButton.Visibility = Visibility.Hidden;

                    _mainWindow.updatePopupBox.IsPopupOpen = true;
                }
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        public bool CheckUpdateOnDemand()
        {
            if (!_webClient.DownloadString(@"\\Dsb192\sdt_resources$\app_version.txt").Contains("1.5.3.0"))
            {
                _mainWindow.updateButton.Visibility = Visibility.Visible;
                _mainWindow.checkUpdateButton.Visibility = Visibility.Hidden;
                return true;
            }
            else
            {
                _mainWindow.popupText.Text = "Brak dostępnych aktualizacji.";
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return false;
            }
        }

        public async void InstallUpdate()
        {
            try
            {
                _mainWindow.popupText.Text = "Pobieranie...";
                _mainWindow.mainPopupBox.IsPopupOpen = true;

                await Task.Run(() =>
                {
                    _webClient.DownloadFile(@"\\Dsb192\SDT_resources$\Update\SDTUpdate.msi", @"C:\\TEMP\SDTUpdate.msi");
                    System.Threading.Thread.Sleep(3000);
                });
                Process.Start(@"C:\\TEMP\SDTUpdate.msi");
                _mainWindow.Window_Closing();
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }
    }
}
