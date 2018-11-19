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
            string[] fileEntries = Directory.GetFiles(PLUGINS_FOLDER_PATH);

            foreach (string fileEntry in fileEntries)
            {
                if(!fileEntry.EndsWith(".dll")) continue;
                
                Assembly assembly = Assembly.LoadFrom(fileEntry);

                foreach (Type type in assembly.GetTypes())
                {
                    ServiceActionAttribute serviceAttr = (ServiceActionAttribute)type.GetCustomAttribute(typeof(ServiceActionAttribute));

                    if (serviceAttr != null)
                    {
                        string serviceName = (serviceAttr.Name != null) ? serviceAttr.Name : type.Name;

                        foreach (var methodInfo in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (methodInfo.DeclaringType.Equals(type))
                            {
                                ServiceMethodAttribute serviceMethod = (ServiceMethodAttribute)methodInfo.GetCustomAttribute(typeof(ServiceMethodAttribute));
                                string actionName = serviceMethod != null && serviceMethod.Name != null ? serviceMethod.Name : methodInfo.Name;
                                string actionIndex = $"{serviceName}.{actionName}";

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

        public List<(string,Dictionary<string,string>)>GetActions()
        {
            List<(string name,Dictionary<string,string> parms)> retVal = new List<(string,Dictionary<string,string>)>();
            foreach(string action in ActionCache.Keys.ToList()){
                MethodInfo theMethod = ActionCache[action].method;
                Dictionary<string,string> theParams = new Dictionary<string,string>();
                 foreach(ParameterInfo pInfo in theMethod.GetParameters()) {
                    theParams[pInfo.Name] = ConvertParam(pInfo.ParameterType);
                }
                retVal.Add((action,theParams));
            }
            return retVal;
        }

        private string ConvertParam(Type parameterType)
        {
            if(parameterType.Equals(typeof(string)))
                return "string";
            return "number";
        }
    }
}
