using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Abstractions.ServiceLocator
{
    public class ServiceEntry
    {
        public string ServiceName { get; private set; }
        public string NodeName { get; private set; }
        public Dictionary<string,string> Params { get; private set; }

        public ServiceEntry(string serviceName, string nodeName)
        {
            ServiceName = serviceName;
            NodeName = nodeName;
            Params = new Dictionary<string, string>();
        }
    }
}
