using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Protocol.Abstractions.Messages.Converters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Protocol.Abstractions.Messages
{
    public class RequestMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("timeout")]
        public double Timeout { get; set; }

        [JsonProperty("params")]
        public JObject Params { get; set; }

        [JsonProperty("meta")]
        public JObject Meta { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("metrics")]
        public bool Metrics { get; set; }

        [JsonProperty("parentID")]
        public string ParentID { get; set; }

        [JsonProperty("requestID")]
        public string RequestID { get; set; }

        [JsonProperty("ver")]
        public string Ver { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        private RequestMessage() { }

        public static RequestMessage Parse(byte[] data)
        {
            RequestMessage retVal = new RequestMessage();

            JObject obj = JObject.Parse(Encoding.Default.GetString(data));
            retVal.Id = (string)obj["id"];
            retVal.Action = (string)obj["action"];
            retVal.Params = (JObject)obj["params"];
            retVal.Timeout = (double)obj["timeout"];
            retVal.Level = (int)obj["level"];
            retVal.Metrics = (bool)obj["metrics"];
            retVal.ParentID = (string)obj["parentID"];
            retVal.RequestID = (string)obj["requestID"];
            retVal.Ver = (string)obj["ver"];
            retVal.Sender = (string)obj["sender"];
            return retVal;
        }

        public static RequestMessage Create(string action, JObject parms, string contextId)
        {
            RequestMessage retVal = new RequestMessage();
            retVal.Action = action;
            retVal.Id = contextId;
            retVal.Level = 2;
            retVal.Metrics = false;
            retVal.Params = parms==null ? new JObject() : parms;
            retVal.ParentID = $"parent{contextId}";
            retVal.RequestID = Guid.NewGuid().ToString();
            retVal.Ver = "3";
            retVal.Timeout = 15000;
            retVal.Meta = new JObject();
            
            return retVal;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

}
