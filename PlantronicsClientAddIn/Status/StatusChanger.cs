using ININ.IceLib.Connection;
using ININ.InteractionClient.AddIn;
using Plantronics.UC.SpokesWrapper;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Status
{
    public class StatusChanger 
    {
        private ISettingsManager _settingsManager;
        private IDeviceManager _deviceManager;
        private ICicStatusService _cicStatusService;
        private Session _icSession = null;

        public StatusChanger(Session icSession, ICicStatusService cicStatusService, IDeviceManager deviceManager, ISettingsManager settingsManager )
        {
            _settingsManager = settingsManager;
            _deviceManager = deviceManager;
            _cicStatusService = cicStatusService;
            _icSession = icSession;

            _deviceManager.PlantronicsDeviceAttached += OnPlantronicsDeviceAttached;
            _deviceManager.HeadsetConnected += OnHeadsetConnected;
            _deviceManager.PlantronicsDeviceDetached += OnPlantronicsDeviceDetached;
            _deviceManager.HeadsetDisconnected += OnHeadsetDisconnected;

        }

        private void OnHeadsetDisconnected(object sender, ConnectedStateArgs e)
        {
            if (_settingsManager.HeadsetDisconnectChangeStatus)
            {
                _cicStatusService.SetStatus(_settingsManager.HeadsetDisconnectStatusKey);
            }

            if (_settingsManager.ShouldLogOutOnHeadsetDisconnect && _icSession != null)
            {
                _icSession.Disconnect();
            }
        }

        private void OnPlantronicsDeviceDetached(object sender, EventArgs e)
        {
            if (_settingsManager.DeviceDisconnectChangeStatus)
            {
                _cicStatusService.SetStatus(_settingsManager.DeviceDisconnectStatusKey);
            }

            if (_settingsManager.ShouldLogOutOnDeviceDisconnect && _icSession != null)
            {
                _icSession.Disconnect();
            }
        }

        private void OnHeadsetConnected(object sender, ConnectedStateArgs e)
        {
            if (_settingsManager.HeadsetConnectChangeStatus)
            {
                _cicStatusService.SetStatus(_settingsManager.HeadsetConnectStatusKey);
            }
        }

        private void OnPlantronicsDeviceAttached(object sender, AttachedArgs e)
        {
            if (_settingsManager.DeviceConnectChangeStatus)
            {
                _cicStatusService.SetStatus(_settingsManager.DeviceConnectStatusKey);
            }
        }
    }
}
