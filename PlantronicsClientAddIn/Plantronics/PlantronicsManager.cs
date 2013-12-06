using ININ.InteractionClient.AddIn;
using Interop.Plantronics;
using PlantronicsClientAddIn.Interactions;
using PlantronicsClientAddIn.Status;
using System;
using System.Diagnostics;

namespace PlantronicsClientAddIn.Plantronics
{
    /// <summary>
    /// The purpose of this class is to handle events coming from the Plantronics API.  
    /// It is the central point for making things happen, based on different events, we can perform specific actions in CIC.
    /// </summary>
    public class PlantronicsManager : IDisposable
	{
        private readonly ISessionCOMManager m_sessionComManager = null;
        private IComSession m_comSession = null;
        private IDevice m_device = null;
        private ISessionCOMManagerEvents_Event m_sessionManagerEvents;

        private IDeviceCOMEvents_Event m_deviceComEvents;
        private IDeviceListenerCOMEvents_Event m_deviceListenerEvents;

		private IStatusManager _statusManager;
		private IInteractionManager _interactionManager;
		private ITraceContext _traceContext;
        private INotificationService _notificationService;

		public PlantronicsManager (IStatusManager statusManager, 
                                    IInteractionManager interactionManager, 
                                    INotificationService notificationService,
                                    ITraceContext traceContext)
		{
			_statusManager = statusManager;
			_interactionManager = interactionManager;
			_traceContext = traceContext;
            _notificationService = notificationService;
            
            m_sessionComManager = new SessionComManagerClass();
            m_sessionManagerEvents = m_sessionComManager as ISessionCOMManagerEvents_Event;
            m_comSession = m_sessionComManager.Register("Interaction Client Plantronics AddIn");

            // Now check if our plugin session was created
            if (m_comSession != null)
            {
                // detect devices added/removed
                m_sessionManagerEvents.DeviceStateChanged += OnDeviceStateChanged;

                //Get current Device
                m_device = m_comSession.ActiveDevice;

                // if we have a device register for events
                if (m_device != null)
                {
                    // Register for device events
                    RegisterEvents();
                }
            }
		}

        private void LogDeviceInfo(IDevice device)
        {
            _traceContext.Status("Plantronics connected device information");
            _traceContext.Status("Plantronics: Internal Name- " + device.InternalName);
            _traceContext.Status("Plantronics: Is Attached- " + device.IsAttached);
            _traceContext.Status("Plantronics: Manufacturer Name- " + device.ManufacturerName);
            _traceContext.Status("Plantronics: Product ID- " + device.ProductID);
            _traceContext.Status("Plantronics: Product Name- " + device.ProductName);
            _traceContext.Status("Plantronics: Serial Number- " + device.SerialNumber);
            _traceContext.Status("Plantronics: Vendor ID- " + device.VendorID);
            _traceContext.Status("Plantronics: Version Number- " + device.VersionNumber);
        }

        private void RegisterEvents()
        {
            LogDeviceInfo(m_device);

            // Register for some device events
            m_deviceComEvents = m_device.DeviceEvents as IDeviceCOMEvents_Event;
            m_deviceComEvents.ButtonPressed += new IDeviceCOMEvents_ButtonPressedEventHandler(OnButtonPressed);

            m_deviceListenerEvents = m_device.DeviceListener as IDeviceListenerCOMEvents_Event;
            if (m_deviceListenerEvents != null)
            {
                m_deviceListenerEvents.HeadsetStateChanged += new IDeviceListenerCOMEvents_HeadsetStateChangedEventHandler(OnHeadsetStateChanged);
            }
        }

        private void UnRegisterEvents()
        {
            m_deviceComEvents.ButtonPressed -= OnButtonPressed;
            m_deviceListenerEvents = m_device.DeviceListener as IDeviceListenerCOMEvents_Event;
            
            if (m_deviceListenerEvents != null)
            {
                m_deviceListenerEvents.HeadsetStateChanged -= OnHeadsetStateChanged;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHeadsetStateChanged(object sender, _DeviceListenerEventArgs e)
        {
            Debug.WriteLine("OnHeadsetStateChanged " + e.ToString());
            _traceContext.Status("OnHeadsetStateChanged " + e.ToString());
            if (e.HeadsetStateChange == HeadsetStateChange.HeadsetStateChange_InRange)
            {
                
                _notificationService.Notify("Headset in range", "Headset", NotificationType.Info, TimeSpan.FromSeconds(3));
                _statusManager.SetLastStatus();
                _interactionManager.PickupHeldCall();
            }
            else if (e.HeadsetStateChange == HeadsetStateChange.HeadsetStateChange_OutofRange)
            {
                _notificationService.Notify("Headset out of range", "Headset", NotificationType.Warning, TimeSpan.FromSeconds(3));
                _statusManager.SetToAwayFromDesk();
                _interactionManager.HoldCall();
            }
        }

        private void OnButtonPressed(object sender, _DeviceEventArgs e)
        {
            Debug.WriteLine("OnButtonPressed " + e.ToString());
            _traceContext.Status("OnButtonPressed " + e.ToString());
            if (e.ButtonPressed == HeadsetButton.HeadsetButton_Talk)
            {
                _interactionManager.PickupOrDisconnectCall();
            }
        }

        private void OnDeviceStateChanged(object sender, _DeviceStateEventArgs e)
        {
            _traceContext.Status("OnDeviceStateChanged " + e.ToString());
            Debug.WriteLine("OnDeviceStateChanged " + e.ToString());
            switch (e.State)
            {
                case DeviceState.DeviceState_Added:
                    // register event handlers
                    if (m_device != null)
                    {
                        UnRegisterEvents();
                    }

                    m_device = m_comSession.ActiveDevice;
                    RegisterEvents();
                    _notificationService.Notify(String.Format("{0} headset connected", m_device.ProductName), "Headset", NotificationType.Info, TimeSpan.FromSeconds(2));
                    break;
                case DeviceState.DeviceState_Removed:
                    // unregister event handlers
                    if (m_device != null)
                    {
                        UnRegisterEvents();
                    }
                    m_device = null;
                    _notificationService.Notify("Plantronics Headset Removed", "Headset", NotificationType.Info, TimeSpan.FromSeconds(2));
                    break;
            }
        }

        public void Dispose()
        {
            try
            {
                UnRegisterEvents();
                m_device = null;
                m_sessionManagerEvents.DeviceStateChanged -= OnDeviceStateChanged;
                m_sessionComManager.UnRegister(m_comSession);
            }
            catch { }
        }
    }
}

