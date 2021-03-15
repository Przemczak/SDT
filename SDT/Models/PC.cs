namespace SDT.Models
{
    public class PC
    {
        public string PcName { get; set; }
        public bool PcPort135 { get; set; }
        public bool PcPort445 { get; set; }
        public bool PcPort2701 { get; set; }
        public bool PcPort8081 { get; set; }
        public bool PcJoulex { get; set; }
        public bool PcInfoNoc { get; set; }
        public bool PcAlterBrowser { get; set; }
        public bool PcProxyBSTB { get; set; }
        public bool PcIntBSTB { get; set; }
        public string PcIP { get; set; }
        public string PcMAC { get; set; }
        public string PcNS { get; set; }
        public string PcLoggedUser { get; set; }
        public string PcFreeSpace { get; set; }
        public string PcSystem { get; set; }
        public string PcSystemVersion { get; set; }
        public string PcSystemUpdate { get; set; }
    }
}
