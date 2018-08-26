using Newtonsoft.Json.Linq;
using Protocol.Abstractions;
using Protocol.Abstractions.Messages;
using Protocol.Abstractions.ServiceLocator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceAction.Abstractions
{
    public class Context
    {
        private ITransporter _transporter;
        private IServiceLocator _locator;

        private static Dictionary<string, Semaphore> Semaphores = new Dictionary<string, Semaphore>();
        private static Dictionary<string, object> Values = new Dictionary<string, object>();

        public Context(ITransporter transporter,IServiceLocator locator)
        {
            _transporter = transporter;
            _locator = locator;
            _transporter.ResponseReceived += _transporter_ResponseReceived;
        }

        public object Call(string actionName, JObject parms, JObject opts)
        {
                ServiceEntry entry = _locator.GetServiceEntry(actionName)[0];
                if (entry == null)
                    throw new InvalidOperationException($"service {actionName} not found");

                string newGuid = Guid.NewGuid().ToString();
                Semaphores[newGuid] = new Semaphore(0, 1);
                RequestMessage reqMessage = RequestMessage.Create(actionName, parms, newGuid);
                string obj = reqMessage.ToString();
                _transporter.Publish($"MOL.REQB.{entry.ServiceName}", reqMessage);
                Semaphores[newGuid].WaitOne();
                return Values[newGuid];

        }

        private void _transporter_ResponseReceived(object sender, Protocol.Abstractions.Messages.ResponseMessage response)
        {
            if(response.Success)
            {
                
                Values[response.Id] = Convert((JValue)response.Data);
                Semaphores[response.Id].Release();
            }
            else
                throw new NotImplementedException();
        }

        private object Convert(JValue value)
        {
            switch(value.Type)
            {
                case JTokenType.Integer:
                    return value.ToObject<int>();
                default:
                    return value.ToObject<object>();
            }
        }


    }
}
