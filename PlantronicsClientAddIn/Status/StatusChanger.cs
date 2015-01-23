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

        public StatusChanger(ICicStatusService cicStatusService, IDeviceManager deviceManager, ISettingsManager settingsManager )
        {
            _settingsManager = settingsManager;
            _deviceManager = deviceManager;
            _cicStatusService = cicStatusService;

            _deviceManager.PlantronicsDeviceAttached += OnPlantronicsDeviceAttached;
            _deviceManager.HeadsetConnected += OnHeadsetConnected;
            _deviceManager.PlantronicsDeviceDetached += OnPlantronicsDeviceDetached;
            _deviceManager.HeadsetDisconnected += OnHeadsetDisconnected;

        }

        void OnHeadsetDisconnected(object sender, ConnectedStateArgs e)
        {
            if (_settingsManager.HeadsetDisconnectChangeStatus)
            {
                _cicStatusService.SetStatus(_settingsManager.HeadsetDisconnectStatusKey);
            }
        }

        void OnPlantronicsDeviceDetached(object sender, EventArgs e)
        {
            if (_settingsManager.DeviceDisconnectChangeStatus)
            {
                _cicStatusService.SetStatus(_settingsManager.DeviceDisconnectStatusKey);
            }
        }

        void OnHeadsetConnected(object sender, ConnectedStateArgs e)
        {
            if (_settingsManager.HeadsetConnectChangeStatus)
            {
                _cicStatusService.SetStatus(_settingsManager.HeadsetConnectStatusKey);
            }
        }

        void OnPlantronicsDeviceAttached(object sender, AttachedArgs e)
        {
            if (_settingsManager.DeviceConnectChangeStatus)
            {
                _cicStatusService.SetStatus(_settingsManager.DeviceConnectStatusKey);
            }
        }
    }
}
