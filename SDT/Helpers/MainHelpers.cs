using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SDT.Helpers
{
    class MainHelpers
    {
        /// <summary>
        ///  and Checkboxes in USER_TAB
        /// </summary>
        public static void ClearBoxesUSERTAB(Grid Grid_UserAD, Grid Grid_UserUser, Grid Grid_UserMail, Grid Grid_UserMailBPTP, Grid Grid_UserDev, Grid Grid_UserMailC, 
            Grid Grid_UserAccess, Grid Grid_UserBYOD, Grid Grid_UserAirWatch)
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

        /// <summary>
        /// Clear Textboxes and Checkboxes in PC_TAB
        /// </summary>
        public static void ClearBoxesPCTAB(Grid Grid_PCInfo)
        {
            foreach (Control c in Grid_PCInfo.Children)
            {
                if (c is TextBox && c != null) { ((TextBox)c).Text = string.Empty; }
            }
        }

        /// <summary>
        /// Clear Chechboxes in PC_TAB (Ports)
        /// </summary>
        public static void ClearChecksPCTAB(Grid Grid_PCPorts)
        {
            foreach (Control c in Grid_PCPorts.Children)
            {
                if (c is CheckBox && c != null) { ((CheckBox)c).IsChecked = false; }
            }
        }

        /// <summary>
        /// Check null for USER
        /// </summary>
        public static bool CheckNullUSERTAB(TextBox TextBox_UserLoginIn)
        {
            if (string.IsNullOrWhiteSpace(TextBox_UserLoginIn.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj login użytkownika.");
                return false;
            }
            else { return true; }
        }

        /// <summary>
        /// Check null for PC
        /// </summary>
        public static bool CheckNullPCTAB(TextBox TextBox_PCin)
        {
            if (string.IsNullOrWhiteSpace(TextBox_PCin.Text))
            {
                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", "Podaj login użytkownika.");
                return false;
            }
            else { return true; }
        }
    }
}
