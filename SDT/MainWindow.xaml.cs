﻿using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SDT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }



        private void Button_Info_Click(object sender, RoutedEventArgs e)
        {
            Information sh = new Information
            {
                Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            sh.Show();
        }

        private void Click_Button_Settings(object sender, RoutedEventArgs e)
        {
            SettingsPage sp = new SettingsPage
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
            //foreach (Control c in Grid_User.Children)
            //{
            //    if (c is TextBox && c != null)
            //    {
            //        if (c.Name != "TextBox_UserLoginIn")
            //            ((TextBox)c).Text = String.Empty;
            //        CheckBox_UserPrintDeny.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCCCC"));
            //        CheckBox_UserMailAct.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCCCC"));
            //    }
            //}

            TextBox_UserLoginIn.Text = string.Join("", TextBox_UserLoginIn.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_UserLoginIn.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj login użytkownika.");
                return;
            }

            User us = new User(this);
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
            PC pec = new PC(this);
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

            PC pec = new PC(this);
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

            PC pec = new PC(this);
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

            PC pec = new PC(this);
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

            PC pec = new PC(this);
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
                PC pec = new PC();
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
                PC pec = new PC(this);
                await pec.PortCheck(TextBox_PCin, WaitBarPC);
            }
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
            PC_Scripts pecs = new PC_Scripts(this);
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
            PC_Scripts pecs = new PC_Scripts(this);
            pecs.BitLocker(TextBox_PCin, WaitBarPC);
        }

        private void Button_IEFix_Click(object sender, RoutedEventArgs e)
        {
            TextBox_PCin.Text = string.Join("", TextBox_PCin.Text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj adres stacji.");
                return;
            }
            PC_Scripts pecs = new PC_Scripts(this);
            pecs.IEFix(TextBox_PCin);
        }
    }
}
