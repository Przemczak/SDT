using System.ComponentModel;

namespace SDT.Models
{
    public class User
    {
        public string UserLogin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Employment { get; set; }
        public string Company { get; set; }
        public string Manager { get; set; }
        public string FunctionalManager { get; set; }
        public string IFS { get; set; }
        public string Mail { get; set; }
        public string MailStatus { get; set; }
        public string MailSIP { get; set; }
        public string MailClass { get; set; }
        public string MailBPTP { get; set; }
        public string MailQuota { get; set; }
        public string PasswordExpire { get; set; }
        public string PasswordLastSet { get; set; }
        public string AccountStatus { get; set; }
        public string AccountExpire { get; set; }
        public string AccountCreated { get; set; }
        public bool PrintDeny { get; set; }
        public bool PrintColor { get; set; }
        public bool Devices { get; set; }
        public bool AirNet { get; set; }
        public bool Lync { get; set; }
        public bool Internet { get; set; }
        public bool Developer { get; set; }
        public bool AirWatchBasic { get; set; }
        public bool AirWatchExpanded { get; set; }
        public bool AirWatchVip { get; set; }
        public bool BYODCitrix { get; set; }
        public bool BYODHDD { get; set; }
        public bool BYODWTG { get; set; }
        public bool BYODLVMW7 { get; set; }
        public bool BYODLVM10 { get; set; }
    }
}
