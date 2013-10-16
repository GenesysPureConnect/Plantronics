using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Interop.Plantronics;

/*******
 * 
 * SpokesWrapper.cs
 * 
 * SpokesWrapper.cs is a wrapper around the Plantronics Spokes COM Service API for C# .NET (.NET Framework 4 and higher).
 * 
 * It's purpose is to make it easier and simpler to integrate support for Plantronics devices into any applications.
 * 
 * It achieves this by hiding a lot of the more tricky aspects of integration behind the wrapper and presenting
 * a simple and consistent set of Event Handlers and functions for the core features of the SDK that the user
 * will typically be needing.
 * 
 * The latest version of this file will be maintained on Github, here:
 * 
 * https://github.com/lewiscollins/SpokesWrapper
 * 
 * Read more about Plantronics Spokes at the Plantronics Developer Connection web site:
 * 
 * http://developer.plantronics.com/community/devzone/
 * 
 * Lewis Collins
 * 
 * http://developer.plantronics.com/people/lcollins
 * 
 * VERSION HISTORY:
 * ********************************************************************************
 * Version 1.0.9:
 * Date: 22nd Feb 2013
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Add proximity enabled / proximity disabled event handlers
 *
 * Version 1.0.8:
 * Date: 21st Feb 2013
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Add headset button events via new ButtonPressed event handler in the wrapper.
 *       NOTE: you are advised NOT to use headset buttons e.g. talk button for
 *       call control, but rather use the IncomingCall/OutgoingCall functions
 *       and CallAnswered/CallEnded event handlers. Using talk button will
 *       cause problems with multiline devices as talk button events for the
 *       deskphone (+EHS) will also be received by your app through the SDK!!!!
 *       Also bad interactions can occur with talk button and other softphones
 *       on your system e.g. Lync if you try to use raw button events.
 *       You have been warned.
 *     - Add CallRequested event handler to obtain user call requested events from
 *       dialpad devices (Calisto P240/800 series)
 *
 * Version 1.0.7:
 * Date: 19th Feb 2013
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Change namespace to Plantronics.UC.SpokesWrapper (from Plantronics.UC.Spokes)
 *
 * Version 1.0.6:
 * Date: 14th Feb 2013
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Minor fix to incorrect worn state in TakenOff event handler
 *
 * Version 1.0.5:
 * Date: 8th Feb 2013
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Added flag for don/doff/dock/undock event to say if it is the initial status
 *       so an app can ignore the initial status if it wants to (i.e. not lock screen
 *       when it first runs and receives initial status event!)
 *
 * Version 1.0.4:
 * Date: 6th Feb 2013
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Added new method to ask if link is active
 *
 * Version 1.0.3:
 * Date: 1st Feb 2013
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Added new "line active changed" event handler so apps can know when 
 *       line to device is active or not
 *
 * Version 1.0.2:
 * Date: 4th December 2012
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Adding multiline device features e.g. for Savi 7xx
 *
 * Version 1.0.1:
 * Date: 30th November 2012
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Fixed order of events for DetachDevice flow
 *     - Fixed need to check for null serial number member
 *
 * Version 1.0:
 * Date: 30th November 2012
 * Compatible with Spokes SDK version(s): 2.7.14092.0
 * Changed by: Lewis Collins
 *   Changes:
 *     - Adds code to extract serial number (thanks Nemanja)
 *     - Adds comments to all publicly expose methods and event handlers  
 * ********************************************************************************
 * 
 **/

namespace Plantronics.UC.SpokesWrapper
{
    /// <summary>
    /// interface to allow your application's class to handle log debug tracing from the SpokesWrapper...
    /// </summary>
    public interface DebugLogger
    {
        void DebugPrint(string methodname, string str);
    }

    /// <summary>
    /// Struct to hold info on Plantronics device capabilities
    /// </summary>
    public struct SpokesDeviceCaps
    {
        public bool HasProximity;
        public bool HasCallerId;
        public bool HasDocking;
        public bool HasWearingSensor;
        public bool HasMultiline;
        public bool IsWireless;

        /// <summary>
        /// Constructor: pass in boolean values for whether it has the given device capabilities or not
        /// </summary>
        public SpokesDeviceCaps(bool HasProximity, bool HasCallerId, bool HasDocking, bool HasWearingSensor, bool HasMultiline, bool IsWireless)
        {
            this.HasProximity = HasProximity;
            this.HasCallerId = HasCallerId;
            this.HasDocking = HasDocking;
            this.HasWearingSensor = HasWearingSensor;
            this.HasMultiline = HasMultiline;
            this.IsWireless = IsWireless;
        }

        /// <summary>
        /// Returns a nice string representation of device capabilites, e.g. for use in logs
        /// </summary>
        public override string ToString()
        {
            return "Proximity = " + HasProximity + "\r\n" +
                "Mobile Caller Id = " + HasCallerId + "\r\n" +
                "Dockable = " + HasDocking + "\r\n" +
                "Wearing Sensor = " + HasWearingSensor + "\r\n" +
                "Multiline = " + HasMultiline + "\r\n" +
                "Is Wireless = " + IsWireless + "\r\n";
        }
    }

    /// <summary>
    /// Event args for Mute Changed event handler
    /// </summary>
    public class MuteChangedArgs : EventArgs
    {
        public bool m_muteon = false;

        public MuteChangedArgs(bool isMuteOn)
        {
            m_muteon = isMuteOn;
        }
    }

    /// <summary>
    /// Event args for Line Active Changed event handler
    /// </summary>
    public class LineActiveChangedArgs : EventArgs
    {
        public bool m_lineactive = false;

        public LineActiveChangedArgs(bool isLineActive)
        {
            m_lineactive = isLineActive;
        }
    }

    /// <summary>
    /// Event args for Attached (device attached) event handler
    /// </summary>
    public class AttachedArgs : EventArgs
    {
        public IDevice m_device = null;

        public AttachedArgs(IDevice aDevice)
        {
            m_device = aDevice;
        }
    }

    /// <summary>
    /// Event args for TakenOff/PutOn events (wearing state) event handlers
    /// </summary>
    public class WearingStateArgs : EventArgs
    {
        public bool m_worn = false;
        public bool m_isInitialStateEvent = false;

        public WearingStateArgs(bool worn, bool isInitialStateEvent)
        {
            m_worn = worn;
            m_isInitialStateEvent = isInitialStateEvent;
        }
    }

    /// <summary>
    /// Event args for Docked/UnDocked events (docking) event handlers
    /// </summary>
    public class DockedStateArgs : EventArgs
    {
        public bool m_docked = false;
        public bool m_isInitialStateEvent = false;

        public DockedStateArgs(bool docked, bool isInitialStateEvent)
        {
            m_docked = docked;
            m_isInitialStateEvent = isInitialStateEvent;
        }
    }

    /// <summary>
    /// Enumeration of call states
    /// </summary>
    public enum OnCallCallState
    {
        Ringing,
        OnCall,
        Idle
    }

    /// <summary>
    /// Event args for OnCall event handler
    /// </summary>
    public class OnCallArgs : EventArgs
    {
        public string CallSource;
        public bool Incoming;
        public OnCallCallState State;

        public OnCallArgs(string source, bool isIncoming, OnCallCallState state)
        {
            CallSource = source;
            Incoming = isIncoming;
            State = state;
        }
    }

    /// <summary>
    /// Enumeration of mobile call states
    /// </summary>
    public enum MobileCallState
    {
        Ringing,
        OnCall,
        Idle
    }

    /// <summary>
    /// Event args for OnMobileCall event handler
    /// </summary>
    public class OnMobileCallArgs : EventArgs
    {
        public bool Incoming;
        public MobileCallState State;

        public OnMobileCallArgs(bool isIncoming, MobileCallState state)
        {
            Incoming = isIncoming;
            State = state;
        }
    }

    /// <summary>
    /// Event args for MobileCallerId event handler
    /// </summary>
    public class MobileCallerIdArgs : EventArgs
    {
        public string MobileCallerId { get; set; }

        public MobileCallerIdArgs(string mobilecallerid)
        {
            MobileCallerId = mobilecallerid;
        }
    }

    /// <summary>
    /// Enumeration of serial numbers in a Plantronics device (i.e. Headset and base/usb adaptor)
    /// </summary>
    public enum SerialNumberTypes
    {
        Headset,
        Base
    }

    /// <summary>
    /// Event args for SerialNumber event handler
    /// </summary>
    public class SerialNumberArgs : EventArgs
    {
        public string SerialNumber { get; set; }
        public SerialNumberTypes SerialNumberType { get; set; }

        public SerialNumberArgs(string serialnumber, SerialNumberTypes serialnumtype)
        {
            SerialNumber = serialnumber;
            SerialNumberType = serialnumtype;
        }
    }

