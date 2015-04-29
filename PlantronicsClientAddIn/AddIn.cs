using System;
using ININ.InteractionClient.AddIn;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.Status;
using PlantronicsClientAddIn.Interactions;
using ININ.IceLib.Connection;
using System.Diagnostics;
using PlantronicsClientAddIn.Settings;
using PlantronicsClientAddIn.Connection;

namespace PlantronicsClientAddIn
{
	public class AddIn : IAddIn
	{
        private static ITraceContext s_traceContext = null;
        private static ICicStatusService s_statusManager = null;
        private static IInteractionManager s_interactionManager = null;
        private static INotificationService s_notificationService = null;
        private static Session s_session = null;
        private static ISettingsManager s_settingsManager = null;
        private static IDeviceManager s_deviceManager = null;
        private static StatusChanger s_statusChanger = null;
        private static NotificationServer s_notificationServer = null;
        private static InteractionSyncManager s_hookSwitchManager = null;
        private static IConnection s_connection;

        private static OutboundEventNotificationService s_outboundEventNotificationService = null;

        public static ISettingsManager SettingsManager
        {
            get
            {
                return s_settingsManager;
            }
        }

        public static ICicStatusService StatusManager
        {
            get
            {
                return s_statusManager;
            }
        }

        public static IDeviceManager DeviceSettings
        {
            get
            {
                return s_deviceManager;
            }
        }

		public void Load (IServiceProvider serviceProvider)
		{
            try
            {
                s_traceContext = (ITraceContext)serviceProvider.GetService(typeof(ITraceContext));

                //must have the icelib sdk license to get the session as a service
                s_session = (Session)serviceProvider.GetService(typeof(Session));
                s_connection = new Connection.Connection(s_session);
            }
            catch (ArgumentNullException)
            {
                s_traceContext.Error("unable to get Icelib Session, is the ICELIB SDK License available?");
                Debug.Fail("unable to get service.  Is the ICELIB SDK licence available?");
                throw;
            }

            s_interactionManager = new InteractionManager(s_session, (IQueueService)serviceProvider.GetService(typeof(IQueueService)), s_traceContext);
            s_statusManager = new CicStatusService(s_session, s_traceContext);
            s_notificationService = (INotificationService)serviceProvider.GetService(typeof(INotificationService));

            s_settingsManager = new SettingsManager();
            s_deviceManager = new DeviceManager(s_traceContext, new SpokesDebugLogger(s_traceContext));

            s_statusChanger = new StatusChanger(s_session, s_statusManager, s_deviceManager, s_settingsManager);
            s_notificationServer = new NotificationServer(s_deviceManager, s_settingsManager, s_notificationService);
       
            s_hookSwitchManager = new InteractionSyncManager(s_interactionManager, s_deviceManager, (IQueueService)serviceProvider.GetService(typeof(IQueueService)), s_traceContext, s_connection);

            s_outboundEventNotificationService = new OutboundEventNotificationService(s_session, s_statusManager, s_deviceManager, s_traceContext);

            s_traceContext.Always("Plantronics AddIn Loaded");
		}

		public void Unload ()
		{
            if (s_hookSwitchManager != null)
            {
                s_hookSwitchManager.Teardown();
            }

            s_deviceManager.Dispose();
            s_deviceManager = null;
		}


    }
}

