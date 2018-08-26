using Protocol.Abstractions.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Abstractions.ServiceLocator
{
    public interface IServiceLocator
    {
        void UpdateNodeInfo(InfoMessage info);
        IList<ServiceEntry> GetServiceEntry(string serviceName);
        IList<ServiceEntry> GetAllServiceEntries();
    }
}