    /// <summary>
    /// Event args for CallAnswered event handler
    /// </summary>
    public class CallAnsweredArgs : EventArgs
    {
        public int CallId { get; set; }
        public string CallSource { get; set; }

        public CallAnsweredArgs(int callid, string callsource)
        {
            CallId = callid;
            CallSource = callsource;
        }
    }

    /// <summary>
    /// Event args for CallEnded event handler
    /// </summary>
    public class CallEndedArgs : EventArgs
    {
        public int CallId { get; set; }
        public string CallSource { get; set; }

        public CallEndedArgs(int callid, string callsource)
        {
            CallId = callid;
            CallSource = callsource;
        }
    }

    /// <summary>
    /// Used with MultiLineStateArgs to hold active/held status of multiple lines (PC, Mobile, Deskphone)
    /// </summary>
    public struct MultiLineStateFlags
    {
        public bool PCActive { get; set; }
        public bool MobileActive { get; set; }
        public bool DeskphoneActive { get; set; }
        public bool PCHeld { get; set; }
        public bool MobileHeld { get; set; }
        public bool DeskphoneHeld { get; set; }
    }

    /// <summary>
    /// EventArgs used with MultiLineStateChanged event handler to receive status of multiple lines (PC, Mobile, Deskphone) 
    /// when the state of any of these lines changes.
    /// </summary>
    public class MultiLineStateArgs : EventArgs
    {
        public MultiLineStateFlags MultiLineState { get; set; }

        public MultiLineStateArgs(MultiLineStateFlags multilinestate)
        {
            MultiLineState = multilinestate;
        }
    }

    /// <summary>
    /// EventArgs used with ButtonPress event handler to receive details of which button
    /// was pressed
    /// </summary>
    public class ButtonPressArgs : EventArgs
    {
        public HeadsetButton headsetButton;
        public AudioType audioType;
        public bool mute;

        public ButtonPressArgs(HeadsetButton headsetButton, AudioType audioType, bool aMute)
        {
            this.headsetButton = headsetButton;
            this.audioType = audioType;
            this.mute = aMute;
        }
    }

    /// <summary>
    /// EventArgs used with CallRequested event handler to receive details of the
    /// number requested to dial from dialpad device (Calisto P240/800 series)
    /// </summary>
    public class CallRequestedArgs : EventArgs
    {
        public Contact m_contact { get; set; }

        public CallRequestedArgs(Contact aContact)
        {
            m_contact = aContact;
        }
    }

    /// <summary>
    /// Enumeration of multiline device line types
    /// </summary>
    public enum Multiline_LineType
    {
        PC,
        Mobile,
        Deskphone
    }

    /// <summary>
    /// Defines a Spokes object which you can use to communicate with Plantronics devices.
    /// Cannot instantiate directly. To obtain singleton call Spokes.Instance.
    /// Note: using singleton model to avoid possibility of multiple instantiation
    /// as specified in: http://msdn.microsoft.com/en-us/library/ff650316.aspx
    /// </summary>
    public sealed class Spokes
    {
        private static volatile Spokes instance;
        private static object syncRoot = new Object();

        /// <summary>
        /// Default constructor, cannot be called directly. To obtain singleton call Spokes.Instance.
        /// </summary>
        private Spokes()
        {
            m_debuglog = null;
        }

        /// <summary>
        /// Default desctructor, disconnects from Spokes
        /// </summary>
        ~Spokes()
        {
            if (isConnected)
                Disconnect();
        }

