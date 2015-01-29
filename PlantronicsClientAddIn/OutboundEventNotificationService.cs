using ININ.IceLib.Connection;
using ININ.IceLib.Connection.Extensions;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.Status;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace PlantronicsClientAddIn
{
    class OutboundEventNotificationService
    {
        private const string WebServerUrlParam = "PlantronicsStatusWebServerUrl";
        private const string ShouldSendHandlerNotificationParam = "PlantronicsSendHandlerNotification";

        private Session _session;
        private ICicStatusService _statusService;
        private IDeviceManager _deviceManager;
        private ServerParameters _serverParams;

        public OutboundEventNotificationService(Session session, ICicStatusService statusService, IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            _session = session;
            _statusService = statusService;
            _statusService.UserStatusChanged += OnUserStatusChanged;

            _session.ConnectionStateChanged += OnConnectionStateChanged;

            _deviceManager.HeadsetConnected += OnDeviceEvent;
            _deviceManager.HeadsetDisconnected += OnDeviceEvent;
            _deviceManager.MuteChanged += OnDeviceEvent;
            _deviceManager.PlantronicsDeviceAttached += OnDeviceEvent;
            _deviceManager.PlantronicsDeviceDetached += OnDeviceEvent;

            _serverParams = new ServerParameters(_session);
            _serverParams.StartWatching(new []{ShouldSendHandlerNotificationParam, WebServerUrlParam});

            PostToWebService();
            
        }

        private void OnUserStatusChanged(object sender, EventArgs e)
        {
            PostToWebService();
        }

        private void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            PostToWebService();
        }

        private void OnDeviceEvent(object sender, EventArgs e)
        {
            PostToWebService();
        }

        private string GetServerParameter(string key)
        {
            var serverParam = _serverParams.GetCachedServerParameter(key);

            if (serverParam != null)
            {
                return serverParam.Value;
            }

            return String.Empty;

        }

        private void PostToWebService()
        {
            var url = GetServerParameter(WebServerUrlParam);
            if (String.IsNullOrEmpty(url))
            {
                return;
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/statuschange");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                bool isConnected = _session.ConnectionState == ConnectionState.Up;

                string json = new JavaScriptSerializer().Serialize(new
                {
                    userId = _session.UserId,
                    station = isConnected ? _session.GetStationInfo().Id : String.Empty,
                    status = isConnected ? _statusService.GetStatus().StatusMessageDetails.MessageText : String.Empty,
                    loggedIn = isConnected ,
                    onPhone = isConnected ?  _statusService.GetStatus().OnPhone : false,
                    headsetConnected = _deviceManager.IsHeadsetConnected,
                    device = _deviceManager.IsDeviceConnected ? _deviceManager.ProductName : String.Empty,
                    serial = _deviceManager.IsDeviceConnected ? _deviceManager.SerialNumber : String.Empty,
                    isMuted = _deviceManager.IsDeviceConnected ? _deviceManager.IsHeadsetMuted : false,
                });

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
        }
    }
}
