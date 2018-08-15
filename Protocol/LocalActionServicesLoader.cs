using ServiceAction.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Protocol
{
    public class LocalActionServicesLoader
    {
        private const string PLUGINS_FOLDER_PATH = "plugin";

        private Dictionary<string, (Type type, MethodInfo method)> ActionCache = new Dictionary<string, (Type type, MethodInfo method)>();

        public LocalActionServicesLoader()
        {
            var fileEntries = Directory.GetFiles(PLUGINS_FOLDER_PATH);

            foreach (string fileEntry in fileEntries)
            {
                var assembly = Assembly.LoadFrom(fileEntry);

                foreach (Type type in assembly.GetTypes())
                {
                    var serviceAttr = (ServiceActionAttribute)type.GetCustomAttribute(typeof(ServiceActionAttribute));

                    if (serviceAttr != null)
                    {
                        var serviceName = (serviceAttr.Name != null) ? serviceAttr.Name : type.Name;

                        foreach (var methodInfo in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (methodInfo.DeclaringType.Equals(type))
                            {
                                var serviceMethod = (ServiceMethodAttribute)methodInfo.GetCustomAttribute(typeof(ServiceMethodAttribute));
                                var actionName = serviceMethod != null && serviceMethod.Name != null ? serviceMethod.Name : methodInfo.Name;
                                var actionIndex = $"{serviceName}.{actionName}";

                                ActionCache[actionIndex] = (type: type, method: methodInfo);
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