        /// <summary>
        /// Returns the single Instance of Spokes which you can use to communicate with Plantronics devices
        /// </summary>
        public static Spokes Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        // Instantiate a singleton Spokes object
                        if (instance == null)
                            instance = new Spokes();
                    }
                }

                return instance;
            }
        }

        #region Spokes interfaces definitions
        static ISessionCOMManager m_sessionComManager = null;
        static IComSession m_comSession = null;
        static IDevice m_activeDevice = null;
        static ISessionCOMManagerEvents_Event m_sessionManagerEvents;
        static ICOMCallEvents_Event m_sessionEvents;
        static IDeviceCOMEvents_Event m_deviceComEvents;
        static IDeviceListenerCOMEvents_Event m_deviceListenerEvents;
        static IATDCommand m_atdCommand;
        static IHostCommand m_hostCommand;
        static IHostCommandExt m_hostCommandExt;
        public static string m_devicename = "";
        #endregion

        DebugLogger m_debuglog = null;

        /// <summary>
        /// A property containing flags that indicate the capabilities of the attached Plantronics device (if any).
        /// </summary>
        public SpokesDeviceCaps DeviceCapabilities;

        /// <summary>
        /// Returns boolean to indicate whether there is currently a Plantronics device attached to the PC or not.
        /// </summary>
        public bool HasDevice
        {
            get
            {
                return (m_activeDevice != null);
            }
        }

        bool m_mobIncoming = false; // mobile call direction flag
        bool m_voipIncoming = false; // mobile call direction flag

        #region Base Device State member fields
        MultiLineStateFlags m_activeHeldFlags;
        #endregion

        // C# event handlers that can be used to register for Spokes events...

        // Wearing sensor:
        public delegate void TakenOffEventHandler(object sender, WearingStateArgs e);
        public delegate void PutOnEventHandler(object sender, WearingStateArgs e);

        // Proximity:
        public delegate void NearEventHandler(object sender, EventArgs e);
        public delegate void FarEventHandler(object sender, EventArgs e);
        public delegate void ProximityEnabledEventHandler(object sender, EventArgs e);
        public delegate void ProximityDisabledEventHandler(object sender, EventArgs e);
        public delegate void ProximityUnknownEventHandler(object sender, EventArgs e);
        // In range/out of range:
        public delegate void InRangeEventHandler(object sender, EventArgs e);
        public delegate void OutOfRangeEventHandler(object sender, EventArgs e);
        // Docked/undocked:
        public delegate void DockedEventHandler(object sender, DockedStateArgs e);
        public delegate void UnDockedEventHandler(object sender, DockedStateArgs e);

        // Mobile caller id:
        public delegate void MobileCallerIdEventHandler(object sender, MobileCallerIdArgs e);
        public delegate void OnMobileCallEventHandler(object sender, OnMobileCallArgs e);
        public delegate void NotOnMobileCallEventHandler(object sender, EventArgs e);

        // Serial number (receives as result of earlier request for serial number):
        public delegate void SerialNumberEventHandler(object sender, SerialNumberArgs e);

        // Call control (headset button call control notification from Spokes):
        public delegate void CallAnsweredEventHandler(object sender, CallAnsweredArgs e);
        public delegate void CallEndedEventHandler(object sender, CallEndedArgs e);
        public delegate void CallSwitchedEventHandler(object sender, EventArgs e);
        // Call state notification (is user on a call or not?):
        public delegate void OnCallEventHandler(object sender, OnCallArgs e);
        public delegate void NotOnCallEventHandler(object sender, EventArgs e);
        // Mute sync:
        public delegate void MuteChangedEventHandler(object sender, MuteChangedArgs e);
        // Line active awareness:
        public delegate void LineActiveChangedEventHandler(object sender, LineActiveChangedArgs e);

        // Device attach/detach:
        public delegate void AttachedEventHandler(object sender, AttachedArgs e);
        public delegate void DetachedEventHandler(object sender, EventArgs e);
        // Device capabilities changed (depends on type of device attached):
        public delegate void CapabilitiesChangedEventHandler(object sender, EventArgs e);

        // Multiline device line state changed (for multi-line device, e.g. Savi 7xx):
        public delegate void MultiLineStateChangedEventHandler(object sender, MultiLineStateArgs e);

        // Button press event:
        public delegate void ButtonPressEventHandler(object sender, ButtonPressArgs e);

        // Button press event:
        public delegate void CallRequestedEventHandler(object sender, CallRequestedArgs e);

        // Definition of event handlers that clients can use to be notified whenever the
        // spokes event occurs:

        // Wearing sensor: ************************************************************
        /// <summary>
        /// Triggered when the user takes off the headset (with products that support wearing sensor)
        /// </summary>
        public event TakenOffEventHandler TakenOff;

        /// <summary>
        /// Triggered when the user puts on the headset (with products that support wearing sensor)
        /// </summary>
        public event PutOnEventHandler PutOn;

        // Proximity: ************************************************************
        /// <summary>
        /// Triggered when a Plantronics device comes near to PC dongle
        /// </summary>
        public event NearEventHandler Near;

        /// <summary>
        /// Triggered when a Plantronics device goes far from PC dongle
        /// </summary>
        public event FarEventHandler Far;

        /// <summary>
        /// Triggered when a Plantronics device proximity has been enabled
        /// </summary>
        public event ProximityEnabledEventHandler ProximityEnabled;

        /// <summary>
        /// Triggered when a Plantronics device proximity has been disabled
        /// </summary>
        public event ProximityDisabledEventHandler ProximityDisabled;

        /// <summary>
        /// Triggered when a Plantronics device proximity is unknown
        /// </summary>
        public event ProximityEnabledEventHandler ProximityUnknown;

        /// <summary>
        /// Triggered when a Plantronics device comes into range of PC dongle
        /// </summary>
        public event InRangeEventHandler InRange;

        /// <summary>
        /// Triggered when a Plantronics device goes out of range of PC dongle
        /// </summary>
        public event OutOfRangeEventHandler OutOfRange;

        /// <summary>
        /// Triggered when a Plantronics device is docked in its base or cradle
        /// </summary>
        public event DockedEventHandler Docked;

        /// <summary>
        /// Triggered when a Plantronics device is undocked from its base or cradle
        /// </summary>
        public event DockedEventHandler UnDocked;

        // Mobile caller id: ************************************************************
        /// <summary>
        /// Triggered when a caller id has been received
        /// </summary>
        public event MobileCallerIdEventHandler MobileCallerId;

        // Call state notification (is user on a call or not?): ************************************************************
        /// <summary>
        /// Triggered when some mobile calling activity is detected with the device
        /// </summary>
        public event OnMobileCallEventHandler OnMobileCall;

        /// <summary>
        /// Triggered when mobile calling activity comes to an end with the device
        /// </summary>
        public event NotOnMobileCallEventHandler NotOnMobileCall;

        // Serial number (receives as result of earlier request for serial number): ************************************************************
        /// <summary>
        /// Triggered when a serial number has been received from device
        /// </summary>
        public event SerialNumberEventHandler SerialNumber;

        // Call control (headset button call control notification from Spokes): ************************************************************
        /// <summary>
        /// Triggered when the user answers a call using the headset device
        /// </summary>
        public event CallAnsweredEventHandler CallAnswered;

        /// <summary>
        /// Triggered when the user answers a call using the headset device
        /// </summary>
        public event CallEndedEventHandler CallEnded;

        /// <summary>
        /// Triggered when the user tries to switch call using the headset device by pressing switch (flash) button
        /// </summary>
        public event CallSwitchedEventHandler CallSwitched;

        // Call state notification (is user on a call or not?): ************************************************************
        /// <summary>
        /// Triggered when some calling activity is detected with the device
        /// </summary>
        public event OnCallEventHandler OnCall;

        /// <summary>
        /// Triggered when calling activity comes to an end with the device
        /// </summary>
        public event NotOnCallEventHandler NotOnCall;

        // Mute sync: ************************************************************
        /// <summary>
        /// Triggered when the user mutes or unmutes the headset device
        /// </summary>
        public event MuteChangedEventHandler MuteChanged;

        // Mute sync: ************************************************************
        /// <summary>
        /// Triggered when the spokes activates or deactivates the line to the headset device
        /// </summary>
        public event LineActiveChangedEventHandler LineActiveChanged;

        // Device attach/detach: ************************************************************
        /// <summary>
        /// Triggered when the user attached a Plantronics device
        /// </summary>
        public event AttachedEventHandler Attached;

        /// <summary>
        /// Triggered when the user detaches a Plantronics device
        /// </summary>
        public event DetachedEventHandler Detached;

        // Device capabilities changed (depends on type of device attached): ************************************************************
        /// <summary>
        /// Triggered when the capabilities available on the device changes, e.g. asyncronous proximity registration is completed or mobile caller id registration is completed
        /// </summary>
        public event CapabilitiesChangedEventHandler CapabilitiesChanged;

        // Multiline device line state changed (for multi-line device, e.g. Savi 7xx): ************************************************************
        /// <summary>
        /// Triggered when there is a change of the active or held states of any of the lines of multi-line device (e.g. Savi 7xx)
        /// </summary>
        public event MultiLineStateChangedEventHandler MultiLineStateChanged;

        // Triggered when a button press event is generated by device: ************************************************************
        /// <summary>
        /// Triggered when a button press event is generated by device
        /// NOTE: you are advised NOT to use headset buttons e.g. talk button for
        /// call control, but rather use the IncomingCall/OutgoingCall functions
        /// and CallAnswered/CallEnded event handlers. (more notes in SpokesWrapper.cs)
        /// </summary>
        public event ButtonPressEventHandler ButtonPress;

        // Triggered when a user call requested event is received from dialpad device: ************************************************************
        /// <summary>
        /// Triggered when a user call requested event is received from dialpad device
        /// </summary>
        public event CallRequestedEventHandler CallRequested;

        // Now for the implementation of the event handlers:

        // Wearing sensor: ************************************************************
        // Invoke the Doffed event; called whenever user doffs (takes off) their headset
        private void OnTakenOff(WearingStateArgs e)
        {
            if (TakenOff != null)
                TakenOff(this, e);
        }

        private void OnPutOn(WearingStateArgs e)
        {
            if (PutOn != null)
                PutOn(this, e);
        }

        // Proximity: ************************************************************
        private void OnNear(EventArgs e)
        {
            if (Near != null)
                Near(this, e);
        }

        private void OnFar(EventArgs e)
        {
            if (Far != null)
                Far(this, e);
        }

        private void OnProximityEnabled(EventArgs e)
        {
            if (ProximityEnabled != null)
                ProximityEnabled(this, e);
        }

        private void OnProximityDisabled(EventArgs e)
        {
            if (ProximityDisabled != null)
                ProximityDisabled(this, e);
        }

        private void OnProximityUnknown(EventArgs e)
        {
            if (ProximityUnknown != null)
                ProximityUnknown(this, e);
        }

        private void OnInRange(EventArgs e)
        {
            if (InRange != null)
                InRange(this, e);
        }

        private void OnOutOfRange(EventArgs e)
        {
            if (OutOfRange != null)
                OutOfRange(this, e);
        }

        private void OnDocked(DockedStateArgs e)
        {
            if (Docked != null)
                Docked(this, e);
        }

        private void OnUnDocked(DockedStateArgs e)
        {
            if (UnDocked != null)
                UnDocked(this, e);
        }

        // Mobile caller id: ************************************************************
        private void OnMobileCallerId(MobileCallerIdArgs e)
        {
            if (MobileCallerId != null)
                MobileCallerId(this, e);
        }

        private void OnOnMobileCall(OnMobileCallArgs e)
        {
            if (OnMobileCall != null)
                OnMobileCall(this, e);
        }

        private void OnNotOnMobileCall(EventArgs e)
        {
            if (NotOnMobileCall != null)
                NotOnMobileCall(this, e);
        }

        // Serial number (receives as result of earlier request for serial number): ************************************************************
        private void OnSerialNumber(SerialNumberArgs e)
        {
            if (SerialNumber != null)
                SerialNumber(this, e);
        }

        // Call control (headset button call control notification from Spokes): ************************************************************
        private void OnCallAnswered(CallAnsweredArgs e)
        {
            if (CallAnswered != null)
                CallAnswered(this, e);
        }

        private void OnCallEnded(CallEndedArgs e)
        {
            if (CallEnded != null)
                CallEnded(this, e);
        }

        private void OnCallSwitched(EventArgs e)
        {
            if (CallSwitched != null)
                CallSwitched(this, e);
        }

        // Call state notification (is user on a call or not?): ************************************************************
        private void OnOnCall(OnCallArgs e)
        {
            if (OnCall != null)
                OnCall(this, e);
        }

        private void OnNotOnCall(EventArgs e)
        {
            if (NotOnCall != null)
                NotOnCall(this, e);
        }

        // Mute sync: ************************************************************
        private void OnMuteChanged(MuteChangedArgs e)
        {
            if (MuteChanged != null)
                MuteChanged(this, e);
        }

        // Line active: ************************************************************
        private void OnLineActiveChanged(LineActiveChangedArgs e)
        {
            if (LineActiveChanged != null)
                LineActiveChanged(this, e);
        }

        // Device attach/detach: ************************************************************
        private void OnAttached(AttachedArgs e)
        {
            if (Attached != null)
                Attached(this, e);
        }

        private void OnDetached(EventArgs e)
        {
            if (Detached != null)
                Detached(this, e);
        }

        // Device capabilities changed (depends on type of device attached): ************************************************************
        private void OnCapabilitiesChanged(EventArgs e)
        {
            if (CapabilitiesChanged != null)
                CapabilitiesChanged(this, e);
        }

        // Multiline device line state changed (for multi-line device, e.g. Savi 7xx): ************************************************************
        private void OnMultiLineStateChanged(MultiLineStateArgs e)
        {
            if (MultiLineStateChanged != null)
                MultiLineStateChanged(this, e);
        }

        // Triggered when a button press event is generated by device: ************************************************************
        private void OnButtonPress(ButtonPressArgs e)
        {
            if (ButtonPress != null)
                ButtonPress(this, e);
        }

        // Triggered when a user call requested event is received from dialpad device: ************************************************************
        private void OnCallRequested(CallRequestedArgs e)
        {
            if (CallRequested != null)
                CallRequested(this, e);
        }

        bool isConnected = false;

        /// <summary>
        /// If your application class implements the Spokes.DebugLogger interface you can pass a reference to your application class
        /// to the SetLogger method. This allows your class to be responsible for debug logging of Spokes related debug trace information.
        /// </summary>
        /// <param name="aLogger">For this parameter pass the "this" reference of your class that implements Spokes.DebugLogger interface.</param>
        public void SetLogger(DebugLogger aLogger)
        {
            m_debuglog = aLogger;
        }

        /// <summary>
        /// Instruct Spokes object to connect to Spokes runtime engine and register itself
        /// so that it can begin to communicate with the attached Plantronics device.
        /// </summary>
        /// <param name="SessionName">Optional name of your appplication's session within Spokes runtime engine. If omitted it will default to "COM Session".</param>
        public bool Connect(string SessionName = "COM Session")
        {
            if (isConnected) return true;
            DeviceCapabilities =
                new SpokesDeviceCaps(false, false, false, false, false, false); // we don't yet know what the capabilities are
            OnCapabilitiesChanged(EventArgs.Empty);
            bool success = false;
            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////
                // create session manager, and attach to session manager events
                m_sessionComManager = new Interop.Plantronics.SessionComManagerClass();
                
                m_sessionManagerEvents = m_sessionComManager as ISessionCOMManagerEvents_Event;
                if (m_sessionManagerEvents != null)
                {
                    m_sessionManagerEvents.CallStateChanged += m_sessionComManager_CallStateChanged;
                    m_sessionManagerEvents.DeviceStateChanged += m_sessionComManager_DeviceStateChanged;
                }
                else
                    success = false;

                ////////////////////////////////////////////////////////////////////////////////////////
                // register session to spokes
                m_comSession = m_sessionComManager.Register(SessionName);
                if (m_comSession != null)
                {
                    // attach to session call events
                    m_sessionEvents = m_comSession.CallEvents as ICOMCallEvents_Event;
                    if (m_sessionEvents != null)
                    {
                        m_sessionEvents.CallRequested += m_sessionEvents_CallRequested;
                        m_sessionEvents.CallStateChanged += m_sessionEvents_CallStateChanged;

                    }
                    else
                        success = false;

                    ////////////////////////////////////////////////////////////////////////////////////////
                    // Attach to active device and print all device information
                    // and registers for proximity (if supported by device)
                    AttachDevice();
                    success = true;
                }
            }
            catch (System.Exception)
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Instruct Spokes object to disconnect from Spokes runtime engine and unregister its
        /// session in Spokes.
        /// </summary>
        internal void Disconnect()
        {
            DetachDevice();

            if (m_comSession != null)
            {
                if (m_sessionEvents != null)
                {
                    // release session events
                    m_sessionEvents.CallRequested -= m_sessionEvents_CallRequested;
                    m_sessionEvents.CallStateChanged -= m_sessionEvents_CallStateChanged;
                    Marshal.ReleaseComObject(m_sessionEvents);
                    m_sessionEvents = null;
                }
                // unregister session
                if (m_sessionEvents != null)
                {
                    m_sessionManagerEvents.DeviceStateChanged -= m_sessionComManager_DeviceStateChanged;
                }
                m_sessionComManager.UnRegister(m_comSession);
                Marshal.ReleaseComObject(m_comSession);
                m_comSession = null;
            }
            if (m_sessionComManager != null)
            {
                Marshal.ReleaseComObject(m_sessionComManager);
                m_sessionComManager = null;
            }
        }

        #region Print Session and Device events to console
        // print session manager events
        void m_sessionComManager_DeviceStateChanged(object sender, _DeviceStateEventArgs e)
        {

            // if our "Active device" was unplugged, detach from it and attach to new one
            if (e.State == DeviceState.DeviceState_Removed && m_activeDevice != null && string.Compare(e.DevicePath, m_activeDevice.DevicePath, true) == 0)
            {
                DetachDevice();
                AttachDevice();

            }
            else if (e.State == DeviceState.DeviceState_Added && m_activeDevice == null)
            {
                // if device is plugged, and we don't have "Active device", just attach to it
                AttachDevice();
            }
        }

        // print session manager events
        void m_sessionComManager_CallStateChanged(object sender, _CallStateEventArgs e)
        {
            DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: call state event = " + e.ToString());

            switch (e.Action)
            {
                case CallState.CallState_CallRinging:
                    m_voipIncoming = true;
                    // Getting here indicates user is ON A CALL!
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Calling activity detected!" + e.ToString());
                    OnOnCall(new OnCallArgs(e.CallSource, m_voipIncoming, OnCallCallState.Ringing));
                    break;
                case CallState.CallState_MobileCallRinging:
                    m_mobIncoming = true;
                    // user incoming mobile call
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Mobile Calling activity detected!" + e.ToString());
                    OnOnMobileCall(new OnMobileCallArgs(m_mobIncoming, MobileCallState.Ringing));
                    break;
                case CallState.CallState_MobileCallInProgress:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Mobile Calling activity detected!" + e.ToString());
                    OnOnMobileCall(new OnMobileCallArgs(m_mobIncoming, MobileCallState.OnCall));
                    break;
                case CallState.CallState_AcceptCall:
                case CallState.CallState_CallInProgress:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Call was ansswered/in progress!" + e.ToString());
                    OnOnCall(new OnCallArgs(e.CallSource, m_voipIncoming, OnCallCallState.OnCall));
                    OnCallAnswered(new CallAnsweredArgs(e.CallId.Id, e.CallSource));
                    break;
                case CallState.CallState_HoldCall:
                case CallState.CallState_Resumecall:
                case CallState.CallState_TransferToHeadSet:
                case CallState.CallState_TransferToSpeaker:
                    // Getting here indicates user is ON A CALL!
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Calling activity detected!" + e.ToString());
                    OnOnCall(new OnCallArgs(e.CallSource, m_voipIncoming, OnCallCallState.OnCall));
                    break;
                case CallState.CallState_MobileCallEnded:
                    m_mobIncoming = false;
                    // Getting here indicates user HAS FINISHED A CALL!
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Mobile Calling activity ended." + e.ToString());
                    OnNotOnMobileCall(EventArgs.Empty);
                    break;
                case CallState.CallState_CallEnded:
                case CallState.CallState_CallIdle:
                case CallState.CallState_RejectCall:
                case CallState.CallState_TerminateCall:
                    m_voipIncoming = false;
                    // Getting here indicates user HAS FINISHED A CALL!
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Calling activity ended." + e.ToString());
                    OnNotOnCall(EventArgs.Empty);
                    OnCallEnded(new CallEndedArgs(e.CallId.Id, e.CallSource));
                    break;
                default:
                    // ignore other call state events
                    break;
            }
        }

        // used internally to get mobile caller id when we are notified of mobile caller id
        // event from Spokes
        private string GetMobileCallerID()
        {
            string retval = "";
            if (m_atdCommand != null)
            {
                try
                {
                    retval = m_atdCommand.CallerID;
                }
                catch (System.Exception e)
                {
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: INFO: Exception occured getting mobile caller id\r\nException = " + e.ToString());
                }
            }
            return retval;
        }

        // print session events
        void m_sessionEvents_CallStateChanged(object sender, _CallStateEventArgs e)
        {
            string id = e.CallId != null ? e.CallId.Id.ToString() : "none";

        }
        // print session events
        void m_sessionEvents_CallRequested(object sender, _CallRequestEventArgs e)
        {
            string contact = e.Contact != null ? e.Contact.Name : "none";
            DebugPrint(MethodInfo.GetCurrentMethod().Name, string.Format("Session CallRequested event: Contact:({0})", contact));
            OnCallRequested(new CallRequestedArgs(e.Contact));
        }
        // print device listner events
        void m_deviceListenerEvents_Handler(object sender, _DeviceListenerEventArgs e)
        {
            switch (e.DeviceEventType)
            {
                case DeviceEventType.DeviceEventType_ATDButtonPressed:
                    break;
                case DeviceEventType.DeviceEventType_ATDStateChanged:
                    DeviceListener_ATDStateChanged(sender, e);
                    break;
                case DeviceEventType.DeviceEventType_BaseButtonPressed:
                case DeviceEventType.DeviceEventType_BaseStateChanged:
                    DeviceListener_BaseStateChanged(sender, e);
                    break;
                case DeviceEventType.DeviceEventType_HeadsetButtonPressed:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "DeviceEventType_HeadsetButtonPressed " + e.HeadsetButtonPressed.ToString());
                    break;
                case DeviceEventType.DeviceEventType_HeadsetStateChanged:
                default:
                    break;
            }
        }

        // Respond to various base state changes by updating our knowledge of multiline active/held states...
        void DeviceListener_BaseStateChanged(object sender, _DeviceListenerEventArgs e)
        {
            // write your own code to react to the state change

            switch (e.BaseStateChange)
            {
                case BaseStateChange.BaseStateChange_Unknown:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: Unknown"));
                    break;
                case BaseStateChange.BaseStateChange_PstnLinkEstablished:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: PstnLinkEstablished"));
                    GetActiveStates();
                    OnMultiLineStateChanged(new MultiLineStateArgs(m_activeHeldFlags));
                    break;
                case BaseStateChange.BaseStateChange_PstnLinkDown:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: PstnLinkDown"));
                    GetActiveStates();
                    OnMultiLineStateChanged(new MultiLineStateArgs(m_activeHeldFlags));
                    break;
                case BaseStateChange.BaseStateChange_VoipLinkEstablished:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: VoipLinkEstablished"));
                    GetActiveStates();
                    OnMultiLineStateChanged(new MultiLineStateArgs(m_activeHeldFlags));
                    break;
                case BaseStateChange.BaseStateChange_VoipLinkDown:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: VoipLinkDown"));
                    GetActiveStates();
                    OnMultiLineStateChanged(new MultiLineStateArgs(m_activeHeldFlags));
                    break;
                case BaseStateChange.BaseStateChange_AudioMixer:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: AudioMixer"));
                    break;
                case BaseStateChange.BaseStateChange_RFLinkWideBand:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: RFLinkWideBand"));
                    break;
                case BaseStateChange.BaseStateChange_RFLinkNarrowBand:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: RFLinkNarrowBand"));
                    break;
                case BaseStateChange.BaseStateChange_MobileLinkEstablished:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: MobileLinkEstablished"));
                    GetActiveStates();
                    OnMultiLineStateChanged(new MultiLineStateArgs(m_activeHeldFlags));
                    break;
                case BaseStateChange.BaseStateChange_MobileLinkDown:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: MobileLinkDown"));
                    GetActiveStates();
                    OnMultiLineStateChanged(new MultiLineStateArgs(m_activeHeldFlags));
                    break;
                case BaseStateChange.BaseStateChange_InterfaceStateChanged:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: InterfaceStateChanged"));
                    GetHoldStates();
                    OnMultiLineStateChanged(new MultiLineStateArgs(m_activeHeldFlags));
                    break;
                case BaseStateChange.BaseStateChange_AudioLocationChanged:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: AudioLocationChanged"));
                    break;
                case BaseStateChange.BaseStateChange_SerialNumber:
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("BaseStateChanged: SerialNumber"));
                    break;
            }
        }

        void m_deviceListenerEvents_HandlerMethods(object sender, _DeviceListenerEventArgs e)
        {
            DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Received Spokes Event: " + e.ToString());

            switch (e.DeviceEventType)
            {
                case DeviceEventType.DeviceEventType_HeadsetStateChanged:
                    switch (e.HeadsetStateChange)
                    {
                        case HeadsetStateChange.HeadsetStateChange_Don:
                            OnPutOn(new WearingStateArgs(true, false));
                            break;
                        case HeadsetStateChange.HeadsetStateChange_Doff:
                            OnTakenOff(new WearingStateArgs(false, false));
                            break;
                        case HeadsetStateChange.HeadsetStateChange_Near:
                            OnNear(EventArgs.Empty);
                            break;
                        case HeadsetStateChange.HeadsetStateChange_Far:
                            OnFar(EventArgs.Empty);
                            break;
                        case HeadsetStateChange.HeadsetStateChange_ProximityDisabled:
                            // Note: intepret this event as that the mobile phone has gone out of Bluetooth
                            // range and is no longer paired to the headset.
                            // Lock the PC, but immediately re-enable proximity
                            OnProximityDisabled(EventArgs.Empty);
                            // Immediately re-enable proximity
                            RegisterForProximity(true);
                            break;
                        case HeadsetStateChange.HeadsetStateChange_ProximityEnabled:
                            OnProximityEnabled(EventArgs.Empty);
                            break;
                        case HeadsetStateChange.HeadsetStateChange_ProximityUnknown:
                            OnProximityUnknown(EventArgs.Empty);
                            break;
                        case HeadsetStateChange.HeadsetStateChange_InRange:
                            OnInRange(EventArgs.Empty);
                            // Immediately re-enable proximity
                            RegisterForProximity(true);
                            // Request headset serial number (maybe user paired with another?)
                            RequestSingleSerialNumber(SerialNumberTypes.Headset);
                            break;
                        case HeadsetStateChange.HeadsetStateChange_OutofRange:
                            OnOutOfRange(EventArgs.Empty);
                            OnSerialNumber(new SerialNumberArgs("", SerialNumberTypes.Headset));
                            break;
                        case HeadsetStateChange.HeadsetStateChange_Docked:
                            OnDocked(new DockedStateArgs(true, false));
                            break;
                        case HeadsetStateChange.HeadsetStateChange_UnDocked:
                            OnUnDocked(new DockedStateArgs(false, false));
                            break;
                        case HeadsetStateChange.HeadsetStateChange_MuteON:
                            OnMuteChanged(new MuteChangedArgs(true));
                            break;
                        case HeadsetStateChange.HeadsetStateChange_MuteOFF:
                            OnMuteChanged(new MuteChangedArgs(false));
                            break;
                        case HeadsetStateChange.HeadsetStateChange_MonoON:
                            OnLineActiveChanged(new LineActiveChangedArgs(true));
                            break;
                        case HeadsetStateChange.HeadsetStateChange_MonoOFF:
                            OnLineActiveChanged(new LineActiveChangedArgs(false));
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

        }
        //        #region DeviceListener events
        void DeviceListener_ATDStateChanged(object sender, _DeviceListenerEventArgs e)
        {
            //HANDLE ATD State changes
            switch (e.ATDStateChange)
            {
                case ATDStateChange.ATDStateChange_MobileInComing:
                    m_mobIncoming = true; // set on call flag
                    OnOnMobileCall(new OnMobileCallArgs(m_mobIncoming, MobileCallState.Ringing));
                    break;
                case ATDStateChange.ATDStateChange_MobileOnCall:
                    OnOnMobileCall(new OnMobileCallArgs(m_mobIncoming, MobileCallState.OnCall));
                    break;
                case ATDStateChange.ATDStateChange_MobileCallEnded:
                    m_mobIncoming = false; // clear mobile call direction flag
                    OnNotOnMobileCall(EventArgs.Empty);
                    break;
                case ATDStateChange.ATDStateChange_MobileCallerID:
                    OnMobileCallerId(new MobileCallerIdArgs(GetMobileCallerID()));
                    break;
                case ATDStateChange.ATDStateChange_MobileOutGoing:
                    break;
                case ATDStateChange.ATDStateChange_PstnInComingCallRingOn:
                    break;
                case ATDStateChange.ATDStateChange_PstnInComingCallRingOff:
                    break;
            }
        }

        // print device events
        void m_deviceComEvents_Handler(object sender, _DeviceEventArgs e)
        {
            DebugPrint(MethodInfo.GetCurrentMethod().Name, string.Format("Device Event: Audio:{0} Buton:{1} Mute:{2} Usage:{3}", e.AudioState, e.ButtonPressed, e.Mute, e.Usage.ToString()));

            if (e.ButtonPressed == HeadsetButton.HeadsetButton_Flash)
            {
                OnCallSwitched(EventArgs.Empty);
            }

            OnButtonPress(new ButtonPressArgs(e.ButtonPressed, e.AudioState, e.Mute));
        }
        #endregion

        // attach to device events
        private void AttachDevice()
        {
            m_activeDevice = m_comSession.ActiveDevice;
            if (m_activeDevice != null)
            {
                // LC assume minimum first set of device capabilities...
                DeviceCapabilities =
                    new SpokesDeviceCaps(false, false, true, true, false, true);
                OnCapabilitiesChanged(EventArgs.Empty);

                OnSerialNumber(new SerialNumberArgs("", SerialNumberTypes.Base));
                OnSerialNumber(new SerialNumberArgs("", SerialNumberTypes.Headset));

                // LC have seen case where ProductName was empty but InternalName was not...
                if (m_activeDevice.ProductName.Length > 0)
                {
                    m_devicename = m_activeDevice.ProductName;
                }
                else if (m_activeDevice.InternalName.Length > 0)
                {
                    m_devicename = m_activeDevice.InternalName;
                }
                else
                {
                    m_devicename = "Could not determine device name";
                }

                m_deviceComEvents = m_activeDevice.DeviceEvents as IDeviceCOMEvents_Event;
                if (m_deviceComEvents != null)
                {
                    // Attach to device events
                    m_deviceComEvents.ButtonPressed += m_deviceComEvents_Handler;
                    m_deviceComEvents.AudioStateChanged += m_deviceComEvents_Handler;
                    m_deviceComEvents.FlashPressed += m_deviceComEvents_Handler;
                    m_deviceComEvents.MuteStateChanged += m_deviceComEvents_Handler;
                    m_deviceComEvents.SmartPressed += m_deviceComEvents_Handler;
                    m_deviceComEvents.TalkPressed += m_deviceComEvents_Handler;
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: AttachedEventHandler to device events");
                }
                else
                {
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error: unable to attach to device events");
                    return;
                }

                m_deviceListenerEvents = m_activeDevice.DeviceListener as IDeviceListenerCOMEvents_Event;
                if (m_deviceListenerEvents != null)
                {
                    // Attach to device listener events
                    m_deviceListenerEvents.ATDStateChanged += m_deviceListenerEvents_Handler;
                    m_deviceListenerEvents.BaseButtonPressed += m_deviceListenerEvents_Handler;
                    m_deviceListenerEvents.BaseStateChanged += m_deviceListenerEvents_Handler;
                    m_deviceListenerEvents.HeadsetButtonPressed += m_deviceListenerEvents_Handler;
                    m_deviceListenerEvents.HeadsetStateChanged += m_deviceListenerEvents_HandlerMethods;
                }
                else
                {
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error: unable to attach to device listener events");
                    return;
                }

                // LC, Nemanja change, wire up serial number friendly events
                IDeviceCOMEventsExt_Event eex = m_deviceComEvents as IDeviceCOMEventsExt_Event;
                eex.HeadsetStateChanged += eex_HeadsetStateChanged;
                IBaseCOMEvents_Event be = m_deviceComEvents as IBaseCOMEvents_Event;
                be.BaseEventReceived += be_BaseEventReceived;

                m_hostCommand = m_activeDevice.HostCommand;
                if (m_hostCommand == null) DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error: unable to obtain host command interface");
                m_atdCommand = m_activeDevice.HostCommand as IATDCommand;
                if (m_atdCommand == null) DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error: unable to obtain atd command interface");
                m_hostCommandExt = m_activeDevice.HostCommand as IHostCommandExt;
                if (m_hostCommandExt == null) DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error: unable to obtain host command ext interface");

                UpdateOtherDeviceCapabilities();

                // trigger user's event handler
                OnAttached(new AttachedArgs(m_activeDevice));

                // now poll for current state (proximity, mobile call status, donned status, mute status)
                GetInitialDeviceState();

                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: AttachedEventHandler to device");
            }
        }

        // NEW, Nemanja's code to get the serial ID's !
        private void be_BaseEventReceived(object sender, _BaseEventArgs e)
        {
            if (e.SerialNumber != null && e.SerialNumber[0] != 0)
            {
                string serialStr = byteArrayToString(e.SerialNumber);
                //Console.WriteLine(string.Format("Base serial number: {0}", serialStr));
                OnSerialNumber(new SerialNumberArgs(serialStr, SerialNumberTypes.Base));
            }
        }

        // NEW, Nemanja's code to get the serial ID's !
        private void eex_HeadsetStateChanged(object sender, _HeadsetStateEventArgs e)
        {
            if (e.SerialNumber != null && e.SerialNumber[0] != 0)
            {
                string serialStr = byteArrayToString(e.SerialNumber);
                //Console.WriteLine(string.Format("Headset serial number: {0}", serialStr));
                OnSerialNumber(new SerialNumberArgs(serialStr, SerialNumberTypes.Headset));
            }
        }

        // NEW, Nemanja's code to get the serial ID's !
        private static string byteArrayToString(byte[] p)
        {
            StringBuilder b = new StringBuilder();
            foreach (byte x in p)
                b.Append(x.ToString("X2"));
            return b.ToString();
        }

        // now poll for current state (proximity, mobile call status, donned status, mute status)
        private void GetInitialDeviceState()
        {
            if (m_activeDevice != null)
            {
                RegisterForProximity(true);

                GetInitialMobileCallStatus(); // are we on a call?

                GetInitialDonnedStatus(); // are we donned?

                GetInitialMuteStatus();

                RequestAllSerialNumbers();

                GetLastDockedStatus();

                GetActiveAndHeldStates();

                OnLineActiveChanged(new LineActiveChangedArgs(m_hostCommand.AudioState == AudioType.AudioType_MonoOn)); // is the line active?
            }
            else
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: No device is attached, cannot get initial device state.");
            }
        }

        private void GetActiveAndHeldStates()
        {
            try
            {
                GetHoldStates();
                GetActiveStates();
                OnMultiLineStateChanged(new MultiLineStateArgs(m_activeHeldFlags));
            }
            catch (Exception)
            {
                // probably the attached device doesn't have multiline, lets inform user...
                DeviceCapabilities.HasMultiline = false;
                OnCapabilitiesChanged(EventArgs.Empty);
            }
        }

        private void DebugPrint(string methodname, string message)
        {
            if (m_debuglog != null)
                m_debuglog.DebugPrint(methodname, message);
        }

        // hard coded other device caps, beside caller id
        private void UpdateOtherDeviceCapabilities()
        {
            // LC temporarily hard-code some device capabilities
            // e.g. fact that Blackwire C710/C720 do not support proximity, docking and is not wireless
            string devname = m_devicename;
            if (devname != null)
            {
                devname = devname.ToUpper();
                if (devname.Contains("BLACKWIRE"))
                {
                    DeviceCapabilities.IsWireless = false;
                    DeviceCapabilities.HasDocking = false;
                    DeviceCapabilities.HasWearingSensor = false;
                }
                if (devname.Contains("C710") || devname.Contains("C720"))
                {
                    DeviceCapabilities.HasProximity = false;
                    DeviceCapabilities.HasCallerId = false;
                    DeviceCapabilities.HasWearingSensor = true;
                    DeviceCapabilities.HasDocking = false;
                    DeviceCapabilities.IsWireless = false;
                }
                // LC new - if using vpro or vlegend then disable docking feature...
                if (devname.Contains("BT300"))
                {
                    DeviceCapabilities.HasDocking = false;
                }
                if (devname.Contains("SAVI 7"))
                {
                    DeviceCapabilities.HasWearingSensor = false;
                    DeviceCapabilities.HasMultiline = true;
                }
            }
            OnCapabilitiesChanged(EventArgs.Empty);
        }

        // detach from device events
        void DetachDevice()
        {
            if (m_activeDevice != null)
            {
                if (m_deviceComEvents != null)
                {
                    // LC, new unregister the serial number events
                    IDeviceCOMEventsExt_Event eex = m_deviceComEvents as IDeviceCOMEventsExt_Event;
                    eex.HeadsetStateChanged -= eex_HeadsetStateChanged;
                    IBaseCOMEvents_Event be = m_deviceComEvents as IBaseCOMEvents_Event;
                    be.BaseEventReceived -= be_BaseEventReceived;

                    // unregister device event handlers
                    m_deviceComEvents.ButtonPressed -= m_deviceComEvents_Handler;
                    m_deviceComEvents.AudioStateChanged -= m_deviceComEvents_Handler;
                    m_deviceComEvents.FlashPressed -= m_deviceComEvents_Handler;
                    m_deviceComEvents.MuteStateChanged -= m_deviceComEvents_Handler;
                    m_deviceComEvents.SmartPressed -= m_deviceComEvents_Handler;
                    m_deviceComEvents.TalkPressed -= m_deviceComEvents_Handler;

                    Marshal.ReleaseComObject(m_deviceComEvents);
                    m_deviceComEvents = null;
                }
                if (m_deviceListenerEvents != null)
                {
                    // unregister device listener event handlers
                    m_deviceListenerEvents.ATDStateChanged -= m_deviceListenerEvents_Handler;
                    m_deviceListenerEvents.BaseButtonPressed -= m_deviceListenerEvents_Handler;
                    m_deviceListenerEvents.BaseStateChanged -= m_deviceListenerEvents_Handler;
                    m_deviceListenerEvents.HeadsetButtonPressed -= m_deviceListenerEvents_Handler;
                    m_deviceListenerEvents.HeadsetStateChanged -= m_deviceListenerEvents_HandlerMethods;

                    RegisterForProximity(false);
                    Marshal.ReleaseComObject(m_deviceListenerEvents);
                    m_deviceListenerEvents = null;
                }

                Marshal.ReleaseComObject(m_activeDevice);
                m_activeDevice = null;

                m_hostCommand = null;
                m_hostCommandExt = null;
                m_atdCommand = null;

                // LC Device was disconnected, clear down the GUI state...
                m_mobIncoming = false; // clear mobile call direction flag
                m_voipIncoming = false; // clear call direction flag
                OnNotOnCall(EventArgs.Empty);
                OnNotOnMobileCall(EventArgs.Empty);

                OnSerialNumber(new SerialNumberArgs("", SerialNumberTypes.Base));
                OnSerialNumber(new SerialNumberArgs("", SerialNumberTypes.Headset));

                // LC Device was disconnected, remove capability data
                DeviceCapabilities = new SpokesDeviceCaps(false, false, false, false, false, false); // no device = no capabilities!
                m_devicename = "";
                OnCapabilitiesChanged(EventArgs.Empty);

                // trigger user's event handler
                OnDetached(EventArgs.Empty);

                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: DetachedEventHandler from device");
            }
            m_devicename = "";
        }

        private void RegisterForProximity(bool register)
        {
            DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: About to register for proximity.");
            try
            {
                if (m_hostCommandExt != null)
                {
                    m_hostCommandExt.EnableProximity(register); // enable proximity reporting for device
                    if (register) m_hostCommandExt.GetProximity();    // request to receive asyncrounous near/far proximity event to HeadsetStateChanged event handler. (note: will return it once. To get continuous updates of proximity you would need a to call GetProximity() repeatedly, e.g. in a worker thread).
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Completed request to register for proximity.");

                    DeviceCapabilities.HasProximity = true;

                    // Tweak availability of proximity per-device...
                    string devname = m_devicename.ToUpper();
                    if (devname.Contains("C710") || devname.Contains("C720"))
                    {
                        DeviceCapabilities.HasProximity = false;
                    }

                    OnCapabilitiesChanged(EventArgs.Empty);
                }
            }
            catch (System.Exception)
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: INFO: proximity may not be supported on your device.");
                // uh-oh proximity may not be supported... disable it as option in GUI
                DeviceCapabilities.HasProximity = false;
                OnCapabilitiesChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Instruct Spokes to tell us the serial numbers of attached Plantronics device, i.e. headset and base/usb adaptor.
        /// </summary>
        public void RequestAllSerialNumbers()
        {
            DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: About to request serial numbers.");
            RequestSingleSerialNumber(SerialNumberTypes.Base);
            RequestSingleSerialNumber(SerialNumberTypes.Headset);
        }

        // Some internal methods to get line active/held states of multi-line devices:
        private void GetHoldStates()
        {
            m_activeHeldFlags.DeskphoneHeld = GetHoldState(LineType.LineType_PSTN);
            m_activeHeldFlags.MobileHeld = GetHoldState(LineType.LineType_Mobile);
            m_activeHeldFlags.PCHeld = GetHoldState(LineType.LineType_VOIP);
            DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("Current Interface Hold States: PSTN: {0} Mobile: {1} VOIP: {2}",
                m_activeHeldFlags.DeskphoneHeld, m_activeHeldFlags.MobileHeld, m_activeHeldFlags.PCHeld));
        }

        private void GetActiveStates()
        {
            m_activeHeldFlags.DeskphoneActive = GetActiveState(LineType.LineType_PSTN);
            m_activeHeldFlags.MobileActive = GetActiveState(LineType.LineType_Mobile);
            m_activeHeldFlags.PCActive = GetActiveState(LineType.LineType_VOIP);
            DebugPrint(MethodInfo.GetCurrentMethod().Name, String.Format("Current Interface Is Line Active States: PSTN: {0} Mobile: {1} VOIP: {2}",
                m_activeHeldFlags.DeskphoneActive, m_activeHeldFlags.MobileActive, m_activeHeldFlags.PCActive));
        }

        private bool GetHoldState(LineType lineType)
        {
            bool state = false; // default - unknown state

            //Get the current hold state
            if (m_hostCommandExt != null)
            {
                state = m_hostCommandExt.GetHoldState(lineType);
            }

            return state;
        }

        private bool GetActiveState(LineType lineType)
        {
            bool state = false; // default - unknown state

            //Get the current active state
            if (m_hostCommandExt != null)
            {
                state = m_hostCommandExt.IsLineActive(lineType);
            }

            return state;
        }

        // new get last docked status of device when app first runs
        private bool GetLastDockedStatus()
        {
            bool docked = false;
            try
            {
                if (m_hostCommandExt != null)
                {
                    docked = m_hostCommandExt.IsHeadsetDocked;
                    if (docked) OnDocked(new DockedStateArgs(true, true));
                    else OnUnDocked(new DockedStateArgs(false, true));
                }
            }
            catch (Exception)
            {
                // probably we don't support docking, lets inform user...
                DeviceCapabilities.HasDocking = false;
                OnCapabilitiesChanged(EventArgs.Empty);
            }
            return docked;
        }

        /// <summary>
        /// Instructs a mobile that is paired with Plantronics device to dial an outbound mobile call.
        /// </summary>
        /// <param name="numbertodial">The phone number you wish the mobile to call.</param>
        public void DialMobileCall(string numbertodial)
        {
            if (m_atdCommand != null)
            {
                m_atdCommand.MakeMobileCall(numbertodial);
            }
            else
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error, unable to dial mobile call. atd command is null.");
            }
        }

        /// <summary>
        /// Instructs a mobile that is paired with Plantronics device to answer an inbound (ringing) mobile call
        /// </summary>
        public void AnswerMobileCall()
        {
            if (m_atdCommand != null)
            {
                m_atdCommand.AnswerMobileCall();
            }
            else
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error, unable to answer mobile call. atd command is null.");
            }
        }

        /// <summary>
        /// Instructs a mobile that is paired with Plantronics device to end on ongoing mobile call
        /// </summary>
        public void EndMobileCall()
        {
            if (m_atdCommand != null)
            {
                m_atdCommand.EndMobileCall();
            }
            else
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error, unable to end mobile call. atd command is null.");
            }
        }

        private void GetInitialMobileCallStatus()
        {
            if (m_atdCommand != null)
            {
                try
                {
                    m_atdCommand.GetMobileCallStatus(); // are we on a call?

                    bool tmpHasCallerId = true; // device does support caller id feature

                    // LC temporarily hard-code some device capabilities
                    // e.g. fact that Blackwire C710/C720 do not support proximity, docking and is not wireless
                    string devname = m_devicename;
                    if (devname != null)
                    {
                        devname = devname.ToUpper();
                        if (devname.Contains("SAVI 7"))
                        {
                            tmpHasCallerId = false; // Savi 7xx does not support caller id feature
                        }
                        if (devname.Contains("BLACKWIRE"))
                        {
                            tmpHasCallerId = false; // Blackwire range does not support caller id feature
                        }
                        if (devname.Contains("C710") || devname.Contains("C720"))
                        {
                            tmpHasCallerId = false; // Blackwire 700 range does not support caller id feature
                        }
                    }

                    DeviceCapabilities.HasCallerId = tmpHasCallerId; // set whether device supports caller id feature
                    OnCapabilitiesChanged(EventArgs.Empty);
                }
                catch (System.Exception e)
                {
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: INFO: Exception occured getting mobile call status\r\nException = " + e.ToString());
                    DeviceCapabilities.HasCallerId = false;
                    OnCapabilitiesChanged(EventArgs.Empty);
                }
            }
            else
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Error, unable to get mobile status. atd command is null.");
                DeviceCapabilities.HasCallerId = false; // device does not support caller id feature
                OnCapabilitiesChanged(EventArgs.Empty);
            }
        }

        // new get last donned status of device when app first runs
        private void GetInitialDonnedStatus()
        {
            try
            {
                if (m_hostCommandExt != null)
                {
                    HeadsetState laststate = m_hostCommandExt.HeadsetState;
                    switch (laststate)
                    {
                        case HeadsetState.HeadsetState_Doff:
                            OnTakenOff(new WearingStateArgs(false, true));
                            break;
                        case HeadsetState.HeadsetState_Don:
                            OnPutOn(new WearingStateArgs(true, true));
                            break;
                    }
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Last donned state was: " + laststate);
                }
            }
            catch (Exception e)
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Other Exception in GetInitialDonnedStatus(): " + e.ToString());
            }
        }

        private void GetInitialMuteStatus()
        {
            try
            {
                if (m_hostCommand != null)
                {
                    OnMuteChanged(new MuteChangedArgs(m_hostCommand.Mute));
                    DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: Last mute state was: " + m_hostCommand.Mute);
                }
            }
            catch (Exception e)
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Other Exception in GetInitialMuteStatus(): " + e.ToString());
            }
        }

        /// <summary>
        /// Allows your softphone application to inform Plantronics device about an incoming call. The Plantronics device will then automatically ring. 
        /// Note: will automatically open audio/rf link to wireless device.
        /// </summary>
        /// <param name="callid">A unique numeric identifier for the call that your application and Spokes will use to identify it as.</param>
        /// <param name="contactname">Optional caller's contact name that will display on Plantronics display devices, e.g. Calisto P800 and P240 devices.</param>
        /// <returns>Boolean indicating if command was issued successfully or not.</returns>
        internal bool IncomingCall(int callid, string contactname = "")
        {
            bool success = false;
            try
            {
                if (m_comSession != null)
                {
                    ContactCOM contact = new ContactCOM() { Name = contactname };
                    CallCOM call = new CallCOM() { Id = callid };
                    m_comSession.CallCommand.IncomingCall(call, contact, RingTone.RingTone_Unknown, AudioRoute.AudioRoute_ToHeadset);
                    ConnectAudioLinkToDevice(true);
                    success = true;
                }
            }
            catch (Exception) { success = false; }
            return success;
        }

        /// <summary>
        /// Allows your softphone application to inform Plantronics device about an outgoing call. Note: will automatically open audio/rf link to wireless device.
        /// </summary>
        /// <param name="callid">A unique numeric identifier for the call that your application and Spokes will use to identify it as.</param>
        /// <param name="contactname">Optional caller's contact name that will display on Plantronics display devices, e.g. Calisto P800 and P240 devices.</param>
        /// <returns>Boolean indicating if command was issued successfully or not.</returns>
        internal bool OutgoingCall(int callid, string contactname = "")
        {
            bool success = false;
            try
            {
                if (m_comSession != null)
                {
                    ContactCOM contact = new ContactCOM() { Name = contactname };
                    CallCOM call = new CallCOM() { Id = callid };
                    m_comSession.CallCommand.OutgoingCall(call, contact, AudioRoute.AudioRoute_ToHeadset);
                    ConnectAudioLinkToDevice(true);
                    success = true;
                }
            }
            catch (Exception) { success = false; }
            return success;
        }

        /// <summary>
        /// Instructs Spokes to end an ongoing softphone call.
        /// </summary>
        /// <param name="callid">The unique numeric id that defines which softphone call you want to end.</param>
        /// <returns>Boolean indicating if the command was called succesfully or not.</returns>
        internal bool EndCall(int callid)
        {
            bool success = false;
            try
            {
                if (m_comSession != null)
                {
                    CallCOM call = new CallCOM() { Id = callid };
                    m_comSession.CallCommand.TerminateCall(call);
                    success = true;
                }
            }
            catch (Exception) { success = false; }
            return success;
        }

        /// <summary>
        /// Instruct Spokes to tell us a serial number of the attached Plantronics device, i.e. headset or base/usb adaptor.
        /// </summary>
        /// <param name="serialNumberType">Allows you to say if you would like the headset or base/usb adaptor serial number.</param>
        internal void RequestSingleSerialNumber(SerialNumberTypes serialNumberType)
        {
            DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: About to request serial number for: " + serialNumberType);
            try
            {
                if (m_hostCommandExt != null)
                {
                    switch (serialNumberType)
                    {
                        case SerialNumberTypes.Headset:
                            m_hostCommandExt.GetSerialNumber(DeviceType.DeviceType_Headset);
                            break;
                        case SerialNumberTypes.Base:
                            m_hostCommandExt.GetSerialNumber(DeviceType.DeviceType_Base);
                            break;
                    }
                }
            }
            catch (System.Exception)
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: INFO: serial number may not be supported on your device.");
            }
        }

        #region Call Control Helper Classes
        // Call abstraction
        public class CallCOM : Interop.Plantronics.CallId
        {
            private int id = 0;
            #region ICall Members

            public int ConferenceId
            {
                get { return 0; }
                set { }
            }

            public int Id
            {
                get { return id; }
                set { id = value; }
            }

            public bool InConference
            {
                get { return false; }
                set { }
            }

            #endregion
        }

        // Contact abstraction
        public class ContactCOM : Interop.Plantronics.Contact
        {
            private string email;
            private string friendlyName;
            private string homePhone;
            private int id;
            private string mobPhone;
            private string name;
            private string phone;
            private string sipUri;
            private string workPhone;
            #region IContact Members

            public string Email
            {
                get
                {
                    return email;
                }
                set
                {
                    email = value;
                }
            }

            public string FriendlyName
            {
                get
                {
                    return friendlyName;
                }
                set
                {
                    friendlyName = value;
                }
            }

            public string HomePhone
            {
                get
                {
                    return homePhone;
                }
                set
                {
                    homePhone = value;
                }
            }

            public int Id
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                }
            }

            public string MobilePhone
            {
                get
                {
                    return mobPhone;
                }
                set
                {
                    mobPhone = value;
                }
            }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public string Phone
            {
                get
                {
                    return phone;
                }
                set
                {
                    phone = value;
                }
            }

            public string SipUri
            {
                get
                {
                    return sipUri;
                }
                set
                {
                    sipUri = value;
                }
            }

            public string WorkPhone
            {
                get
                {
                    return workPhone;
                }
                set
                {
                    workPhone = value;
                }
            }

            #endregion
        }
        #endregion

        /// <summary>
        /// This function will establish or close the audio link between PC and the Plantronics audio device.
        /// It is required to be called where your app needs audio (i.e. when on a call) in order to support Plantronics wireless devices, because
        /// opening the audio link will also bring up the RF link.
        /// </summary>
        /// <param name="connect">Tells Spokes whether to open or close the audio/rf link to device</param>
        internal void ConnectAudioLinkToDevice(bool connect)
        {
            DebugPrint(MethodInfo.GetCurrentMethod().Name, "INFO: Setting audio link active = " + connect.ToString());
            if (m_activeDevice != null)
            {
                m_activeDevice.HostCommand.AudioState =
                    connect ? AudioType.AudioType_MonoOn : AudioType.AudioType_MonoOff;
            }
            else
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: INFO: cannot set audio link state, no device");
            }
        }

        /// <summary>
        /// This function return true or false to indicate whether there is an active audio/rf
        /// link between PC and the device.
        /// </summary>
        internal bool IsAudioLinkToDeviceActive()
        {
            bool isActive = false;
            DebugPrint(MethodInfo.GetCurrentMethod().Name, "INFO: Querying audio link active.");
            if (m_activeDevice != null)
            {
                isActive = m_activeDevice.HostCommand.AudioState == AudioType.AudioType_MonoOn;
            }
            else
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: INFO: cannot get audio link state, no device");
            }
            return isActive;
        }

        /// <summary>
        /// Set the microphone mute state of the attached Plantronics device.
        /// Note: For wireless devices mute only works when the audio/rf link is active (see also ConnectAudioLinkToDevice method).
        /// </summary>
        /// <param name="mute">A boolean indicating if you want mute on or off</param>
        internal void SetMute(bool mute)
        {
            DebugPrint(MethodInfo.GetCurrentMethod().Name, "INFO: Setting mute = " + mute.ToString());
            if (m_activeDevice != null && m_hostCommandExt != null)
            {
                m_hostCommandExt.SetHeadsetMute(mute);
            }
            else
            {
                DebugPrint(MethodInfo.GetCurrentMethod().Name, "Spokes: INFO: cannot set mute, no device");
            }
        }

        /// <summary>
        /// Instruct the Plantronics multiline device to activate or deactivate the specified phone line.
        /// </summary>
        /// <param name="multiline_LineType">The line to activate or deactive, PC, Mobile or Desk Phone</param>
        /// <param name="activate">Boolean indicating whether to activate or de-activate the line</param>
        internal void SetLineActive(Multiline_LineType multiline_LineType, bool activate)
        {
            if (m_hostCommandExt != null)
            {
                switch (multiline_LineType)
                {
                    case Multiline_LineType.PC:
                        m_hostCommandExt.SetActiveLink(LineType.LineType_VOIP, activate);
                        break;
                    case Multiline_LineType.Mobile:
                        m_hostCommandExt.SetActiveLink(LineType.LineType_Mobile, activate);
                        break;
                    case Multiline_LineType.Deskphone:
                        m_hostCommandExt.SetActiveLink(LineType.LineType_PSTN, activate);
                        break;
                }
            }
        }

        /// <summary>
        /// Instruct the Plantronics multiline device to place on hold or remove from hold the specified phone line.
        /// </summary>
        /// <param name="multiline_LineType">The line to place on hold or remove from hold, PC, Mobile or Desk Phone</param>
        /// <param name="hold">Boolean indicating whether to hold or un-hold the line</param>
        internal void SetLineHold(Multiline_LineType multiline_LineType, bool hold)
        {
            if (m_hostCommandExt != null)
            {
                switch (multiline_LineType)
                {
                    case Multiline_LineType.PC:
                        m_hostCommandExt.Hold(LineType.LineType_VOIP, hold);
                        break;
                    case Multiline_LineType.Mobile:
                        m_hostCommandExt.Hold(LineType.LineType_Mobile, hold);
                        break;
                    case Multiline_LineType.Deskphone:
                        m_hostCommandExt.Hold(LineType.LineType_PSTN, hold);
                        break;
                }
            }
        }
    }
}
