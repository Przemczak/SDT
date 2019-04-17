using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SDT.Services
{
    class User
    {
        private readonly MainWindow _mainWindow;

        public User(MainWindow MainWindow)
        {
            _mainWindow = MainWindow;
        }

        /// <summary>
        /// Check User in AD
        /// </summary>
        public async void CheckAD()
        {
            _mainWindow.userProgressBar.Visibility = Visibility.Visible;
            string userlogin = _mainWindow.userLoginTextBox.Text;

            var cuser = await CheckUserInAD(userlogin);
            if(cuser)
            {
                await DirectoryEnt(userlogin);
                await PrincipalCon(userlogin);
                await UserGroups(userlogin);
                await PowerS(userlogin);
            }
            else
            {
                _mainWindow.popupText.Text = "Brak podanego loginu w AD";
                _mainWindow.mainPopupBox.IsPopupOpen = true;

                _mainWindow.userProgressBar.Visibility = Visibility.Hidden;
                return;
            }
            _mainWindow.userProgressBar.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Check User in AD (login verification in AD)
        /// </summary>
        private async Task<bool> CheckUserInAD(string userlogin)
        {
            try
            {
                var user = await Task.Run(() =>
                    {
                        var ctx = new PrincipalContext(ContextType.Domain);
                        return UserPrincipal.FindByIdentity(ctx, userlogin);
                    });

                if (user == null) return false;
                else return true;
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return false;
            }
        }

        /// <summary>
        /// Check User in AD (DirectoryEntry - attributes + CheckBox_UserMailAct)
        /// </summary>
        private async Task DirectoryEnt(string userlogin)
        {
            try
            {
                SearchResult rs = await Task.Run(() =>
                {
                    DirectoryEntry de = new DirectoryEntry("LDAP://ldap.gk.corp.tepenet");
                    DirectorySearcher ds = new DirectorySearcher(de)
                    {
                        Filter = "(&((&(objectCategory=Person)(objectClass=User)))(sAMAccountName=" + userlogin + "))",
                        SearchScope = SearchScope.Subtree
                    };
                    return ds.FindOne();
                });

                var value3 = (rs.GetDirectoryEntry().Properties["userPrincipalName"].Value ?? "BRAK").ToString();
                    if (value3.Contains("BRAK"))
                        _mainWindow.userBptpTextBox.Text = "Brak";
                    else
                        _mainWindow.userBptpTextBox.Text = value3;

                long value4 = (long)rs.Properties["pwdLastSet"][0];
                    if (value4 == 0)
                        _mainWindow.userLastPassSetTextBox.Text = "Flaga zmiany hasła!";
                    else
                        { DateTime pwdLastSet = DateTime.FromFileTimeUtc(value4).ToLocalTime();
                        _mainWindow.userLastPassSetTextBox.Text = pwdLastSet.ToString(); }

                var value6 = (rs.GetDirectoryEntry().Properties["employeePTKID"].Value ?? "BRAK").ToString();
                    if (value6.Contains("BRAK"))
                        _mainWindow.userIfsTextBox.Text = "Brak Danych";
                    else
                        _mainWindow.userIfsTextBox.Text = value6;

                var value7 = (rs.GetDirectoryEntry().Properties["manager"].Value ?? "BRAK").ToString();
                var value5 = (rs.GetDirectoryEntry().Properties["extensionAttribute12"].Value ?? "BRAK").ToString();
                if (value7.Contains("BRAK"))
                { _mainWindow.userManagerTextBox.Text = "BRAK"; _mainWindow.userFunctionalTextBox.Text = "BRAK"; }
                else
                {
                    string value7a = value7.Remove(value7.IndexOf(",")).Substring(value7.IndexOf("=") + 1);

                    using (var ctx = new PrincipalContext(ContextType.Domain))
                    using (UserPrincipal fullmanag = UserPrincipal.FindByIdentity(ctx, value7a))
                    {
                        if (fullmanag == null)
                        {
                            _mainWindow.userManagerTextBox.Text = "Błędne dane w AD";
                            _mainWindow.userFunctionalTextBox.Text = "Błędne dane w AD";
                        }
                        else if (value5.Contains("BRAK"))
                        {
                            _mainWindow.userManagerTextBox.Text = fullmanag.DisplayName;
                            _mainWindow.userFunctionalTextBox.Text = "BRAK";
                        }
                        else
                        {
                            _mainWindow.userManagerTextBox.Text = value5;
                            _mainWindow.userFunctionalTextBox.Text = fullmanag.DisplayName;
                        }
                    }
                }
                
                var value8 = (rs.GetDirectoryEntry().Properties["businessCategory"].Value ?? "BRAK").ToString();
                if (value8.Contains("BRAK"))
                    _mainWindow.userEmploymentTextBox.Text = "BRAK";
                else
                    _mainWindow.userEmploymentTextBox.Text = value8;

                var value9 = (rs.GetDirectoryEntry().Properties["msRTCSIP-PrimaryUserAddress"].Value ?? "BRAK").ToString();
                if (value9.Contains("BRAK"))
                    _mainWindow.userSipTextBox.Text = "BRAK";
                else
                    _mainWindow.userSipTextBox.Text = value9.Replace("sip:", "");

                var value10 = (rs.GetDirectoryEntry().Properties["extensionAttribute1"].Value ?? "BRAK").ToString();
                if (value10.Contains("BRAK"))
                    _mainWindow.userMailClassTextBox.Text = "BRAK";
                else
                    _mainWindow.userMailClassTextBox.Text = value10;

                var value11 = (rs.GetDirectoryEntry().Properties["mDBOverHardQuotaLimit"].Value ?? "BRAK").ToString();
                if (value11.Contains("BRAK"))
                    _mainWindow.userMailQuotaTextBox.Text = "BRAK";
                else
                    _mainWindow.userMailQuotaTextBox.Text = value11;

                var value12 = (rs.GetDirectoryEntry().Properties["mail"].Value ?? "BRAK").ToString();
                if (value12.Contains("BRAK"))
                    _mainWindow.userMailAddressTextBox.Text = "BRAK";
                else
                    _mainWindow.userMailAddressTextBox.Text = value12;

                var count = rs.GetDirectoryEntry().Properties["workstationAdmin"].Count;
                var value13Array = rs.GetDirectoryEntry().Properties["workstationAdmin"].Value;
                string value13 = "";
                if (count == 1)
                    _mainWindow.userDeveloperTextBox.Text = value13Array.ToString();
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        value13 += ((object[])value13Array)[i].ToString() + Environment.NewLine;
                    }
                    if (value13.Contains("BRAK"))
                    {
                        _mainWindow.userDeveloperTextBox.Text = "Brak profilu developer.";
                    }
                    else
                    {
                        _mainWindow.userDeveloperTextBox.Text = value13;
                    }
                }

                var value14 = (rs.GetDirectoryEntry().Properties["extensionAttribute1"].Value ?? "BRAK").ToString();

                if (value14.Contains("BRAK"))
                { _mainWindow.mailAccCheckBox.IsChecked = false; _mainWindow.mailAccCheckBox.Background = Brushes.Red; }
                else if (value14.Contains("Disabled"))
                { _mainWindow.mailAccCheckBox.IsChecked = false; _mainWindow.mailAccCheckBox.Background = Brushes.Red; }
                else
                { _mainWindow.mailAccCheckBox.IsChecked = true; _mainWindow.mailAccCheckBox.Background = Brushes.ForestGreen; }

                var value15 = (rs.GetDirectoryEntry().Properties["whenCreated"].Value ?? "BRAK").ToString();
                _mainWindow.userAccCreated.Text = value15;
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Check User in AD (PrincipalContext - Account Status)
        /// </summary>
        private async Task PrincipalCon(string userlogin)
        {
            try
            {
                var user = await Task.Run(() =>
                {
                    var ctx = new PrincipalContext(ContextType.Domain);
                    return UserPrincipal.FindByIdentity(ctx, userlogin);
                });

                if (user.IsAccountLockedOut())
                {
                    _mainWindow.blockedAccCheckbox.IsChecked = true; _mainWindow.blockedAccCheckbox.Background = Brushes.Red;
                    _mainWindow.AdStatusCheckbox.IsChecked = true; _mainWindow.AdStatusCheckbox.Background = Brushes.ForestGreen;

                }
                else if (user.Enabled == false)
                {
                    _mainWindow.blockedAccCheckbox.IsChecked = false;
                    _mainWindow.AdStatusCheckbox.IsChecked = false; _mainWindow.AdStatusCheckbox.Background = Brushes.Red;
                }
                else
                {
                    _mainWindow.blockedAccCheckbox.IsChecked = false;
                    _mainWindow.AdStatusCheckbox.IsChecked = true; _mainWindow.AdStatusCheckbox.Background = Brushes.ForestGreen;
                }

                if (user.AccountExpirationDate.HasValue)
                {
                    DateTime expiration = user.AccountExpirationDate.Value.ToLocalTime();
                    _mainWindow.userAccExpireTextBox.Text = expiration.ToString();
                }
                else
                {
                    _mainWindow.userAccExpireTextBox.Text = "Konto nie wygasa";
                }
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Check User in AD (PrincipalContext - AD Groups)
        /// </summary>
        private async Task UserGroups(string userlogin)
        {
            try
            {
                List<string> output = await Task.Run(() =>
                   {
                       var ctx = new PrincipalContext(ContextType.Domain);
                       UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userlogin);

                       return output = user.GetGroups().Select(x => x.SamAccountName).ToList();
                   });

                _mainWindow.printDenyCheckBox.IsChecked = output.Contains("#Print_Deny") ;

                _mainWindow.printColorCheckBox.IsChecked = output.Contains("#Print_Color");

                _mainWindow.devicesCheckBox.IsChecked = output.Contains("#EW_GxDeviceAll");

                _mainWindow.airnetCheckBox.IsChecked = output.Contains("#AirNet_WLAN");

                _mainWindow.lyncPcCheckBox.IsChecked = output.Contains("# Lync Pracownicy Etatowi Użytkownicy") || output.Contains("# Lync Partnerzy Użytkownicy");

                _mainWindow.internetCheckBox.IsChecked = output.Contains("Internet");

                _mainWindow.developerCheckBox.IsChecked = output.Contains("#Developer");

                //AIRWATCH
                _mainWindow.airwatchBasicCheckBox.IsChecked = output.Contains("AirWatch_view");

                _mainWindow.airwatchExpCheckBox.IsChecked = output.Contains("AirWatch_edit");

                _mainWindow.airwatchVipCheckBox.IsChecked = output.Contains("Airwatch_VIP");

                _mainWindow.airwatchLyncCheckBox.IsChecked = output.Contains("# Lync Dostęp z Urządzeń Mobilnych");

                _mainWindow.airwatchSkypeCheckbox.IsChecked = output.Contains("# S4B 2015");

                //BYOD
                _mainWindow.byodCitrixCheckBox.IsChecked = output.Contains("BYOD-Pulpit-Standard");

                _mainWindow.byodHddCheckBox.IsChecked = output.Contains("BYOD_HDD_users");

                _mainWindow.byodWtgCheckBox.IsChecked = output.Contains("EBU-Users-WTG");

                _mainWindow.lvmW7CheckBox.IsChecked = output.Contains("BYODVM_W7_users");

                _mainWindow.lvm10CheckBox.IsChecked = output.Contains("BYODVM_W10_users");

                _mainWindow.byodTestCitrixCheckBox.IsChecked = output.Contains("BYOD-Pulpit-Standard-Test");

                _mainWindow.lvm7TestCheckBox.IsChecked = output.Contains("BYODVM_W7_manual");

                _mainWindow.lvm10TestCheckBox.IsChecked = output.Contains("BYODVM_W10_manual");
            }
            catch (Exception e)
            {
                _mainWindow.popupText.Text = e.Message;
                _mainWindow.mainPopupBox.IsPopupOpen = true;
                return;
            }
        }

        /// <summary>
        /// Check User in AD (PowerShell - password time expire)
        /// </summary>
        private async Task PowerS(string userlogin)
        {
            try
            {
                using (PowerShell PS = PowerShell.Create())
                {
                    string result = await Task.Run(() =>
                    {
                        PS.AddScript("Get-ADUser " + userlogin + " -Properties msDS-UserPasswordExpiryTimeComputed | Select @{ Name = \"ExpiryDate\"; Expression ={ ([datetime]::fromfiletime($_.\"msDS-UserPasswordExpiryTimeComputed\")).DateTime} }");
                        Collection<PSObject> psObjects;
                        psObjects = PS.Invoke();
                        return psObjects.FirstOrDefault().ToString();
                    });

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Substring(result.IndexOf(',') + 1);
                        _mainWindow.userPassExpireTextBox.Text = result.Substring(result.IndexOf('=') + 1).TrimEnd('}').ToString();
                    }
                    else
                    {
                        _mainWindow.popupText.Text = "Brak danych.";
                        _mainWindow.mainPopupBox.IsPopupOpen = true;
                        return;
                    }
                }
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

