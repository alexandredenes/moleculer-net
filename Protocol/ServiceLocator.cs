using Protocol.Abstractions.Messages;
using Protocol.Abstractions.ServiceLocator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Protocol
{
    public class DumbServiceLocator : IServiceLocator
    {
        private List<ServiceEntry> ServiceEntries = new List<ServiceEntry>();

        public List<ServiceEntry> GetAllServiceEntries()
        {
            List<ServiceEntry> retVal = new List<ServiceEntry>();
            retVal.AddRange(ServiceEntries);

            return retVal;
        }

        public List<ServiceEntry> GetServiceEntry(string serviceName)
        {
            return ServiceEntries.Where(x => x.ServiceName == serviceName).ToList();
        }

        public void UpdateNodeInfo(InfoMessage info)
        {
            ServiceEntries.RemoveAll(x => x.NodeName == info.Sender);

            foreach(var service in info.Services)
            {
                foreach(var action in service.Actions)
                {
                    ServiceEntry entry = new ServiceEntry(action.Key,info.Sender);
                    foreach(var key in action.Value.Params.Keys)
                    {
                        entry.Params[key] = action.Value.Params[key];
                    }
                    ServiceEntries.Add(entry);
                }
            }

        }
    }
}
