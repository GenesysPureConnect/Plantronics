using ININ.InteractionClient.AddIn;
using Interop.Plantronics;
using Plantronics.UC.SpokesWrapper;
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
   
        private Spokes _spokes;

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

            _spokes = Spokes.Instance;

            _spokes.PutOn += OnHeadsetPutOn;
            _spokes.TakenOff += OnHeadsetTakenOff;
            
            _spokes.Docked += OnHeadsetDocked;
            _spokes.UnDocked += OnHeadsetUnDocked;

            _spokes.Attached += OnHeadsetAttached;
            _spokes.Detached += OnHeadsetDetached;

            _spokes.InRange += OnHeadsetInRange;
            _spokes.OutOfRange += OnHeadsetOutOfRange;

            _spokes.ButtonPress += OnDeviceButtonPress;

            _spokes.Connect("Interaction Client AddIn");
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
            _notificationService.Notify("Headset out of range", "Headset", NotificationType.Warning, TimeSpan.FromSeconds(3));
            _statusManager.SetToAwayFromDesk();
        }

        private void OnHeadsetInRange(object sender, EventArgs e)
        {
            Debug.WriteLine("OnHeadsetInRange ");
            _traceContext.Status("OnHeadsetInRange ");

            _notificationService.Notify("Headset in range", "Headset", NotificationType.Info, TimeSpan.FromSeconds(3));
            _statusManager.SetLastStatus();
        }

        private void OnHeadsetDetached(object sender, EventArgs e)
        {
            Debug.WriteLine("OnHeadsetDetached " );
            _traceContext.Status("OnHeadsetDetached ");

            _notificationService.Notify("Plantronics Headset Detached", "Headset", NotificationType.Info, TimeSpan.FromSeconds(2));
            _statusManager.SetToAwayFromDesk();
        }

        private void OnHeadsetAttached(object sender, AttachedArgs e)
        {
            Debug.WriteLine("OnHeadsetAttached " + e.m_device.ManufacturerName + " " + e.m_device.SerialNumber);
            _traceContext.Status("OnHeadsetAttached " + e.ToString());
            _notificationService.Notify(String.Format("{0} headset connected", _spokes.GetDevice.ProductName), "Headset", NotificationType.Info, TimeSpan.FromSeconds(2));
            LogDeviceInfo(e.m_device);
            _statusManager.SetLastStatus();
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
        
        private void LogDeviceInfo(ICOMDevice device)
        {
            _traceContext.Status("Plantronics connected device information");
            _traceContext.Status("Plantronics: Internal Name- " + device.InternalName);
            _traceContext.Status("Plantronics: Manufacturer Name- " + device.ManufacturerName);
            _traceContext.Status("Plantronics: Product Name- " + device.ProductName);
            _traceContext.Status("Plantronics: Serial Number- " + device.SerialNumber);
            _traceContext.Status("Plantronics: Version Number- " + device.VersionNumber);
        }
      
        public void Dispose()
        {
            _spokes = null;
            _spokes.Disconnect();
        }
    }
     
}

