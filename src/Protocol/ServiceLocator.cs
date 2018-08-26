using Protocol.Abstractions.Messages;
using Protocol.Abstractions.ServiceLocator;
using System.Collections.Generic;
using System.Linq;

namespace Protocol
{
    public class ServiceLocator : IServiceLocator
    {
        private List<ServiceEntry> ServiceEntries = new List<ServiceEntry>();

        public IList<ServiceEntry> GetAllServiceEntries()
        {
            List<ServiceEntry> retVal = new List<ServiceEntry>();
            retVal.AddRange(ServiceEntries);

            return retVal;
        }

        public IList<ServiceEntry> GetServiceEntry(string serviceName)
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
