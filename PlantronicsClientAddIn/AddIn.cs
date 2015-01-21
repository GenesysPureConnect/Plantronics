using System;
using ININ.InteractionClient.AddIn;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.Status;
using PlantronicsClientAddIn.Interactions;
using ININ.IceLib.Connection;
using System.Diagnostics;
using PlantronicsClientAddIn.Settings;

namespace PlantronicsClientAddIn
{
	public class AddIn : IAddIn
	{
        private static ITraceContext s_traceContext = null;
        private static PlantronicsManager s_plantronicsManager = null;
        private static IStatusManager s_statusManager = null;
        private static IInteractionManager s_interactionManager = null;
        private static INotificationService s_notificationService = null;
        private static Session _session = null;
        private static ISettingsManager s_settingsManager = null;
        private static IDeviceStatus s_deviceSettings = null;

        public static ISettingsManager SettingsManager
        {
            get
            {
                return s_settingsManager;
            }
        }

        public static IStatusManager StatusManager
        {
            get
            {
                return s_statusManager;
            }
        }

        public static IDeviceStatus DeviceSettings
        {
            get
            {
                return s_deviceSettings;
            }
        }



		public void Load (IServiceProvider serviceProvider)
		{
            try
            {
                s_traceContext = (ITraceContext)serviceProvider.GetService(typeof(ITraceContext));

                //must have the icelib sdk license to get the session as a service
                _session = (Session)serviceProvider.GetService(typeof(Session));
                s_interactionManager = new InteractionManager(_session, (IQueueService)serviceProvider.GetService(typeof(IQueueService)), s_traceContext);
                s_statusManager = new StatusManager(_session, s_traceContext);
                s_notificationService = (INotificationService)serviceProvider.GetService(typeof(INotificationService));

                s_settingsManager = new SettingsManager();
                s_deviceSettings = new DeviceStatus(s_traceContext);

                s_plantronicsManager = new PlantronicsManager(s_statusManager, s_interactionManager, s_notificationService, s_settingsManager, s_deviceSettings, s_traceContext);
                s_traceContext.Always("Plantronics AddIn Loaded");
            }
            catch (ArgumentNullException)
            {
                Debug.Fail("unable to get service.  Is the ICELIB SDK licence available?");
                throw;
            }
		}

		public void Unload ()
		{
            s_plantronicsManager.Dispose();
            s_plantronicsManager = null;
		}


    }
}

