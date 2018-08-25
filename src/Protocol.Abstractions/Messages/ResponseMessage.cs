using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Protocol.Abstractions.Messages.Converters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Protocol.Abstractions.Messages
{
    public class ResponseMessage
    {
        [JsonProperty("ver")]
        public string Ver { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public object Data;

        [JsonProperty("error")]
        public object Error;

        [JsonProperty("meta")]
        public object Meta;

        private ResponseMessage() { }

        public static ResponseMessage ParseResult(ServiceInfo serviceInfo, RequestMessage req, object value)
        {
            ResponseMessage retVal = new ResponseMessage();

            retVal.Ver = req.Ver;
            retVal.Sender = serviceInfo.ServiceName;
            retVal.Id = req.Id;
            retVal.Success = true;
            retVal.Data = value;
            retVal.Error = new object();
            retVal.Meta = new object();

            return retVal;

        }

        public static ResponseMessage ParseError(ServiceInfo serviceInfo, RequestMessage req, Exception value)
        {
            ResponseMessage retVal = new ResponseMessage();

            retVal.Ver = req.Ver;
            retVal.Sender = serviceInfo.ServiceName;
            retVal.Id = req.Id;
            retVal.Success = false;
            retVal.Error = value.Message;
            retVal.Data = new object();
            retVal.Meta = new object();

            return retVal;

        }

        public static ResponseMessage Parse(byte[] data)
        {
            ResponseMessage retVal = new ResponseMessage();

            JObject obj = JObject.Parse(Encoding.Default.GetString(data));
            retVal.Ver = (string)obj["ver"];
            retVal.Sender = (string)obj["sender"];
            retVal.Id = (string)obj["id"];
            retVal.Success = (bool)obj["success"];
            retVal.Data = obj["data"];
            retVal.Error = obj["error"];
            retVal.Meta = obj["meta"];
            return retVal;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

}
