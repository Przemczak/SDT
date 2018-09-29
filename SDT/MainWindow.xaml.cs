using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SDT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Regex REGEX_TP = new Regex(@"(TP[a-zA-Z0-9]{12})", RegexOptions.Compiled);
        Regex REGEX_OPL = new Regex(@"(OPL[a-zA-Z0-9]{12})", RegexOptions.Compiled);
        Regex REGEX_FRA = new Regex(@"(FRA[a-zA-Z0-9]{10})", RegexOptions.Compiled);
        Regex REGEX_WTG = new Regex(@"(WTG[a-zA-Z0-9]{12})", RegexOptions.Compiled);
        Regex REGEX_BHD = new Regex(@"(BHD[a-zA-Z0-9]{11})", RegexOptions.Compiled);
        Regex REGEX_IP = new Regex(@"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})", RegexOptions.Compiled);

        public MainWindow()
        {
            InitializeComponent();
            StartListeningForClipboardChange();
            ThemeSettingsLoad();
            Helpers.TrayIcon.Tray(this);
        }

        /// <summary>
        /// Clear Textboxes and Checkboxes in User Tab
        /// </summary>
        private void ClearBoxes()
        {
            foreach (Control c in Grid_UserAD.Children)
            {
                if (c is TextBox && c != null) { ((TextBox)c).Text = string.Empty; }
            }

            foreach (Control c in Grid_UserUser.Children)
            {
                if (c is TextBox && c != null) { ((TextBox)c).Text = string.Empty; }
            }

            foreach (Control c in Grid_UserMail.Children)
            {
                if (c is TextBox && c != null) { ((TextBox)c).Text = string.Empty; }
            }

            foreach (Control c in Grid_UserMailBPTP.Children)
            {
                if (c is TextBox && c != null) { ((TextBox)c).Text = string.Empty; }
            }

            foreach (Control c in Grid_UserDev.Children)
            {
                if (c is TextBox && c != null) { ((TextBox)c).Text = string.Empty; }
            }

            foreach (Control c in Grid_UserMailC.Children)
            {
                if (c is CheckBox && c != null) { ((CheckBox)c).IsChecked = false; ((CheckBox)c).ClearValue(CheckBox.ForegroundProperty); }
            }

            foreach (Control c in Grid_UserAccess.Children)
            {
                if (c is CheckBox && c != null) { ((CheckBox)c).IsChecked = false; ((CheckBox)c).ClearValue(CheckBox.ForegroundProperty); }
            }

            foreach (Control c in Grid_UserDev.Children)
            {
                if (c is CheckBox && c != null) { ((CheckBox)c).IsChecked = false; }
            }

            foreach (Control c in Grid_UserBYOD.Children)
            {
                if (c is CheckBox && c != null) { ((CheckBox)c).IsChecked = false; }
            }

            foreach (Control c in Grid_UserAirWatch.Children)
            {
                if (c is CheckBox && c != null) { ((CheckBox)c).IsChecked = false; }
            }


        }

        public void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Helpers.TrayIcon.disposeico();

            Properties.Settings.Default.Save();

            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void StartListeningForClipboardChange()
        {
            Helpers.ClipboardNotification.ClipboardUpdate += (o, e) =>
            {
               onClipboardChanged(Clipboard.GetText());
            };
        }

        /// <summary>
        /// Open Info page
        /// </summary>
        private void Button_Info_Click(object sender, RoutedEventArgs e)
        {
            Information sh = new Information
            {
                Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            sh.Show();
        }

        /// <summary>
        /// Open Settings page
        /// </summary>
        private void Click_Button_Settings(object sender, RoutedEventArgs e)
        {
            SettingsPage sp = new SettingsPage(this)
            {
                Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            sp.Show();
        }


        /// <summary>
        /// User (User TAB)
        /// </summary>
        private void Button_UserCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearBoxes();

            TextBox_UserLoginIn.Text = string.Join("", TextBox_UserLoginIn.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_UserLoginIn.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj login użytkownika.");
                return;
            }

            Services.User us = new Services.User(this);
            us.CheckAD(TextBox_UserLoginIn, WaitBarUser);
        }


        /// <summary>
        /// PC (PC TAB)
        /// </summary>
        private async void Button_PCping_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }
            Services.PC pec = new Services.PC(this);
            await pec.Ping(TextBox_PCin, WaitBarPC);
        }

        private void Button_Sharing_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }

            Services.PC pec = new Services.PC(this);
            pec.Sharing(TextBox_PCin, WaitBarPC);
        }

        private void Button_RCV_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }

            Services.PC pec = new Services.PC(this);
            pec.Cmrcviewer(TextBox_PCin, WaitBarPC);
        }
        
        private void Button_PCinfo_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }

            Services.PC pec = new Services.PC(this);
            pec.PCinfo(TextBox_PCin, WaitBarPC);
        }

        private void Button_Scripts_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private async void Button_PsExec_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }

            Services.PC pec = new Services.PC(this);
            pec.PsExecRUN(TextBox_PCin, WaitBarPC);
        }

        private async void Button_Insta_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }
            else
            {
                Services.PC pec = new Services.PC();
                var pscheck = pec.PsExecCheck();
                if (pscheck)
                {
                    PC_Installer PI = new PC_Installer(TextBox_PCin)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    PI.Show();
                }
            }
        }

        private async void Button_PCPorts_Click(object sender, RoutedEventArgs e)
        {
            CheckBox_PC135.IsChecked = false;
            CheckBox_PC445.IsChecked = false;
            CheckBox_PC2701.IsChecked = false;
            CheckBox_PC8081.IsChecked = false;

            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }
            else
            {
                Services.PC pec = new Services.PC(this);
                await pec.PortCheck(TextBox_PCin, WaitBarPC);
            }
        }

        private async void Button_PCpingT_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }

            Services.PC pec = new Services.PC(this);
            pec.PingT(TextBox_PCin);
        }


        /// <summary>
        /// Scripts (List in Button_Scripts_Click)
        /// </summary>
        private void Button_GP_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }
            Services.PC_Scripts pecs = new Services.PC_Scripts(this);
            pecs.GPUpdate(TextBox_PCin, WaitBarPC);
        }

        private void Button_BitLocker_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }
            Services.PC_Scripts pecs = new Services.PC_Scripts(this);
            pecs.BitLocker(TextBox_PCin, WaitBarPC);
        }

        private async void Button_IEFix_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                   await window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }
            Services.PC_Scripts pecs = new Services.PC_Scripts(this);
            pecs.IEFix(TextBox_PCin);
        }

        private async void Button_SpoolReset_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    await window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }
            Services.PC_Scripts pecs = new Services.PC_Scripts(this);
            pecs.SpoolerReset(TextBox_PCin, WaitBarPC);
        }

        /// <summary>
        /// Theme load settings
        /// </summary>
        private void ThemeSettingsLoad()
        {
            try
            {
                var newAccent = Properties.Settings.Default.AccentC;
                var newTheme = Properties.Settings.Default.ThemeC;

                ThemeManager.ChangeAppStyle(Application.Current,
                    ThemeManager.GetAccent(newAccent),
                    ThemeManager.GetAppTheme(newTheme));
            }
            catch (Exception ex)
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", ex.Message);
                return;
            }
        }

        /// <summary>
        /// Clipboard results
        /// </summary>
        public void onClipboardChanged(string clipboardContents)
        {
            /*
                1. TPxxxxxxxxxxxx - 14 characters, only alphanumeric
                2. OPLxxxxxxxxxxxx - 15 characters, only alphanumeric
                3. FRAxxxxxxxxxx - 13 characters, only alphanumeric
                4. WTGxxxxxxxxxxxx - 15 characters, only alphanumeric
                5. BHDxxxxxxxxxxx - 14 characters, only alphanumeric
                6. xxx.xxx.xxx.xxx - Address IP. Numerical characters and "."
            */

            {
                Match m = REGEX_TP.Match(clipboardContents);
                if (m.Success)
                {
                    process_TP(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_OPL.Match(clipboardContents);
                if (m.Success)
                {
                    process_OPL(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_FRA.Match(clipboardContents);
                if (m.Success)
                {
                    process_FRA(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_WTG.Match(clipboardContents);
                if (m.Success)
                {
                    process_WTG(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_BHD.Match(clipboardContents);
                if (m.Success)
                {
                    process_BHD(m.Groups[1].Value);
                }
            }

            {
                Match m = REGEX_IP.Match(clipboardContents);
                if (m.Success)
                {
                    process_IP(m.Groups[1].Value);
                }
            }
        }
        private void process_TP(string value)
        {
            TextBox_PCin.Text = value;
        }
        private void process_OPL(string value)
        {
            TextBox_PCin.Text = value;
        }
        private void process_FRA(string value)
        {
            TextBox_PCin.Text = value;
        }
        private void process_WTG(string value)
        {
            TextBox_PCin.Text = value;
        }
        private void process_BHD(string value)
        {
            TextBox_PCin.Text = value;
        }
        private void process_IP(string value)
        {
            TextBox_PCin.Text = value;
        }

        /// <summary>
        /// Check IP ping from Clipboard
        /// </summary>
        private async void TextBox_PCin_TextChanged(object sender, TextChangedEventArgs e)
        {
            string pcaddress = TextBox_PCin.Text;

            if(REGEX_TP.IsMatch(TextBox_PCin.Text))
            {
                Services.PC pec = new Services.PC();
                int pingcheck = await pec.PingAuto(TextBox_PCin, WaitBarPC);

                if(pingcheck == 1) { Helpers.TrayIcon.BalloonPingOnline(pcaddress); }
                else if(pingcheck == 2) { Helpers.TrayIcon.BalloonPingOffline(pcaddress); }
            }

            else if(REGEX_OPL.IsMatch(TextBox_PCin.Text))
            {
                Services.PC pec = new Services.PC();
                int pingcheck = await pec.PingAuto(TextBox_PCin, WaitBarPC);

                if (pingcheck == 1) { Helpers.TrayIcon.BalloonPingOnline(pcaddress); }
                else if (pingcheck == 2) { Helpers.TrayIcon.BalloonPingOffline(pcaddress); }
            }

            else if(REGEX_FRA.IsMatch(TextBox_PCin.Text))
            {
                Services.PC pec = new Services.PC();
                int pingcheck = await pec.PingAuto(TextBox_PCin, WaitBarPC);

                if (pingcheck == 1) { Helpers.TrayIcon.BalloonPingOnline(pcaddress); }
                else if (pingcheck == 2) { Helpers.TrayIcon.BalloonPingOffline(pcaddress); }
            }

            else if(REGEX_WTG.IsMatch(TextBox_PCin.Text))
            {
                Services.PC pec = new Services.PC();
                int pingcheck = await pec.PingAuto(TextBox_PCin, WaitBarPC);

                if (pingcheck == 1) { Helpers.TrayIcon.BalloonPingOnline(pcaddress); }
                else if (pingcheck == 2) { Helpers.TrayIcon.BalloonPingOffline(pcaddress); }
            }

            else if (REGEX_BHD.IsMatch(TextBox_PCin.Text))
            {
                Services.PC pec = new Services.PC();
                int pingcheck = await pec.PingAuto(TextBox_PCin, WaitBarPC);

                if (pingcheck == 1) { Helpers.TrayIcon.BalloonPingOnline(pcaddress); }
                else if (pingcheck == 2) { Helpers.TrayIcon.BalloonPingOffline(pcaddress); }
            }

            else if(REGEX_IP.IsMatch(TextBox_PCin.Text))
            {
                Services.PC pec = new Services.PC();
                int pingcheck = await pec.PingAutoIP(TextBox_PCin, WaitBarPC);

                if (pingcheck == 1) { Helpers.TrayIcon.BalloonPingOnline(pcaddress); }
                else if (pingcheck == 2) { Helpers.TrayIcon.BalloonPingOffline(pcaddress); }
            }
        }
    }
}

