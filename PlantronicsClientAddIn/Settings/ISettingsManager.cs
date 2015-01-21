using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Settings
{
    public interface ISettingsManager
    {
        string DisconnectStatusKey { get; set; }
        bool DisconnectChangeStatus { get; set; }
        bool DisconnectNotification { get; set; }

        string ConnectStatusKey { get; set; }
        bool ConnectChangeStatus { get; set; }
        bool ConnectNotification { get; set; }

        string OutOfRangeStatusKey { get; set; }
        bool OutOfRangeChangeStatus { get; set; }
        bool OutOfRangeNotification { get; set; }

        string InRangeStatusKey { get; set; }
        bool InRangeChangeStatus { get; set; }
        bool InRangeNotification { get; set; }

    }
}
