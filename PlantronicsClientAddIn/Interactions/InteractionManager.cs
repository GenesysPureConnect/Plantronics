using ININ.IceLib.Connection;
using ININ.IceLib.Interactions;
using ININ.InteractionClient.AddIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Interactions
{
    internal class InteractionManager : IInteractionManager
    {
        private readonly InteractionsManager _interactionManager;
        private readonly IQueueService _queueService;
        private readonly IQueue _myInteractionsQueue;

        public InteractionManager(Session session, IQueueService queueService)
        {
            _interactionManager = InteractionsManager.GetInstance(session);
            _queueService = queueService;

            //We could use icelib to get the queue and interactions, but the AddIn API wraps things up to be a little simpler to use. 
            _myInteractionsQueue = _queueService.GetMyInteractions(new[] { InteractionAttributes.State });
        }

        public void PickupOrDisconnectCall()
        {
            IInteraction call = null;

            //first try to find an alerting CALL, and pick that up
            call = _myInteractionsQueue.Interactions.FirstOrDefault(c => c.GetAttribute(InteractionAttributeName.State) == InteractionAttributeValues.State.Alerting);
            if (call != null)
            {
                _interactionManager.CreateInteraction(new InteractionId(call.InteractionId)).Pickup();
                return; //don't do anything else here
            }
            
            //if there is not an alerting call, Disconnect a connected Call;
            call = _myInteractionsQueue.Interactions.FirstOrDefault(c => c.GetAttribute(InteractionAttributeName.State) == InteractionAttributeValues.State.Connected);
            if (call != null)
            {
                _interactionManager.CreateInteraction(new InteractionId(call.InteractionId)).Disconnect();
            }
        }
    }
}
