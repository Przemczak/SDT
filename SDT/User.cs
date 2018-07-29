using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management.Automation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SDT
{
    class User
    {
        private readonly MainWindow _MetroWindow;

        public User(MainWindow MetroWindow)
        {
            _MetroWindow = MetroWindow;
        }

        /// <summary>
        /// Check User in AD
        /// </summary>
        public void CheckAD(TextBox TextBox_UserLoginIn)
        {
            try
            {
                using (var ctx = new PrincipalContext(ContextType.Domain))
                using (var user = UserPrincipal.FindByIdentity(ctx, TextBox_UserLoginIn.Text))
                {
                    if (user != null)
                    {
                        user.Dispose();
                    }
                    else
                    {
                        var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                        if (window != null)
                            window.ShowMessageAsync("Błąd!", "Brak użytkownika o takim loginie.");
                        return;
                    }
                }
                
                DirectoryEnt(TextBox_UserLoginIn);

                PrincipalCon(TextBox_UserLoginIn);

                PowerS(TextBox_UserLoginIn);
            }
            catch (Exception a)
            {

                var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                if (window != null)
                    window.ShowMessageAsync("Błąd!", a.Message);
                return;
            }
        }

        /// <summary>
        /// Check User in AD (DirectoryEntry - attributes)
        /// </summary>
        void DirectoryEnt(TextBox TextBox_UserLoginIn)
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://ldap.gk.corp.tepenet");
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(sAMAccountName=" + TextBox_UserLoginIn.Text + "))";
            ds.SearchScope = SearchScope.Subtree;
            SearchResult rs = ds.FindOne();

            var value3 = (rs.GetDirectoryEntry().Properties["userPrincipalName"].Value ?? "BRAK").ToString();
            if (value3.Contains("BRAK"))
            {
                _MetroWindow.TextBox_UserBPTP.Text = "Brak";
            }
            else
            {
                _MetroWindow.TextBox_UserBPTP.Text = value3;
            }

            long value4 = (long)rs.Properties["pwdLastSet"][0];
            if (value4 == 0)
            {
                _MetroWindow.TextBox_UserLastPassChange.Text = "Flaga zmiany hasła!";
            }
            else
            {
                DateTime pwdLastSet = DateTime.FromFileTimeUtc(value4).ToLocalTime();
                _MetroWindow.TextBox_UserLastPassChange.Text = pwdLastSet.ToString();
            }

            var value6 = (rs.GetDirectoryEntry().Properties["employeePTKID"].Value ?? "BRAK").ToString();
            if (value6.Contains("BRAK"))
            {
                _MetroWindow.TextBox_UserIFS.Text = "Brak Danych";
            }
            else
            {
                _MetroWindow.TextBox_UserIFS.Text = value6;
            }
            //TODO: DISPLAY NAME - SPRAWDZIC
            var value7 = (rs.GetDirectoryEntry().Properties["manager"].Value ?? "BRAK").ToString();
            var value5 = (rs.GetDirectoryEntry().Properties["extensionAttribute12"].Value ?? "BRAK").ToString();
            string value7a = value7.Remove(value7.IndexOf(",")).Substring(value7.IndexOf("=") + 1);

            using (var ctx = new PrincipalContext(ContextType.Domain))
            using (UserPrincipal fullmanag = UserPrincipal.FindByIdentity(ctx, value7a))
            {
                if (value5.Contains("BRAK"))
                {
                    _MetroWindow.TextBox_User_Manager.Text = fullmanag.DisplayName;
                    _MetroWindow.TextBox_User_FunctManag.Text = "BRAK";
                }
                else
                {
                    _MetroWindow.TextBox_User_Manager.Text = value5;
                    _MetroWindow.TextBox_User_FunctManag.Text = fullmanag.DisplayName;
                }
            }



            //if (value5.Contains("BRAK"))
            //{
            //    _MetroWindow.TextBox_User_Manager.Text = value7.Remove(value7.IndexOf(",")).Substring(value7.IndexOf("=") + 1);
            //    _MetroWindow.TextBox_User_FunctManag.Text = "BRAK";
            //}
            //else
            //{
            //    _MetroWindow.TextBox_User_Manager.Text = value5;
            //    _MetroWindow.TextBox_User_FunctManag.Text = value7.Remove(value7.IndexOf(",")).Substring(value7.IndexOf("=") + 1);
            //}
            //using (var ctx = new PrincipalContext(ContextType.Domain))
            //using (UserPrincipal fullmanag = UserPrincipal.FindByIdentity(ctx, value7clear));





            var value8 = (rs.GetDirectoryEntry().Properties["businessCategory"].Value ?? "BRAK").ToString();
            if (value8.Contains("BRAK"))
            {
                _MetroWindow.TextBox_UserEmploy.Text = value5.Remove(value5.IndexOf(",")).Substring(value5.IndexOf("=") + 1);
            }
            else
            {
                _MetroWindow.TextBox_UserEmploy.Text = value8;
            }

            var value9 = (rs.GetDirectoryEntry().Properties["msRTCSIP-PrimaryUserAddress"].Value ?? "BRAK").ToString();
            if (value9.Contains("BRAK"))
            {
                _MetroWindow.TextBox_UserSIP.Text = "BRAK";
            }
            else
            {
                _MetroWindow.TextBox_UserSIP.Text = value9.Replace("sip:", "");
            }

            var value10 = (rs.GetDirectoryEntry().Properties["extensionAttribute1"].Value ?? "BRAK").ToString();
            if (value10.Contains("BRAK"))
            {
                _MetroWindow.TextBox_UserMailBoxClass.Text = "BRAK";
            }
            else
            {
                _MetroWindow.TextBox_UserMailBoxClass.Text = value10;
            }

            var value11 = (rs.GetDirectoryEntry().Properties["mDBOverHardQuotaLimit"].Value ?? "BRAK").ToString();
            if (value11.Contains("BRAK"))
            {
                _MetroWindow.TextBox_UserMailQuota.Text = "BRAK";
            }
            else
            {
                _MetroWindow.TextBox_UserMailQuota.Text = value11;
            }

            var value12 = (rs.GetDirectoryEntry().Properties["mail"].Value ?? "BRAK").ToString();
            if (value12.Contains("BRAK"))
            {
                _MetroWindow.TextBox_UserMailAdr.Text = "BRAK";
            }
            else
            {
                _MetroWindow.TextBox_UserMailAdr.Text = value12;
            }

            var count = rs.GetDirectoryEntry().Properties["workstationAdmin"].Count;
            var value13Array = rs.GetDirectoryEntry().Properties["workstationAdmin"].Value;
            string value13 = "";
            for (int i = 0; i < count; i++)
            {
                value13 += ((object[])value13Array)[i].ToString() + Environment.NewLine;
            }
            if (value13.Contains("BRAK"))
            {
                _MetroWindow.TextBox_UserDevPC.Text = "Brak profilu developer.";
            }
            else
            {
                _MetroWindow.TextBox_UserDevPC.Text = value13;
            }

            var value14 = (rs.GetDirectoryEntry().Properties["extensionAttribute1"].Value ?? "BRAK").ToString();
            if (value14.Contains("BRAK"))
            {
                _MetroWindow.CheckBox_UserMailAct.IsChecked = false;
            }
            else if (value14.Contains("Disabled"))
            {
                _MetroWindow.CheckBox_UserMailAct.IsChecked = false;
                _MetroWindow.CheckBox_UserMailAct.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                _MetroWindow.CheckBox_UserMailAct.IsChecked = true;
            }

        }

        /// <summary>
        /// Check User in AD (PrincipalContext - AD groups)
        /// </summary>
        void PrincipalCon(TextBox TextBox_UserLoginIn)
        {
            string[] output = null;
            using (var ctx = new PrincipalContext(ContextType.Domain))
            using (var user = UserPrincipal.FindByIdentity(ctx, TextBox_UserLoginIn.Text))
            {
                if (user.IsAccountLockedOut())
                {
                    _MetroWindow.TextBox_UserLocked.Text = "ZABLOKOWANE";

                }
                else
                {
                    _MetroWindow.TextBox_UserLocked.Text = "AKTYWNE";
                }

                if (user.AccountExpirationDate.HasValue)
                {
                    DateTime expiration = user.AccountExpirationDate.Value.ToLocalTime();
                    _MetroWindow.TextBox_UserAccExpire.Text = expiration.ToString();
                }
                else
                {
                    _MetroWindow.TextBox_UserAccExpire.Text = "Konto nie wygasa";
                }

                //GROUPS AD
                output = user.GetGroups().Select(x => x.SamAccountName).ToArray();

                if (output.Contains("#Print_Deny"))
                {
                    _MetroWindow.CheckBox_UserPrintDeny.IsChecked = true;
                    _MetroWindow.CheckBox_UserPrintDeny.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    _MetroWindow.CheckBox_UserPrintDeny.IsChecked = false;
                }

                if (output.Contains("#Print_Color"))
                {
                    _MetroWindow.CheckBox_UserPrintColor.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserPrintColor.IsChecked = false;
                }

                if (output.Contains("#EW_GxDeviceAll"))
                {
                    _MetroWindow.CheckBox_UserDevices.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserDevices.IsChecked = true;
                }

                if (output.Contains("#AirNet_WLAN"))
                {
                    _MetroWindow.CheckBox_UserAirnet.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserAirnet.IsChecked = false;
                }

                if (output.Contains("# Lync Pracownicy Etatowi Użytkownicy") || output.Contains("# Lync Partnerzy Użytkownicy"))
                {
                    _MetroWindow.CheckBox_UserLyncPC.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserLyncPC.IsChecked = false;
                }

                if (output.Contains("Internet"))
                {
                    _MetroWindow.CheckBox_UserInternet.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserInternet.IsChecked = false;
                }

                if (output.Contains("#Developer"))
                {
                    _MetroWindow.CheckBox_UserDev.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserDev.IsChecked = false;
                }

                //AIRWATCH
                if (output.Contains("AirWatch_view"))
                {
                    _MetroWindow.CheckBox_UserAirWatchBasic.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserAirWatchBasic.IsChecked = false;
                }

                if (output.Contains("AirWatch_edit"))
                {
                    _MetroWindow.CheckBox_UserAirWatchExp.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserAirWatchExp.IsChecked = false;
                }

                if (output.Contains("Airwatch_VIP"))
                {
                    _MetroWindow.CheckBox_UserAirWatchVIP.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserAirWatchVIP.IsChecked = false;
                }

                if (output.Contains("# Lync Dostęp z Urządzeń Mobilnych"))
                {
                    _MetroWindow.CheckBox_UserAirWatchLync.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserAirWatchLync.IsChecked = false;
                }

                if (output.Contains("# S4B 2015"))
                {
                    _MetroWindow.CheckBox_UserAirWatchSkype.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserAirWatchSkype.IsChecked = false;
                }

                //BYOD
                if (output.Contains("BYOD-Pulpit-Standard"))
                {
                    _MetroWindow.CheckBox_UserByodCitrix.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserByodCitrix.IsChecked = false;
                }

                if (output.Contains("BYOD_HDD_users"))
                {
                    _MetroWindow.CheckBox_UserByodHDD.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserByodHDD.IsChecked = false;
                }

                if (output.Contains("EBU-Users-WTG"))
                {
                    _MetroWindow.CheckBox_UserWTG.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserWTG.IsChecked = false;
                }

                if (output.Contains("BYODVM_W7_users"))
                {
                    _MetroWindow.CheckBox_UserLWMW7.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserLWMW7.IsChecked = false;
                }

                if (output.Contains("BYODVM_W10_users"))
                {
                    _MetroWindow.CheckBox_UserLWMW10.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserLWMW10.IsChecked = false;
                }

                if (output.Contains("BYOD-Pulpit-Standard-Test"))
                {
                    _MetroWindow.CheckBox_UserByodCitrixTest.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserByodCitrixTest.IsChecked = false;
                }

                if (output.Contains("BYODVM_W7_manual"))
                {
                    _MetroWindow.CheckBox_UserLWMW7Test.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserLWMW7Test.IsChecked = false;
                }

                if (output.Contains("BYODVM_W10_manual"))
                {
                    _MetroWindow.CheckBox_UserLWMW10Test.IsChecked = true;
                }
                else
                {
                    _MetroWindow.CheckBox_UserLWMW10Test.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// Check User in AD (PowerShell - passexpire)
        /// </summary>
        void PowerS(TextBox TextBox_UserLoginIn)
        {
            using (PowerShell PS = PowerShell.Create())
            {
                //PS.AddScript("Get-ADUser wojtcpr1 -Properties msDS-UserPasswordExpiryTimeComputed | Select @{ Name = \"ExpiryDate\"; Expression ={ ([datetime]::fromfiletime($_.\"msDS-UserPasswordExpiryTimeComputed\")).DateTime} }");
                // @{ ExpiryDate = niedziela, 5 sierpnia 2018 07:39:22}
                //PS.AddScript("Get-ADUser " + TextBox_UserLoginIn.Text + " -Properties msDS-UserPasswordExpiryTimeComputed | Select -Expand \"msDS-UserPasswordExpiryTimeComputed\"");
                PS.AddScript("Get-ADUser " + TextBox_UserLoginIn.Text + " -Properties msDS-UserPasswordExpiryTimeComputed | Select @{ Name = \"ExpiryDate\"; Expression ={ ([datetime]::fromfiletime($_.\"msDS-UserPasswordExpiryTimeComputed\")).DateTime} }");
                Collection<PSObject> psObjects;
                psObjects = PS.Invoke();
                string result = psObjects.FirstOrDefault().ToString();
                if (!string.IsNullOrEmpty(result))
                {
                    _MetroWindow.TextBox_UserPassExpire.Text = result.ToString();
                }
                else
                {
                    var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
                    if (window != null)
                        window.ShowMessageAsync("Błąd!", "Brak danych");
                    return;
                }
            }
    }
        

    }
}

