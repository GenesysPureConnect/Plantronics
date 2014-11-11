using System;
using ININ.InteractionClient.AddIn;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.Status;
using PlantronicsClientAddIn.Interactions;
using ININ.IceLib.Connection;
using System.Diagnostics;

namespace PlantronicsClientAddIn
{
	public class AddIn : IAddIn
	{
        private ITraceContext _traceContext = null;
        private PlantronicsManager _plantronicsManager = null;
        private IStatusManager _statusManager = null;
        private IInteractionManager _interactionManager = null;
        private INotificationService _notificationService = null;
        private Session _session = null;

		public void Load (IServiceProvider serviceProvider)
		{
            try
            {
                _traceContext = (ITraceContext)serviceProvider.GetService(typeof(ITraceContext));

                //must have the icelib sdk license to get the session as a service
                _session = (Session)serviceProvider.GetService(typeof(Session));
                _interactionManager = new InteractionManager(_session, (IQueueService)serviceProvider.GetService(typeof(IQueueService)), _traceContext);
                _statusManager = new StatusManager(_session, _traceContext);
                _notificationService = (INotificationService)serviceProvider.GetService(typeof(INotificationService));

                _plantronicsManager = new PlantronicsManager(_statusManager, _interactionManager, _notificationService, _traceContext);
                _traceContext.Always("Plantronics AddIn Loaded");
            }
            catch (ArgumentNullException)
            {
                Debug.Fail("unable to get service.  Is the ICELIB SDK licence available?");
                throw;
            }
		}

		public void Unload ()
		{
            _plantronicsManager.Dispose();
            _plantronicsManager = null;
		}
    }
}

