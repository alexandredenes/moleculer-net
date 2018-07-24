using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Protocol.Abstractions.Messages
{
    public class HeartbeatMessage
    {
        public string Ver { get; private set; }
        public string Sender { get; private set; }
        public double Cpu { get; private set; }


        private HeartbeatMessage() { }

        public static HeartbeatMessage Parse(ServiceInfo serviceInfo)
        {
            HeartbeatMessage retVal = new HeartbeatMessage();
            retVal.Ver = "3";
            retVal.Sender = serviceInfo.ServiceName;
            retVal.Cpu = 50;

            return retVal;
        }

        public override string ToString()
        {
            JObject obj = new JObject();
            obj["ver"] = Ver;
            obj["sender"] = Sender;
            obj["cpu"] = Cpu;

            return obj.ToString();
        }
    }
}
