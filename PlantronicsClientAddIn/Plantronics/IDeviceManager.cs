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

        event Spokes.CallEndedEventHandler CallEndedByDevice;
        
        event Spokes.CallAnsweredEventHandler CallAnsweredByDevice;

        event Spokes.OnCallEventHandler OnCall;

        /// <summary>
        /// Event is raised when the talk button is momentarily pressed.
        /// </summary>
        event EventHandler TalkButtonPressed;

        /// <summary>
        /// Event is raised when the talk button is held  down.
        /// </summary>
        event EventHandler TalkButtonHeld;

        bool IsHeadsetConnected{get;}
        bool IsHeadsetMuted { get; }
        bool IsDeviceConnected { get; }

        string InternalName  { get; }
        string ManufacturerName { get; }
        string ProductName { get; }
        string SerialNumber { get; }
        ushort VersionNumber { get; }

        string CurrentCallId { get; set; }

        void ToggleMute();

        void IncomingCall(string callId);
        void OutgoingCall(string callId);
        void CallAnswered(string callId);
        void CallEnded(string callId);
        void CallResumed(string callId);
        void CallHeld(string callId);

    }
}
