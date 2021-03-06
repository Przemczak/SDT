﻿using SDT.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SDT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Services.User _user;
        private readonly Services.PC _pc;
        private readonly Services.Printer _printer;
        private readonly MainHelpers _mainHelpers;
        private readonly ClipboardHelpers _clipboardhelpers;
        private readonly Updater _updater;

        private Pages.PasswordWindow _passwordWindow = null;
        public string iDNumberPaste = null;

        public MainWindow()
        {
            InitializeComponent();

            ///Load Settings
            Settings _settings = new Settings();

            /// Load tray icon
            TrayIcon.Tray(this);

            /// Load clipboard listening and load Regex
            StartListeningForClipboardChange();

            /// Create instance of methods
            _user = new Services.User(this);
            _pc = new Services.PC(this);
            _printer = new Services.Printer(this);
            _mainHelpers = new MainHelpers(this);
            _clipboardhelpers = new ClipboardHelpers(this);
            _updater = new Updater(this);

            /// Check updates
            _updater.CheckUpdate();
        }

        /// <summary>
        /// Custom TAB Control
        /// </summary>
        private void UserGridButton_Click(object sender, RoutedEventArgs e)
        {
            userGrid.Visibility = Visibility.Visible;
            pcGrid.Visibility = Visibility.Hidden;
            printerGrid.Visibility = Visibility.Hidden;
        }

        private void PcGridButton_Click(object sender, RoutedEventArgs e)
        {
            userGrid.Visibility = Visibility.Hidden;
            pcGrid.Visibility = Visibility.Visible;
            printerGrid.Visibility = Visibility.Hidden;
        }

        private void PrinterGridButton_Click(object sender, RoutedEventArgs e)
        {
            userGrid.Visibility = Visibility.Hidden;
            pcGrid.Visibility = Visibility.Hidden;
            printerGrid.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Open password Window
        /// </summary>
        private void PassGenButton_Click(object sender, RoutedEventArgs e)
        {
            if (_passwordWindow == null)
            {
                _passwordWindow = new Pages.PasswordWindow();
                _passwordWindow.Closed += PasswordWindow_Closed;
                _passwordWindow.Owner = Application.Current.MainWindow;
                _passwordWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _passwordWindow.Show();
            }
            _passwordWindow.Show();
        }

        private void PasswordWindow_Closed(object sender, System.EventArgs e)
        {
            _passwordWindow = null;
        }

        /// <summary>
        /// Moving borderless window event
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Exit button
        /// </summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Window_Closing();
        }

        public void Window_Closing()
        {
            /// Dispose tray icon
            TrayIcon.Disposeico();

            /// Close all opened pages on exit
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Minimize button
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Open Info Dialog
        /// </summary>
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Pages.Info _info = new Pages.Info
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            _info.ShowDialog();
        }

        /// <summary>
        /// Open Settings page
        /// </summary>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Pages.Settings _settings = new Pages.Settings
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            _settings.ShowDialog();
        }

        /// <summary>
        /// Start Listening from Clipboard
        /// </summary>
        private void StartListeningForClipboardChange()
        {
            Services.ClipboardNotification.ClipboardUpdate += (o, e) =>
            {
                _clipboardhelpers.OnClipboardChanged(Clipboard.GetText());
            };
        }

        /// <summary>
        /// Ping and Tray Balloon from Clipboard
        /// </summary>
        private void PcTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _clipboardhelpers.ClipboardPing();
        }

        /// <summary>
        /// Update
        /// </summary>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            updatePopupBox.IsPopupOpen = true;
        }

        private void UpdateYesButton_Click(object sender, RoutedEventArgs e)
        {
            _updater.InstallUpdate();
        }

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            bool update = _updater.CheckUpdateOnDemand();

            if (update)
            {
                updatePopupBox.IsPopupOpen = true;
            }
        }

        /// <summary>
        /// userGrid - Check User Login
        /// </summary>  
        private void UserCheckButton_Click(object sender, RoutedEventArgs e)
        {
            _mainHelpers.ClearUser();

            userLoginTextBox.Text = userLoginTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxUser();
            if (CS)
                _user.CheckAD();
        }

        /// <summary>
        /// userGrid - Ping
        /// </summary>
        private async void PcPingButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                await _pc.Ping();
        }

        /// <summary>
        /// userGrid - Ping cmd with -t
        /// </summary>
        private void PcPingTButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.PingT();
        }

        /// <summary>
        /// userGrid - Ports
        /// </summary>
        private async void PcPortsButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                await _pc.PortCheck();
        }

        /// <summary>
        /// userGrid - PC Info
        /// </summary>
        private void PcInfoButton_Click(object sender, RoutedEventArgs e)
        {
            _mainHelpers.ClearPcInfo();

            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.PCinfo();
        }

        /// <summary>
        /// userGrid - RCV
        /// </summary>
        private void RcvButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.Cmrcviewer();
        }

        /// <summary>
        /// pcGrid - Open Installer
        /// </summary>
        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool checknull = _mainHelpers.CheckTextBoxPc();
            if (checknull)
            {
                var pscheck = _pc.PsExecCheck();
                if (pscheck)
                {
                    Pages.Installer _installer = new Pages.Installer(this)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    _installer.Show();
                }
            }
        }

        /// <summary>
        /// pcGrid - Sharing
        /// </summary>
        private void SharingButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.Sharing();
        }

        /// <summary>
        /// userGrid - PsExec
        /// </summary>
        private async void PsexecButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                await _pc.PsExecRUN();
        }

        /// <summary>
        /// userGrid - Scripts
        /// </summary>
        private void ScriptsButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void GpUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.GPUpdate();
        }

        private void BitLockerStatusButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.BitLocker();
        }

        private void IeFixButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.IEFix();
        }

        private void SpoolResetButton_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.SpoolerReset();
        }

        private void RcServicesStart_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.ServicesStart();
        }

        private void CertificatesFix_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.CertificatesFix();
        }

        private void TMPRepair_Click(object sender, RoutedEventArgs e)
        {
            pcTextBox.Text = pcTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPc();
            if (CS)
                _pc.TPMRepair();
        }

        /// <summary>
        /// printerGrid - Check printer
        /// </summary>
        private void PrinterButton_Click(object sender, RoutedEventArgs e)
        {
            _mainHelpers.ClearPrinter();

            printerTextBox.Text = printerTextBox.Text.TrimStart().TrimEnd();

            bool CS = _mainHelpers.CheckTextBoxPrinter();
            if (CS)
                _printer.CheckPrinter();
        }

        private void PrinterIpTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _printer.PingPrinter();
        }

        private void PingPrinterButton_Click(object sender, RoutedEventArgs e)
        {
            _printer.PingPrinter();
        }

        private void IdNumberPasteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string idNumber = idNumberPasteTextBox.Text;
            var IDList = idNumber.Select(x => Convert.ToInt32(x.ToString())).ToArray();
            var IDlistLast = idNumber.Skip(6).Take(5).ToArray();

            if (_passwordWindow == null)
            {
                _passwordWindow = new Pages.PasswordWindow();
                _passwordWindow.Closed += PasswordWindow_Closed;
                _passwordWindow.Owner = Application.Current.MainWindow;
                _passwordWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _passwordWindow.Show();
                _passwordWindow.Activate();
            }
            else
            {
                _passwordWindow.Activate();
            }
            _passwordWindow.idNumberTextBox.Text = idNumberPasteTextBox.Text;
            


            var random = new Random();
            switch (random.Next(1, 10))
            {
                case 1:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[0].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "7";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[1].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "8";
                    break;

                case 2:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[0].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "7";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[2].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "9";
                    break;

                case 3:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[0].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "7";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[3].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "10";
                    break;

                case 4:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[0].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "7";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[4].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "11";
                    break;

                case 5:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[1].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "8";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[2].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "9";
                    break;

                case 6:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[1].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "8";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[3].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "10";
                    break;

                case 7:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[1].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "8";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[4].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "11";
                    break;

                case 8:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[2].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "9";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[3].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "10";
                    break;

                case 9:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[2].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "9";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[4].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "11";
                    break;

                case 10:
                    _passwordWindow.firstIDNumberTextBox.Text = IDlistLast[3].ToString();
                    _passwordWindow.firstIDNumberOrderTextBox.Text = "10";

                    _passwordWindow.secondIDNumberTextBox.Text = IDlistLast[4].ToString();
                    _passwordWindow.secondIDNumberOrderTextBox.Text = "11";
                    break;
  
            }
        }
    }
}
