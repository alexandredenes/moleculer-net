using ServiceAction.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Protocol
{
    public class LocalActionServicesLoader
    {
        private Dictionary<string, (Type type, MethodInfo method)> ActionCache = new Dictionary<string, (Type type, MethodInfo method)>();

        public LocalActionServicesLoader()
        {
            string[] fileEntries = Directory.GetFiles("plugin");
            foreach (string fileEntry in fileEntries)
            {
                Assembly assembly = Assembly.LoadFrom(fileEntry);
                foreach (Type t in assembly.GetTypes())
                {
                    {
                        ServiceActionAttribute serviceAttr = (ServiceActionAttribute)t.GetCustomAttribute(typeof(ServiceActionAttribute));
                        if (serviceAttr != null)
                        {
                            string serviceName;
                            if (serviceAttr.Name != null)
                                serviceName = serviceAttr.Name;
                            else
                                serviceName = t.Name;

                            foreach (MethodInfo m in t.GetMethods())
                            {
                                if (m.DeclaringType.Equals(t))
                                {
                                    string actionName = serviceName + ".";

                                    ServiceMethodAttribute serviceMethod = (ServiceMethodAttribute)m.GetCustomAttribute(typeof(ServiceMethodAttribute));
                                    if (serviceMethod != null && serviceMethod.Name != null)
                                        actionName += serviceMethod.Name;
                                    else
                                        actionName += m.Name;

                                    ActionCache[actionName] = (type: t, method: m);
                                }
                            }
                        }
                    }
                }
            }

        }

        public (Type type, MethodInfo info)? GetExecutionInfo(string actionName)
        {
            if (ActionCache.ContainsKey(actionName))
                return ActionCache[actionName];
            return null;
        }

        public List<string> GetActions()
        {
            return ActionCache.Keys.ToList();
        }
    }
}
