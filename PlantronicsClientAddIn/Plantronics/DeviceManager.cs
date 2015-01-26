using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plantronics.UC.SpokesWrapper;
using System.Diagnostics;
using ININ.InteractionClient.AddIn;

namespace PlantronicsClientAddIn.Plantronics
{
    class DeviceManager :IDeviceManager
    {
        private bool _isHeadsetConnected = false;
        private ITraceContext _traceContext;
        private Spokes _spokes;

        public event Spokes.AttachedEventHandler PlantronicsDeviceAttached;
        public event Spokes.DetachedEventHandler PlantronicsDeviceDetached;
        public event Spokes.ConnectedEventHandler HeadsetConnected;
        public event Spokes.DisconnectedEventHandler HeadsetDisconnected;
        public event Spokes.MuteChangedEventHandler MuteChanged;
        public event EventHandler TalkButtonPressed;

        public bool IsHeadsetConnected
        {
            get { return _isHeadsetConnected; }
        }

        public bool IsHeadsetMuted
        {
            get { return _spokes.GetMute(); }
        }

        public bool IsDeviceConnected
        {
            get { return _spokes.HasDevice; }
        }

        public void ToggleMute()
        {
            _spokes.SetMute(!_spokes.GetMute());
        }

        public DeviceManager(ITraceContext traceContext, DebugLogger logger)
        {
            _traceContext = traceContext;

            _spokes = Spokes.Instance;
            _spokes.SetLogger(logger);

            _spokes.Attached += OnDeviceAttached;
            _spokes.Detached += OnDeviceDetached;

            _spokes.Connected +=OnHeadsetConnected;
            _spokes.Disconnected += OnHeadsetDisconnected;

            _spokes.MuteChanged += OnMuteChanged;

            _spokes.Connect("Interaction Client AddIn");
            _spokes.ButtonPress += OnButtonPress;
        }

        private void OnButtonPress(object sender, ButtonPressArgs e)
        {

            _traceContext.Status(String.Format("{0} pressed", e.headsetButton));

            switch (e.headsetButton)
            {
                case Interop.Plantronics.DeviceHeadsetButton.HeadsetButton_Talk:
                    if (TalkButtonPressed != null)
                    {
                        TalkButtonPressed(this, EventArgs.Empty);
                    }
                    break;
                
                default:
                    break;
            }
        }

        private void OnMuteChanged(object sender, MuteChangedArgs e)
        {
            if (MuteChanged != null)
            {
                MuteChanged(this, e);
            }
        }

        private void OnHeadsetConnected(object sender, ConnectedStateArgs e)
        {
            Debug.WriteLine("OnHeadsetConnected ");
            _traceContext.Status("OnHeadsetConnected ");
            _isHeadsetConnected = true;

            if (HeadsetConnected != null)
            {
                HeadsetConnected(this, e);
            }
        }

        private void OnHeadsetDisconnected(object sender, ConnectedStateArgs e)
        {
            Debug.WriteLine("OnHeadsetDisconnected ");
            _traceContext.Status("OnHeadsetDisconnected ");

            _isHeadsetConnected = false;

            if (HeadsetDisconnected != null)
            {
                HeadsetDisconnected(this, e);
            }
        }

        private void OnDeviceDetached(object sender, EventArgs e)
        {
            Debug.WriteLine("OnDeviceDetached ");
            _traceContext.Status("OnDeviceDetached ");

            InternalName = String.Empty;
            ManufacturerName = String.Empty;
            ProductName = String.Empty;
            SerialNumber = String.Empty;
            VersionNumber = 0;

            if (PlantronicsDeviceDetached != null)
            {
                PlantronicsDeviceDetached(this, e);
            }
        }

        private void OnDeviceAttached(object sender, AttachedArgs e)
        {
            Debug.WriteLine("OnDeviceAttached ");
            _traceContext.Status("OnDeviceAttached "); ;

            InternalName = e.m_device.InternalName;
            ManufacturerName = e.m_device.ManufacturerName;
            ProductName = e.m_device.ProductName;
            SerialNumber = e.m_device.SerialNumber;
            VersionNumber = e.m_device.VersionNumber;
           

            if (PlantronicsDeviceAttached != null)
            {
                PlantronicsDeviceAttached(this, e);
            }
        }

        public string InternalName
        {
            get;
            private set;
        }

        public string ManufacturerName
        {
            get;
            private set;
        }

        public string ProductName
        {
            get;
            private set;
        }

        public ushort VersionNumber
        {
            get;
            private set;
        }

        public string SerialNumber
        {
            get;
            private set;
        }

        public void TraceSettings()
        {
            _traceContext.Status("Plantronics connected device information");
            _traceContext.Status("Plantronics: Internal Name- " + InternalName);
            _traceContext.Status("Plantronics: Manufacturer Name- " + ManufacturerName);
            _traceContext.Status("Plantronics: Product Name- " + ProductName);
            _traceContext.Status("Plantronics: Serial Number- " + SerialNumber);
            _traceContext.Status("Plantronics: Version Number- " + VersionNumber);
        }

        public void Dispose()
        {
            _spokes.Disconnect();
            _spokes = null;
        }
    }
}
