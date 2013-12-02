using ININ.IceLib.Connection;
using ININ.IceLib.Interactions;
using ININ.InteractionClient.AddIn;
using System;

namespace PlantronicsClientAddIn.Interactions
{
    /// <summary>
    /// The InteractionManager class is responsible for picking up or disconnecting calls based on the state that they are in. 
    /// </summary>
    internal class InteractionManager : IInteractionManager
    {
        private readonly InteractionsManager _interactionManager;
        private readonly IQueueService _queueService;
        private readonly IQueue _myInteractionsQueue;
        private readonly ITraceContext _traceContext;

        public InteractionManager(Session session, IQueueService queueService, ITraceContext traceContext)
        {
            _traceContext = traceContext;
            _interactionManager = InteractionsManager.GetInstance(session);
            _queueService = queueService;

            //We could use icelib to get the queue and interactions, but the AddIn API wraps things up to be a little simpler to use. 
            _myInteractionsQueue = _queueService.GetMyInteractions(new[] { InteractionAttributes.State });
        }

        public void PickupOrDisconnectCall()
        {
            _traceContext.Status("looking for a call that is alerting or connected");
            Interaction call = null;

            //first try to find an alerting CALL, and pick that up
            call = FindCall(InteractionAttributeValues.State.Alerting);
            if (call != null)
            {
                call.Pickup();
                return; //don't do anything else here
            }
            
            //if there is not an alerting call, Disconnect a connected Call;
            call = FindCall(InteractionAttributeValues.State.Connected);
            if (call != null)
            {
                call.Disconnect();
            }
        }

        private Interaction FindCall(string state)
        {
            foreach (var interaction in _myInteractionsQueue.Interactions)
            {
                if (interaction.GetAttribute(InteractionAttributeName.State) == state && 
                    interaction.GetAttribute(InteractionAttributeName.InteractionType) == InteractionAttributeValues.InteractionType.Call)
                {
                    _traceContext.Note(String.Format("Found {0} with state {1}", interaction.InteractionId, state));
                    return _interactionManager.CreateInteraction(new InteractionId(interaction.InteractionId));
                }
            }

            _traceContext.Note(String.Format("Could not find interaction with state {0}", state));
            return null;
        }
    }
}
