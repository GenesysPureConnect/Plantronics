using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Settings
{
    public interface ISettingsManager
    {
        string DeviceDisconnectStatusKey { get; set; }
        bool DeviceDisconnectChangeStatus { get; set; }
        bool DeviceDisconnectNotification { get; set; }

        string DeviceConnectStatusKey { get; set; }
        bool DeviceConnectChangeStatus { get; set; }
        bool DeviceConnectNotification { get; set; }

        string HeadsetDisconnectStatusKey { get; set; }
        bool HeadsetDisconnectChangeStatus { get; set; }
        bool HeadsetDisconnectNotification { get; set; }

        string HeadsetConnectStatusKey { get; set; }
        bool HeadsetConnectChangeStatus { get; set; }
        bool HeadsetConnectNotification { get; set; }

    }
}
