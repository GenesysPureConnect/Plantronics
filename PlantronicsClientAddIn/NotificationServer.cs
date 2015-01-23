using ININ.InteractionClient.AddIn;
using Plantronics.UC.SpokesWrapper;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.Settings;
using PlantronicsClientAddIn.Status;
using System;

namespace PlantronicsClientAddIn
{
    public class NotificationServer
    {
        private ISettingsManager _settingsManager;
        private IDeviceManager _deviceManager;
        private INotificationService _addinNotificationService;
        public NotificationServer( IDeviceManager deviceManager, ISettingsManager settingsManager, INotificationService addinNotificationService)
        {
            _addinNotificationService = addinNotificationService;
            _deviceManager = deviceManager;
            _settingsManager = settingsManager;

            _deviceManager.PlantronicsDeviceAttached += OnPlantronicsDeviceAttached;
            _deviceManager.HeadsetConnected += OnHeadsetConnected;
            _deviceManager.PlantronicsDeviceDetached += OnPlantronicsDeviceDetached;
            _deviceManager.HeadsetDisconnected += OnHeadsetDisconnected;

        }

        void OnHeadsetDisconnected(object sender, ConnectedStateArgs e)
        {
            if (_settingsManager.HeadsetConnectNotification)
            {
                _addinNotificationService.Notify("Headset disconnected", "Headset", NotificationType.Warning, TimeSpan.FromSeconds(3));
            }
        }

        void OnPlantronicsDeviceDetached(object sender, EventArgs e)
        {
            if (_settingsManager.DeviceDisconnectNotification)
            {
                _addinNotificationService.Notify("Plantronics device detached", "Headset", NotificationType.Warning, TimeSpan.FromSeconds(3));
            }
        }

        void OnHeadsetConnected(object sender, ConnectedStateArgs e)
        {
            if (_settingsManager.HeadsetConnectNotification)
            {
                _addinNotificationService.Notify("Headset connected", "Headset", NotificationType.Info, TimeSpan.FromSeconds(2));
            }
        }

        void OnPlantronicsDeviceAttached(object sender, AttachedArgs e)
        {
            if (_settingsManager.DeviceConnectNotification)
            {
                _addinNotificationService.Notify("Plantronics device attached", "Headset", NotificationType.Info, TimeSpan.FromSeconds(2));
            }
        }
    }
}
