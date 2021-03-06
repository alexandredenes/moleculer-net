﻿using Microsoft.Extensions.Logging;
using Protocol.Abstractions;
using Protocol.Abstractions.Messages;
using ServiceAction.Abstractions;
using System;
using System.Reflection;

namespace Protocol
{
    public class ServiceExecutor : IServiceExecutor
    {
        private readonly ILogger<ServiceExecutor> _logger;
        private readonly LocalActionServicesLoader _localServices;
        private readonly Context _context;

        public ServiceExecutor(ILogger<ServiceExecutor> logger, LocalActionServicesLoader localServices,Context context)
        {
            _logger = logger;
            _localServices = localServices;
            _context = context;
        }

        public object Execute(RequestMessage requestMessage)
        {
            (Type type, MethodInfo info)? action = _localServices.GetExecutionInfo(requestMessage.Action);

            if (!action.HasValue)
                throw new InvalidOperationException($"Action {requestMessage.Action} not found");

            object[] parms = null;

            if (action.Value.info.GetParameters().Length != 0)
                parms = CreateParams(action.Value.info.GetParameters(), requestMessage);

            bool contextConstructor = action.Value.type.GetConstructor(new Type[]{ _context.GetType()}) != null;
            object obj;
            if(contextConstructor)
                obj = Activator.CreateInstance(action.Value.type, new object[] { _context });
            else
                obj = Activator.CreateInstance(action.Value.type, new object[] { });

            return action.Value.info.Invoke(obj, parms);

        }

        private object[] CreateParams(ParameterInfo[] parameterInfo, RequestMessage requestMessage)
        {
            object[] retVal = new object[parameterInfo.Length];
            for(int x=0;x<retVal.Length;x++)
            {
                retVal[x] = Convert.ChangeType(requestMessage.Params[parameterInfo[x].Name],parameterInfo[x].ParameterType);
            }
            return retVal;
        }
    }
}
