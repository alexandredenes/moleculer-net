using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Protocol.Abstractions

{
    public class ServiceInfo
    {
        public string ServiceName { get; set; }
        public IPAddress[] IPList { get; set; }
        public string HostName { get; set; }
        public List<string> LocalServices { get; set; }
    }
}
