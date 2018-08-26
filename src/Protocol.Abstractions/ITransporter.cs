using Protocol.Abstractions.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Abstractions
{
    public interface ITransporter
    {
        event EventHandler<HeartbeatMessage> HeartbeatReceived;
        event EventHandler<RequestMessage> RequestReceived;
        event EventHandler<DiscoverMessage> DiscoverReceived;
        event EventHandler<InfoMessage> InfoReceived;
        event EventHandler<ResponseMessage> ResponseReceived;

        void Start(ServiceInfo serviceInfo);
        void Stop();
        void Publish(string channel, ResponseMessage retMessage);
        void Publish(string channel, InfoMessage retMessage);
        void Publish(string channel, DiscoverMessage retMessage);
        void Publish(string channel, HeartbeatMessage retMessage);
        void Publish(string channel, RequestMessage reqMessage);
    }
}
