using ININ.IceLib.Connection;
using ININ.IceLib.Interactions;
using ININ.InteractionClient.AddIn;
using System;
using System.Collections.Generic;

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

        public void PickupOrHoldCall(string callId)
        {
            _traceContext.Status(String.Format("InteractionManager.PickupOrHoldCall {0}", callId));
            try
            {
                var interaction = GetInteraction(callId);

                if (interaction != null)
                {
                    if (interaction.IsConnected)
                    {
                        interaction.Hold(true);
                    }
                    else
                    {
                        interaction.Pickup();
                    }
                }
            }
            catch { }
        }

        public void MuteInteraction(string callId, bool muteOn)
        {
            _traceContext.Status(String.Format("InteractionManager.MuteInteraction {0}", callId));
            try
            {
                var interaction = GetInteraction(callId);

                if (interaction != null)
                {
                    interaction.Mute(muteOn);
                }
            }
            catch { }
            
        }

        public void PickupCall(string callId)
        {
            _traceContext.Status(String.Format("InteractionManager.PickupCall {0}", callId));
            try
            {
                var interaction = GetInteraction(callId);

                if (interaction != null)
                {
                    interaction.Pickup();
                }
            }
            catch { }
        }

        public void DisconnectCall(string callId)
        {
            _traceContext.Status(String.Format("InteractionManager.Disconnect {0}", callId));
            try
            {
                var interaction = GetInteraction(callId);

                if (interaction != null)
                {
                    interaction.Disconnect();
                }
            }
            catch { }
        }

        public void HoldCall(string callId)
        {
            _traceContext.Status(String.Format("InteractionManager.HoldCall {0}", callId));
           
            try
            {
                var interaction = GetInteraction(callId);

                if (interaction != null)
                {
                    interaction.Hold(true);
                }
            }
            catch { }
        }
        /*
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

        private IList<Interaction> FindAllCalls(string state)
        {
            List<Interaction> calls = new List<Interaction>();
            foreach (var interaction in _myInteractionsQueue.Interactions)
            {
                if (interaction.GetAttribute(InteractionAttributeName.State) == state &&
                    interaction.GetAttribute(InteractionAttributeName.InteractionType) == InteractionAttributeValues.InteractionType.Call)
                {
                    _traceContext.Note(String.Format("Found {0} with state {1}", interaction.InteractionId, state));
                    calls.Add(_interactionManager.CreateInteraction(new InteractionId(interaction.InteractionId)));
                }
            }

            return calls;
        }
        */
        private Interaction GetInteraction(string callId)
        {
            return _interactionManager.CreateInteraction(new InteractionId(callId));
        }
    }
}
