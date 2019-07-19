using System.Text.RegularExpressions;

namespace SDT.Helpers
{
    public class ClipboardHelpers
    {
        private readonly MainWindow _mainWindow;

        public ClipboardHelpers(MainWindow MainWindows)
        {
            _mainWindow = MainWindows;
        }

        Regex REGEX_TP = new Regex(@"(TP[a-zA-Z0-9]{12})", RegexOptions.Compiled);
        Regex REGEX_OPL = new Regex(@"(OPL[a-zA-Z0-9]{12})", RegexOptions.Compiled);
        Regex REGEX_FRA = new Regex(@"(FRA[a-zA-Z0-9]{10})", RegexOptions.Compiled);
        Regex REGEX_WTG = new Regex(@"(WTG[a-zA-Z0-9]{12})", RegexOptions.Compiled);
        Regex REGEX_BHD = new Regex(@"(BHD[a-zA-Z0-9]{11})", RegexOptions.Compiled);
        Regex REGEX_IP = new Regex(@"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})", RegexOptions.Compiled);
        Regex REGEX_ID = new Regex(@"(^\d{11}$)", RegexOptions.Compiled);

        void Process_TP(string value)
        {
            _mainWindow.pcTextBox.Text = value;
        }
        void Process_OPL(string value)
        {
            _mainWindow.pcTextBox.Text = value;
        }
        void Process_FRA(string value)
        {
            _mainWindow.pcTextBox.Text = value;
        }
        void Process_WTG(string value)
        {
            _mainWindow.pcTextBox.Text = value;
        }
        void Process_BHD(string value)
        {
            _mainWindow.pcTextBox.Text = value;
        }
        void Process_IP(string value)
        {
            _mainWindow.pcTextBox.Text = value;
        }
        void Process_ID(string value)
        {
            _mainWindow.idNumberPasteTextBox.Text = value;
        }

        /// <summary>
        /// Change in Clipboard
        /// </summary>
        public void OnClipboardChanged(string clipboardContents)
        {
            {
                Match m = REGEX_TP.Match(clipboardContents);
                if (m.Success)
                {
                    Process_TP(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_OPL.Match(clipboardContents);
                if (m.Success)
                {
                    Process_OPL(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_FRA.Match(clipboardContents);
                if (m.Success)
                {
                    Process_FRA(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_WTG.Match(clipboardContents);
                if (m.Success)
                {
                    Process_WTG(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_BHD.Match(clipboardContents);
                if (m.Success)
                {
                    Process_BHD(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_IP.Match(clipboardContents);
                if (m.Success)
                {
                    Process_IP(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_ID.Match(clipboardContents);
                if (m.Success)
                {
                    Process_ID(m.Groups[1].Value);
                }
            }
        }

        /// <summary>
        /// Clipboard to Textbox + Ping + TrayBalloon
        /// </summary>
        public async void ClipboardPing()
        {
            try
            {
                string pcAddress = _mainWindow.pcTextBox.Text;

                if (REGEX_TP.IsMatch(_mainWindow.pcTextBox.Text))
                {
                    int pingcheck = await Services.ClipboardPing.Instance.ClipboardNetbiosPing(_mainWindow.pcTextBox, _mainWindow.pcProgressBar);

                    if (pingcheck == 1) { TrayIcon.BalloonPingOnline(pcAddress); }
                    else if (pingcheck == 2) { TrayIcon.BalloonPingOffline(pcAddress); }
                }

                else if (REGEX_OPL.IsMatch(_mainWindow.pcTextBox.Text))
                {
                    int pingcheck = await Services.ClipboardPing.Instance.ClipboardNetbiosPing(_mainWindow.pcTextBox, _mainWindow.pcProgressBar);

                    if (pingcheck == 1) { TrayIcon.BalloonPingOnline(pcAddress); }
                    else if (pingcheck == 2) { TrayIcon.BalloonPingOffline(pcAddress); }
                }

                else if (REGEX_FRA.IsMatch(_mainWindow.pcTextBox.Text))
                {
                    int pingcheck = await Services.ClipboardPing.Instance.ClipboardNetbiosPing(_mainWindow.pcTextBox, _mainWindow.pcProgressBar);

                    if (pingcheck == 1) { TrayIcon.BalloonPingOnline(pcAddress); }
                    else if (pingcheck == 2) { TrayIcon.BalloonPingOffline(pcAddress); }
                }

                else if (REGEX_WTG.IsMatch(_mainWindow.pcTextBox.Text))
                {
                    int pingcheck = await Services.ClipboardPing.Instance.ClipboardNetbiosPing(_mainWindow.pcTextBox, _mainWindow.pcProgressBar);

                    if (pingcheck == 1) { TrayIcon.BalloonPingOnline(pcAddress); }
                    else if (pingcheck == 2) { TrayIcon.BalloonPingOffline(pcAddress); }
                }

                else if (REGEX_BHD.IsMatch(_mainWindow.pcTextBox.Text))
                {
                    int pingcheck = await Services.ClipboardPing.Instance.ClipboardNetbiosPing(_mainWindow.pcTextBox, _mainWindow.pcProgressBar);

                    if (pingcheck == 1) { TrayIcon.BalloonPingOnline(pcAddress); }
                    else if (pingcheck == 2) { TrayIcon.BalloonPingOffline(pcAddress); }
                }

                else if (REGEX_IP.IsMatch(_mainWindow.pcTextBox.Text))
                {
                    int pingcheck = await Services.ClipboardPing.Instance.ClipboardIPPing(_mainWindow.pcTextBox, _mainWindow.pcProgressBar);

                    if (pingcheck == 1) { TrayIcon.BalloonPingOnline(pcAddress); }
                    else if (pingcheck == 2) { TrayIcon.BalloonPingOffline(pcAddress); }
                }
            }
            catch (System.Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
            }
        }
    }
}
