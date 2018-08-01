﻿using MahApps.Metro.Controls;
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
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Button_Info_Click(object sender, RoutedEventArgs e)
        {
            Information sh = new Information
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            sh.ShowDialog();
        }

        private void Button_UserCheck_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control c in test1.Children)
            {
                if (c is TextBox && c != null)
                {
                    if (c.Name != "TextBox_UserLoginIn")
                        ((TextBox)c).Text = String.Empty;
                    CheckBox_UserPrintDeny.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCCCC"));
                    CheckBox_UserMailAct.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCCCC"));
                }
            }

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
            pec.Sharing(TextBox_PCin);
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

        private void Button_PsExecTools_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void Button_PsExec_Click(object sender, RoutedEventArgs e)
        {
            PC pec = new PC(this);
            pec.PsExecInstall();
        }

        private void Button_Insta_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}