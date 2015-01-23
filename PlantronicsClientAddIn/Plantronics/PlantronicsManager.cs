using ININ.InteractionClient.AddIn;
using Interop.Plantronics;
using Plantronics.UC.SpokesWrapper;
using PlantronicsClientAddIn.Interactions;
using PlantronicsClientAddIn.Status;
using System;
using System.Diagnostics;
using PlantronicsClientAddIn.Settings;

namespace PlantronicsClientAddIn.Plantronics
{
    
    /// <summary>
    /// The purpose of this class is to handle events coming from the Plantronics API.  
    /// It is the central point for making things happen, based on different events, we can perform specific actions in CIC.
    /// </summary>
    public class PlantronicsManager : IDisposable
	{
   
        private Spokes _spokes;

		private ICicStatusService _statusManager;
		private IInteractionManager _interactionManager;
		private ITraceContext _traceContext;
        private INotificationService _notificationService;
        private ISettingsManager _settingsManager;
     

		public PlantronicsManager (ICicStatusService statusManager, 
                                    IInteractionManager interactionManager, 
                                    INotificationService notificationService,
                                    ISettingsManager settingsManager,
                                    IDeviceStatus deviceSettings,
                                    ITraceContext traceContext, 
                                    DebugLogger logger)
		{
			_statusManager = statusManager;
			_interactionManager = interactionManager;
			_traceContext = traceContext;
            _notificationService = notificationService;
            _settingsManager = settingsManager;
            _deviceSettings = deviceSettings;

            _spokes = Spokes.Instance;
            _spokes.SetLogger(logger);

            _spokes.PutOn += OnHeadsetPutOn;
            _spokes.TakenOff += OnHeadsetTakenOff;
            
            _spokes.Docked += OnHeadsetDocked;
            _spokes.UnDocked += OnHeadsetUnDocked;

            _spokes.Attached += OnHeadsetAttached;
            _spokes.Detached += OnHeadsetDetached;

            _spokes.InRange += OnHeadsetInRange;
            _spokes.OutOfRange += OnHeadsetOutOfRange;

            _spokes.Connected += _spokes_Connected;
            
            _spokes.ButtonPress += OnDeviceButtonPress;

            _spokes.Connect("Interaction Client AddIn");

            
		}

        void _spokes_Connected(object sender, ConnectedStateArgs e)
        {
            Debug.WriteLine("connected " + e.m_isInitialStateEvent.ToString());
        }

        private void OnDeviceButtonPress(object sender, ButtonPressArgs e)
        {
            Debug.WriteLine("OnButtonPressed " + e.headsetButton);
            _traceContext.Status("OnButtonPressed " + e.headsetButton);
            if (e.headsetButton == DeviceHeadsetButton.HeadsetButton_Talk)
            {
                _interactionManager.PickupOrDisconnectCall();
            }

        }

        private void OnHeadsetOutOfRange(object sender, EventArgs e)
        {
            Debug.WriteLine("OnHeadsetOutOfRange " );
            _traceContext.Status("OnHeadsetOutOfRange " );

            if (_settingsManager.HeadsetDisconnectNotification)
            {
                _notificationService.Notify("Headset out of range", "Headset", NotificationType.Warning, TimeSpan.FromSeconds(3));
            }

            if (_settingsManager.HeadsetDisconnectChangeStatus)
            {
                _statusManager.SetStatus(_settingsManager.OutOfRangeStatusKey);
            }
        }

        private void OnHeadsetInRange(object sender, EventArgs e)
        {
            Debug.WriteLine("OnHeadsetInRange ");
            _traceContext.Status("OnHeadsetInRange ");

            if (_settingsManager.HeadsetConnectNotification)
            {
                _notificationService.Notify("Headset in range", "Headset", NotificationType.Info, TimeSpan.FromSeconds(3));
            }

            if (_settingsManager.HeadsetConnectChangeStatus)
            {
                _statusManager.SetStatus(_settingsManager.HeadsetConnectStatusKey);
            }
        }

        private void OnHeadsetDetached(object sender, EventArgs e)
        {
            Debug.WriteLine("OnHeadsetDetached " );
            _traceContext.Status("OnHeadsetDetached ");

            if (_settingsManager.DisconnectNotification)
            {
                _notificationService.Notify("Plantronics Headset Detached", "Headset", NotificationType.Info, TimeSpan.FromSeconds(2));
            }

            if (_settingsManager.DisconnectChangeStatus)
            {
                _statusManager.SetStatus(_settingsManager.DisconnectStatusKey);
            }

            _deviceSettings.DeviceDisconnected();
        }

        private void OnHeadsetAttached(object sender, AttachedArgs e)
        {
            Debug.WriteLine("OnHeadsetAttached " + e.m_device.ManufacturerName + " " + e.m_device.SerialNumber);
            _traceContext.Status("OnHeadsetAttached " + e.ToString());
            
            _deviceSettings.DeviceConnected(e.m_device);
            _deviceSettings.TraceSettings();

            if (_settingsManager.ConnectNotification)
            {
                _notificationService.Notify(String.Format("{0} headset connected", _spokes.GetDevice.ProductName), "Headset", NotificationType.Info, TimeSpan.FromSeconds(2));
            }

            if (_settingsManager.ConnectChangeStatus)
            {
                _statusManager.SetStatus(_settingsManager.ConnectStatusKey);
            }
        }

        
        private void OnHeadsetUnDocked(object sender, DockedStateArgs e)
        {
            Debug.WriteLine("OnHeadsetUnDocked " );
            _traceContext.Status("OnHeadsetUnDocked ");
            _interactionManager.PickupAlertingCall();
        }

        private void OnHeadsetDocked(object sender, DockedStateArgs e)
        {
            Debug.WriteLine("OnHeadsetDocked " );
            _traceContext.Status("OnHeadsetDocked " );
            _interactionManager.DisconnectCall();

        }

        private void OnHeadsetTakenOff(object sender, WearingStateArgs e)
        {
            Debug.WriteLine("OnHeadsetTakenOff " );
            _traceContext.Status("OnHeadsetTakenOff " );
            _interactionManager.HoldCall();
        }

        private void OnHeadsetPutOn(object sender, WearingStateArgs e)
        {
            Debug.WriteLine("OnHeadsetPutOn " );
            _traceContext.Status("OnHeadsetPutOn " );
            _interactionManager.PickupHeldCall();
        }
        
      
        public void Dispose()
        {
            _spokes = null;
            _spokes.Disconnect();
        }
    }
     
}

