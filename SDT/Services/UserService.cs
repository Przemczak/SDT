using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using SDT.ViewModels;
using SDT.Models;

namespace SDT.Services
{
    class UserService
    {
        private User userModel;
        private ApplicationViewModel applicationVM;
        private IDialogCoordinator dialogCoordinator;
        private string UserLogin;

        public UserService(User UserModel, ApplicationViewModel ApplicationVM, IDialogCoordinator DialogCoordinator)
        {
            userModel = UserModel;
            applicationVM = ApplicationVM;
            dialogCoordinator = DialogCoordinator;
        }

        public async Task CheckUser()
        {
            UserLogin = userModel.UserLogin;

            if (string.IsNullOrWhiteSpace(UserLogin))
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Błąd", "Podaj login użytkownika.");
                return;
            }
            else
            {
                UserLogin = UserLogin.TrimStart().TrimEnd();
                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain))
                {
                    var user = UserPrincipal.FindByIdentity(principalContext, UserLogin);
                    if (user == null)
                    {
                        await dialogCoordinator.ShowMessageAsync(applicationVM, "Błąd", "Brak podanego użytkownika w AD.");
                        return;
                    }
                    else
                    {
                        await GetUserData();
                        await GetUserAccStatus();
                        await GetUserGroups();
                        await GetUserPassword();
                    }
                }
            }
        }

        private async Task GetUserData()
        {
            try
            {
                await Task.Run(() =>
                {
                    using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP NAME"))
                    {
                        DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry)
                        {
                            Filter = "(&((&(objectCategory=Person)(objectClass=User)))(sAMAccountName=" + UserLogin + "))",
                            SearchScope = SearchScope.Subtree
                        };
                        SearchResult searchResult = directorySearcher.FindOne();

                        userModel.FirstName = (searchResult.GetDirectoryEntry().Properties["givenName"].Value ?? "").ToString();
                        userModel.LastName = (searchResult.GetDirectoryEntry().Properties["sn"].Value ?? "").ToString();
                        userModel.Employment = (searchResult.GetDirectoryEntry().Properties["businessCategory"].Value ?? "").ToString();
                        userModel.Company = (searchResult.GetDirectoryEntry().Properties["company"].Value ?? "").ToString();
                        userModel.Manager = (searchResult.GetDirectoryEntry().Properties["manager"].Value ?? "").ToString();
                        userModel.FunctionalManager = (searchResult.GetDirectoryEntry().Properties["extensionAttribute12"].Value ?? "").ToString();
                        userModel.IFS = (searchResult.GetDirectoryEntry().Properties["employeePTKID"].Value ?? "").ToString();
                        userModel.Mail = (searchResult.GetDirectoryEntry().Properties["mail"].Value ?? "").ToString();

                        if (searchResult.GetDirectoryEntry().Properties["extensionAttribute1"].Contains(null))
                        { userModel.MailStatus = "Brak danych"; }
                        else if (searchResult.GetDirectoryEntry().Properties["extensionAttribute1"].Contains("Disabled"))
                        { userModel.MailStatus = "Wyłączona"; }
                        else { userModel.MailStatus = "Aktywna"; }

                        userModel.MailClass = (searchResult.GetDirectoryEntry().Properties["extensionAttribute1"].Value ?? "").ToString();
                        userModel.MailQuota = (searchResult.GetDirectoryEntry().Properties["mDBOverHardQuotaLimit"].Value ?? "").ToString();
                        userModel.MailSIP = (searchResult.GetDirectoryEntry().Properties["msRTCSIP-PrimaryUserAddress"].Value ?? "").ToString().Replace("sip:", "");
                        userModel.MailBPTP = (searchResult.GetDirectoryEntry().Properties["userPrincipalName"].Value ?? "").ToString();
                        userModel.AccountCreated = Convert.ToDateTime(searchResult.GetDirectoryEntry().Properties["whenCreated"].Value ?? "").ToString("dd/MM/yyyy HH:mm:ss");

                        long tempPwdLastSet = (long)searchResult.Properties["pwdLastSet"][0];
                        userModel.PasswordLastSet = DateTime.FromFileTimeUtc(tempPwdLastSet).ToString("dd/MM/yyyy HH:mm:ss");
                    }
                });
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Błąd pobierania danych", ex.ToString());
            }
        }

        private async Task GetUserAccStatus()
        {
            try
            {
                await Task.Run(() =>
                {
                    using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain))
                    {
                        using (UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, UserLogin))
                        {
                            //AD ACCOUNT STATUS
                            if (userPrincipal.IsAccountLockedOut())
                            { userModel.AccountStatus = "Zablokowane"; }
                            else if (userPrincipal.Enabled == false)
                            { userModel.AccountStatus = "Wyłączone"; }
                            else
                            { userModel.AccountStatus = "Aktywne"; }

                            //AD ACCOUNT EXPIRATION 
                            if (userPrincipal.AccountExpirationDate.HasValue)
                            { userModel.AccountExpire = Convert.ToDateTime(userPrincipal.AccountExpirationDate.Value).ToString("dd/MM/yyyy HH:mm:ss"); }
                            else if (!userPrincipal.AccountExpirationDate.HasValue || userPrincipal.Enabled == true)
                            { userModel.AccountExpire = "Nigdy"; }
                            else
                            { userModel.AccountExpire = "Brak danych"; }
                        };
                    };
                });
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Błąd pobierania danych", ex.ToString());
            }
        }

        private async Task GetUserGroups()
        {
            try
            {
                await Task.Run(() =>
                {
                    using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain))
                    {
                        using (UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, UserLogin))
                        {
                            List<string> groupsList = userPrincipal.GetGroups().Select(x => x.SamAccountName).ToList();

                            userModel.PrintDeny = groupsList.Contains("#Print_Deny");
                            userModel.PrintColor = groupsList.Contains("#Print_Color");
                            userModel.Devices = groupsList.Contains("#EW_GxDeviceAll");
                            userModel.AirNet = groupsList.Contains("#EW_GxDeviceAll");
                            userModel.Lync = groupsList.Contains("# Lync Pracownicy Etatowi Użytkownicy") || groupsList.Contains("# Lync Partnerzy Użytkownicy");
                            userModel.Internet = groupsList.Contains("Internet");
                            userModel.Developer = groupsList.Contains("#Developer");
                            userModel.AirWatchBasic = groupsList.Contains("AirWatch_view");
                            userModel.AirWatchExpanded = groupsList.Contains("AirWatch_edit");
                            userModel.AirWatchVip = groupsList.Contains("Airwatch_VIP");
                            userModel.BYODCitrix = groupsList.Contains("BYOD-Pulpit-Standard");
                            userModel.BYODHDD = groupsList.Contains("BYOD_HDD_users");
                            userModel.BYODWTG = groupsList.Contains("EBU-Users-WTG");
                            userModel.BYODLVMW7 = groupsList.Contains("BYODVM_W7_users");
                            userModel.BYODLVM10 = groupsList.Contains("BYODVM_W10_users");
                        };
                    };
                });
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Błąd pobierania danych", ex.ToString());
            }
        }

        private async Task GetUserPassword()
        {
            try
            {
                await Task.Run(() =>
                {
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        powerShell.AddScript("[datetime]::FromFileTimeUtc((Get-ADUser " + UserLogin + " -Properties msDS-UserPasswordExpiryTimeComputed | Select -Expand \"msDS-UserPasswordExpiryTimeComputed\"");
                        Collection<PSObject> psObjects;
                        psObjects = powerShell.Invoke();
                        userModel.PasswordExpire = psObjects.FirstOrDefault().ToString();
                    }
                });
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(applicationVM, "Błąd pobierania danych", ex.ToString());
            }
        }
    }
}
