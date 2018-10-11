using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SDT.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SDT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly Services.User _user;
        private readonly Services.PC _pc;
        private readonly Services.PC_Scripts _pc_scripts;

        public MainWindow()
        {
            InitializeComponent();

            // Load theme color and application color settings
            ThemeSettingsLoad();

            // Load tray icon
            TrayIcon.Tray(this);

            // Load clipboard listening and load Regex
            StartListeningForClipboardChange();

            // Create instance of methods
            _user = new Services.User(this);
            _pc = new Services.PC(this);
            _pc_scripts = new Services.PC_Scripts(this);
        }

        public void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Dispose tray icon
            TrayIcon.disposeico();

            // Save settings
            Properties.Settings.Default.Save();

            //Close all opened pages on exit
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// USER TAB
        /// Check User Login
        /// </summary>
        private void Button_UserCheck_Click(object sender, RoutedEventArgs e)
        {
            //Clear Textboxes and Checkboxes in USER_TAB
            MainHelpers.ClearBoxesUSERTAB(Grid_UserAD, Grid_UserUser, Grid_UserMail, Grid_UserMailBPTP, Grid_UserDev, Grid_UserMailC, Grid_UserAccess, Grid_UserBYOD, Grid_UserAirWatch);

            TextBox_UserLoginIn.Text = TextBox_UserLoginIn.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullUSERTAB(TextBox_UserLoginIn);
            if(CS)
                _user.CheckAD(TextBox_UserLoginIn, WaitBarUser);
        }

        /// <summary>
        /// PC TAB
        /// Check PC ping
        /// </summary>
        private async void Button_PCping_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                await _pc.Ping(TextBox_PCin, WaitBarPC);
        }

        /// <summary>
        /// PC TAB
        /// Run PC CMD Ping (-t)
        /// </summary>
        private void Button_PCpingT_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc.PingT(TextBox_PCin);
        }

        /// <summary>
        /// PC TAB
        /// Run Sharing C:\
        /// </summary>
        private void Button_Sharing_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc.Sharing(TextBox_PCin, WaitBarPC);
        }

        /// <summary>
        /// PC TAB
        /// Check PC info
        /// </summary>
        private void Button_PCinfo_Click(object sender, RoutedEventArgs e)
        {
            //Clear Textboxes in PC_TAB(PC info)
            MainHelpers.ClearBoxesPCTAB(Grid_PCInfo);

            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc.PCinfo(TextBox_PCin, WaitBarPC);
        }

        /// <summary>
        /// PC TAB
        /// Run RCV Client
        /// </summary>
        private void Button_RCV_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc.Cmrcviewer(TextBox_PCin, WaitBarPC);
        }

        /// <summary>
        /// PC TAB
        /// Check Ports
        /// </summary>
        private async void Button_PCPorts_Click(object sender, RoutedEventArgs e)
        {
            //Clear Checkboxes in PC_TAB(Ports)
            MainHelpers.ClearBoxesPCTAB(Grid_PCPorts);

            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                await _pc.PortCheck(TextBox_PCin, WaitBarPC);
        }

        /// <summary>
        /// PC TAB
        /// Run PsExec
        /// </summary>
        private void Button_PsExec_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc.PsExecRUN(TextBox_PCin, WaitBarPC);
        }

        /// <summary>
        /// PC TAB
        /// Run PsExec
        /// </summary>
        private void Button_Insta_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
            {
                var pscheck = _pc.PsExecCheck();
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

        /// <summary>
        /// PC TAB
        /// Menu - Button Scripts
        /// </summary>
        private void Button_Scripts_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// PC TAB
        /// Menu - Button Scripts (Scripts list)
        /// </summary>
        private void Button_GP_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc_scripts.GPUpdate(TextBox_PCin, WaitBarPC);
        }
        private void Button_BitLocker_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc_scripts.BitLocker(TextBox_PCin, WaitBarPC);
        }
        private void Button_IEFix_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc_scripts.IEFix(TextBox_PCin);
        }
        private void Button_SpoolReset_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = TextBox_PCin.Text.TrimStart().TrimEnd();

            bool CS = MainHelpers.CheckNullPCTAB(TextBox_PCin);
            if (CS)
                _pc_scripts.SpoolerReset(TextBox_PCin, WaitBarPC);
        }

        /// <summary>
        /// Start Listening from Clipboard
        /// </summary>
        private void StartListeningForClipboardChange()
        {
            var helper = new ClipboardHelpers(this);
            Services.ClipboardNotification.ClipboardUpdate += (o, e) =>
            {
                helper.OnClipboardChanged(Clipboard.GetText());
            };
        }

        /// <summary>
        /// Load Theme/Color Settings
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
        /// Open Information Page
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
        /// Open Settings Page
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
        /// Ping and Tray Balloon from Clipboard
        /// </summary>
        private void TextBox_PCin_TextChanged(object sender, TextChangedEventArgs e)
        {
            var helper2 = new ClipboardHelpers(this);
            helper2.ClipboardToTextbox();
        }
    }
}

