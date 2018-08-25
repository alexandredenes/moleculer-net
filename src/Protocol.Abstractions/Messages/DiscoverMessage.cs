using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Abstractions.Messages
{
    public class DiscoverMessage
    {
        [JsonProperty("ver")]
        public string Ver { get; private set; }

        [JsonProperty("sender")]
        public string Sender { get; private set; }

        private DiscoverMessage() { }

        public static DiscoverMessage Parse(byte[] data)
        {
            string strData = Encoding.Default.GetString(data); ;
            JObject obj = JObject.Parse(strData);
            DiscoverMessage retVal = new DiscoverMessage()
            {
                Ver = (string)obj["ver"],
                Sender = (string)obj["sender"]
            };
            return retVal;
        }

        public static DiscoverMessage Parse(ServiceInfo serviceInfo)
        {
            DiscoverMessage retVal = new DiscoverMessage()
            {
                Ver = "3",
                Sender = serviceInfo.ServiceName
            };
       
            return retVal;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonConverter[] {
                });
        }
    }
}
