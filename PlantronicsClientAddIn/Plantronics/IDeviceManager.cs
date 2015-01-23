using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plantronics.UC.SpokesWrapper;

namespace PlantronicsClientAddIn.Plantronics
{
    public interface IDeviceManager: IDisposable
    {

        /// <summary>
        /// Triggered when the user attached a Plantronics device
        /// </summary>
        event Spokes.AttachedEventHandler PlantronicsDeviceAttached;

        /// <summary>
        /// Triggered when the user detaches a Plantronics device
        /// </summary>
        event Spokes.DetachedEventHandler PlantronicsDeviceDetached;

        /// <summary>
        /// Triggered when a CC Plantronics headset is connected to the amplifier (QD connecter)
        /// </summary>
        event Spokes.ConnectedEventHandler HeadsetConnected;

        /// <summary>
        /// Triggered when a CC Plantronics headset is disconnected from the amplifier (QD connecter)
        /// </summary>
        event Spokes.DisconnectedEventHandler HeadsetDisconnected;

        event Spokes.MuteChangedEventHandler MuteChanged;

        bool IsHeadsetConnected{get;}
        bool IsHeadsetMuted { get; }
        bool IsDeviceConnected { get; }

        string InternalName  { get; }
        string ManufacturerName { get; }
        string ProductName { get; }
        string SerialNumber { get; }
        ushort VersionNumber { get; }

        void ToggleMute();
    }
}
