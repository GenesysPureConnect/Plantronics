using System;
using ININ.InteractionClient.AddIn;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.Status;
using PlantronicsClientAddIn.Interactions;
using ININ.IceLib.Connection;

namespace PlantronicsClientAddIn
{
	public class AddIn : IAddIn
	{
        ITraceContext _traceContext = null;
        PlantronicsManager _plantronicsManager = null;
        IStatusManager _statusManager = null;
        IInteractionManager _interactionManager = null;
        INotificationService _notificationService = null;

        Session _session = null;
		
		//ICallService _callService;
		
        public AddIn ()
		{
		}

		public void Load (IServiceProvider serviceProvider)
		{
			_traceContext = (ITraceContext)serviceProvider.GetService(typeof(ITraceContext));
            
            //must have the icelib sdk license to get the session as a service
            _session = (Session)serviceProvider.GetService(typeof(Session));
            _interactionManager = new InteractionManager(_session, (IQueueService)serviceProvider.GetService(typeof(IQueueService)));
            _statusManager = new StatusManager(_session);
			_notificationService = (INotificationService)serviceProvider.GetService (typeof(INotificationService));

            _plantronicsManager = new PlantronicsManager(_statusManager, _interactionManager, _notificationService, _traceContext);
			_traceContext.Always ("Plantronics AddIn Loaded");
		}

		public void Unload ()
		{
            _plantronicsManager.Dispose();
            _plantronicsManager = null;
		}
    }
}

