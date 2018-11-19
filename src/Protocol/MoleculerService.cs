using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Protocol.Abstractions;
using Protocol.Abstractions.Messages;
using Protocol.Abstractions.ServiceLocator;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Protocol
{
    public class MoleculerService : IHostedService
    {
        private const int HEARTBEAT_PERIOD = 4000;

        private readonly ILogger _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly ITransporter _transporter;
        private readonly LocalActionServicesLoader _localServices;
        private readonly IServiceExecutor _executor;
        private readonly ServiceInfo _serviceInfo;
        private System.Timers.Timer _heartbeat;
        private readonly IServiceLocator _serviceLocator;

        public MoleculerService(ILogger<MoleculerService> logger, IApplicationLifetime appLifetime, ITransporter transporter, LocalActionServicesLoader localServices, IServiceExecutor executor, IServiceLocator serviceLocator)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _transporter = transporter;
            _localServices = localServices;
            _executor = executor;
            _serviceLocator = serviceLocator;

            _serviceInfo = new ServiceInfo();
            FillHostInfo();

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("StartAsync invoked");

            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStoped);

            return Task.CompletedTask;
        }

        private void OnStoped()
        {
            _logger.LogInformation("MoleculerService stoped");
        }

        private void OnStopping()
        {
            _logger.LogInformation("MoleculerService requested stop");
            _transporter.Stop();
        }

        private void OnStarted()
        {
            _logger.LogInformation("MoleculerService starting");

            _transporter.HeartbeatReceived += _transporter_HeartbeatReceived;
            _transporter.RequestReceived += _transporter_RequestReceived;
            _transporter.DiscoverReceived += _transporter_DiscoverReceived;
            _transporter.InfoReceived += _transporter_InfoReceived;


            _transporter.Start(_serviceInfo);

            _logger.LogInformation("MoleculerService started");

            _heartbeat = new System.Timers.Timer(HEARTBEAT_PERIOD);
            _heartbeat.Elapsed += _heartbeat_Elapsed;
            _heartbeat.Start();

            DiscoverMessage discoverMessage = DiscoverMessage.Parse(_serviceInfo);
            _transporter.Publish("MOL.DISCOVER", discoverMessage);
        }

        private void _transporter_InfoReceived(object sender, InfoMessage e)
        {

            if ("MOL.INFO." + _serviceInfo.ServiceName ==(string)sender)
            {
                InfoMessage infoMessage = InfoMessage.Parse(_serviceInfo);
                _transporter.Publish("MOL.INFO", infoMessage);
            }
            _serviceLocator.UpdateNodeInfo(e);
        }

        private void _heartbeat_Elapsed(object sender, ElapsedEventArgs e)
        {
            HeartbeatMessage heartbeat = HeartbeatMessage.Parse(_serviceInfo);
            _transporter.Publish("MOL.HEARTBEAT", heartbeat);

        }

        private void _transporter_DiscoverReceived(object sender, DiscoverMessage e)
        {
            _logger.LogDebug("Discover message received: " + e.ToString());
            if (!e.Sender.Equals(_serviceInfo.ServiceName))
            {
                InfoMessage infoMessage = InfoMessage.Parse(_serviceInfo);
                _transporter.Publish("MOL.INFO." + e.Sender, infoMessage);
            }
        }

        private void _transporter_RequestReceived(object sender, RequestMessage reqMessage)
        {
            _logger.LogDebug("Received Request");
            ResponseMessage retMessage;
            try
            {
                object response = _executor.Execute(reqMessage);
                retMessage = ResponseMessage.ParseResult(_serviceInfo, reqMessage, response);
            }
            catch (Exception e)
            {
                retMessage = ResponseMessage.ParseError(_serviceInfo, reqMessage, e);
            }
            _logger.LogDebug($"Will return to MOL.RES.{reqMessage.Sender}");
            _logger.LogDebug(retMessage.ToString());
            _logger.LogDebug("End Will return");

            _transporter.Publish($"MOL.RES.{reqMessage.Sender}", retMessage);
        }

        private void _transporter_HeartbeatReceived(object sender, HeartbeatMessage e)
        {
            _logger.LogDebug("Heartbeat received");
        }

        private void FillHostInfo()
        {
            _serviceInfo.HostName = Environment.MachineName;
            _serviceInfo.ServiceName = _serviceInfo.HostName + "-" + (new Random().Next(100000) + 1);
            var host = Dns.GetHostEntry(Dns.GetHostName());
            _serviceInfo.IPList = host.AddressList;

            _serviceInfo.LocalServices = _localServices.GetActions();

            InfoMessage localinfo = InfoMessage.Parse(_serviceInfo);
            _serviceLocator.UpdateNodeInfo(localinfo);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("StopAsync invoked");
            return Task.CompletedTask;
        }
    }
}
