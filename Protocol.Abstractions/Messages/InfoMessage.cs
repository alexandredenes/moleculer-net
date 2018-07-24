using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Protocol.Abstractions.Messages.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Protocol.Abstractions.Messages
{
    public class InfoMessage
    {
        [JsonProperty("ver")]
        public string Ver { get; private set; }

        [JsonProperty("sender")]
        public string Sender { get; private set; }

        [JsonProperty("services")]
        public List<Service> Services { get; private set; }

        [JsonProperty("config")]
        public string Config { get; private set; }

        [JsonProperty("ipList")]
        public IPAddress[] IPList { get; private set; }

        [JsonProperty("hostname")]
        public string HostName { get; private set; }

        [JsonProperty("client")]
        public Dictionary<string,string> Client { get; set; }


        private InfoMessage() { }

        public static InfoMessage Parse(ServiceInfo serviceInfo)
        {
            InfoMessage retVal = new InfoMessage();
            retVal.Ver = "3";
            retVal.Sender = serviceInfo.ServiceName;
            retVal.IPList = serviceInfo.IPList;
            retVal.HostName = serviceInfo.HostName;
            retVal.Client = new Dictionary<string, string>();
            retVal.Client["type"] = "c#";
            retVal.Client["version"] = "2.1";
            retVal.Client["langVersion"] = "7";

            retVal.Services = LoadServices(serviceInfo);

            return retVal;
        }

        private static List<Service> LoadServices(ServiceInfo serviceInfo)
        {
            var retVal = new List<Service>();
            foreach (string s in serviceInfo.LocalServices)
            {
                string serviceName = s.Split('.')[0];
                Service aux = retVal.SingleOrDefault(x => x.Name == serviceName);
                if (aux == null)
                {
                    aux = new Service();
                    aux.Actions = new Dictionary<string, Service.Action>();
                    aux.Name = serviceName;
                    retVal.Add(aux);
                    aux.Settings = new object();
                    aux.Metadata = new object();
                }
                Service.Action action = new Service.Action();
                action.Name = s;
                action.Cache = false;
                action.Metric = new Service.Action.Metrics();
                action.Params = new Dictionary<string, string>();
                action.Params["parametro"] = "string";
                aux.Actions[action.Name] = action;
            }
            return retVal;

        }

        public static InfoMessage Parse(byte[] data)
        {
            JObject obj = JObject.Parse(Encoding.Default.GetString(data));
            InfoMessage retVal  = new InfoMessage();
            JObject client = (JObject)obj["client"];
            retVal.Client = new Dictionary<string, string>();
            foreach(var x in client.Properties())
            {
                retVal.Client[x.Name] = (string)x.Value;
            }
            retVal.HostName = (string)obj["hostname"];
            retVal.IPList = new IPAddress[obj["ipList"].Count()];
            for(int x = 0; x < retVal.IPList.Length ;x++)
            {
                retVal.IPList[x] = IPAddress.Parse((string)obj["ipList"][x]);
            }
            retVal.Sender = (string)obj["sender"];
            retVal.Services = new List<Service>();
            foreach(JObject serviceElement in obj["services"])
            {
                Service serv = new Service();
                serv.Actions = new Dictionary<string, Service.Action>();
                foreach(var actionElement in ((JObject)serviceElement["actions"]).Properties())
                {
                    Service.Action act = new Service.Action();
                    act.Cache = (bool)actionElement.Value["cache"];
                    act.Metric = new Service.Action.Metrics();
                    act.Metric.Meta = (bool)actionElement.Value["metrics"]["meta"];
                    act.Metric.Params = (bool)actionElement.Value["metrics"]["params"];
                    act.Name = actionElement.Name;
                    act.Params = new Dictionary<string, string>();
                    if (actionElement.Value["params"] != null)
                    {
                        foreach (var paramElement in ((JObject)actionElement.Value["params"]).Properties())
                        {
                            act.Params[paramElement.Name] = paramElement.Value.HasValues ? (string)paramElement.Value["type"] : (string)paramElement.Value;
                        }
                    }
                    
                    serv.Actions[actionElement.Name] = act;
                }
                serv.Metadata = null;
                serv.Name = (string)serviceElement["name"];
                serv.Settings = null;
                retVal.Services.Add(serv);
            }
            retVal.Ver = (string)obj["ver"];

            return retVal;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonConverter[] {
                    new IPAddressConverter()
                });
        }

        public class Service
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("settings")]
            public object Settings { get; set; }

            [JsonProperty("metadata")]
            public object Metadata { get; set; }

            [JsonProperty("actions")]
            public Dictionary<string,Action> Actions { get; set; }

            public class Action
            {
                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("params")]
                public Dictionary<string, string> Params { get; set; }

                [JsonProperty("cache")]
                public bool Cache { get; set; }

                [JsonProperty("metrics")]
                public Metrics Metric { get; set; }

                public class Metrics
                {
                    [JsonProperty("params")]
                    public bool Params { get; set; }

                    [JsonProperty("meta")]
                    public bool Meta { get; set; }
                }
            }

        }

    }

}
