using Microsoft.Extensions.Logging;
using NATS.Client;
using Protocol.Abstractions;
using Protocol.Abstractions.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace Protocol.NATS
{
    public class NATSTransporter : ITransporter
    {
        private IConnection _conn;
        private IAsyncSubscription _molRequestSubscription;
        private IAsyncSubscription _molResponseSubscription;
        private IAsyncSubscription _molDiscoverySubscription;
        private IAsyncSubscription _molInfoSubscription;
        private IAsyncSubscription _molTargetedInfoSubscription;
        private IAsyncSubscription _molHeartbeatSubscription;

        private Timer _heartbeat;

        ServiceInfo _serviceInfo;
        private readonly ILogger _logger;

        public event EventHandler<HeartbeatMessage> HeartbeatReceived;
        public event EventHandler<RequestMessage> RequestReceived;
        public event EventHandler<DiscoverMessage> DiscoverReceived;
        public event EventHandler<InfoMessage> InfoReceived;
        public event EventHandler<ResponseMessage> ResponseReceived;

        public NATSTransporter(ILogger<ITransporter> logger)
        {
            _logger = logger;
        }



        public void Start(ServiceInfo serviceInfo)
        {
            _serviceInfo = serviceInfo;

            // Create a new connection factory to create
            // a connection.
            ConnectionFactory cf = new ConnectionFactory();

            // Creates a live connection to the default
            // NATS Server running locally
            _conn = cf.CreateConnection();

            BeginMolDiscoveryListener();
            BeginRequestListener();
            BeginResponseListener();
            BeginMolInfoListener();
            BeginHeartBeatingListener();

            _logger.LogInformation("NATS Transporter started");
        }

        private void BeginRequestListener()
        {
            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                if(RequestReceived == null)
                {
                    _logger.LogError("No RequestReceivedEventHandler defined");
                }
                else
                {
                    RequestMessage msg = RequestMessage.Parse(args.Message.Data);
                    RequestReceived("MOL.REQ." + _serviceInfo.ServiceName, msg);
                }
            };

            _molRequestSubscription = _conn.SubscribeAsync("MOL.REQ."+_serviceInfo.ServiceName, handler);
        }

        private void BeginResponseListener()
        {
            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                if (ResponseReceived == null)
                {
                    _logger.LogError("No RequestReceivedEventHandler defined");
                }
                else
                {
                    ResponseMessage msg = ResponseMessage.Parse(args.Message.Data);
                    ResponseReceived("MOL.RES." + _serviceInfo.ServiceName, msg);
                }
            };

            _molResponseSubscription = _conn.SubscribeAsync("MOL.RES." + _serviceInfo.ServiceName, handler);
        }

        private void BeginHeartBeatingListener()
        {
            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                if(HeartbeatReceived == null)
                {
                    _logger.LogError("No HeatbeatEventHandler defined");
                }
                else
                {
                    HeartbeatReceived("MOL.HEARTBEAT", null);
                }
            };
            _molHeartbeatSubscription = _conn.SubscribeAsync("MOL.HEARTBEAT", handler);

            _heartbeat = new Timer(4000);
            _heartbeat.Elapsed += _heartbeat_Elapsed;
            _heartbeat.Start();
        }

        private void _heartbeat_Elapsed(object sender, ElapsedEventArgs e)
        {
            HeartbeatMessage heartbeat = HeartbeatMessage.Parse(_serviceInfo);
            _conn.Publish("MOL.HEARTBEAT", Encoding.Default.GetBytes(heartbeat.ToString()));

        }

        private void BeginMolDiscoveryListener()
        {             
            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                if (DiscoverReceived == null)
                {
                    _logger.LogError("No DiscoverEventHandler defined");
                }
                else
                {
                    DiscoverMessage msg = DiscoverMessage.Parse(args.Message.Data);
                    DiscoverReceived("MOL.DISCOVER", msg);
                }
            };

            _molDiscoverySubscription = _conn.SubscribeAsync("MOL.DISCOVER", handler);
        }

        private void BeginMolInfoListener()
        {   
            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                AsyncSubscription aux = sender as AsyncSubscription;
                if (InfoReceived == null)
                {
                    _logger.LogError("No InfoEventHandler defined");
                }
                else
                {
                    InfoMessage msg = InfoMessage.Parse(args.Message.Data);
                    InfoReceived(aux.Subject, msg);
                }

            };

            _molInfoSubscription = _conn.SubscribeAsync("MOL.INFO", handler);
            _molTargetedInfoSubscription = _conn.SubscribeAsync("MOL.INFO." + _serviceInfo.ServiceName, handler);
        }


        public void Stop()
        {
            _heartbeat.Stop();
            _molDiscoverySubscription.Unsubscribe();
            _molInfoSubscription.Unsubscribe();
            _molTargetedInfoSubscription.Unsubscribe();
            _molRequestSubscription.Unsubscribe();
            _molResponseSubscription.Unsubscribe();

            _conn.Close();
            _logger.LogInformation("NATS Transporter stoped");

        }

        public void Publish(string channel, ResponseMessage retMessage)
        {
            _conn.Publish(channel, Encoding.Default.GetBytes(retMessage.ToString()));
        }

        public void Publish(string channel, InfoMessage retMessage)
        {
            _conn.Publish(channel, Encoding.Default.GetBytes(retMessage.ToString()));
        }

        public void Publish(string channel, DiscoverMessage retMessage)
        {
            _conn.Publish(channel, Encoding.Default.GetBytes(retMessage.ToString()));
        }

        public void Publish(string channel, HeartbeatMessage retMessage)
        {
            _conn.Publish(channel, Encoding.Default.GetBytes(retMessage.ToString()));
        }

        public void Publish(string channel, RequestMessage reqMessage)
        {
            reqMessage.Sender = _serviceInfo.ServiceName;
            
            _conn.Publish(channel, Encoding.Default.GetBytes(reqMessage.ToString()));
        }
    }
}
