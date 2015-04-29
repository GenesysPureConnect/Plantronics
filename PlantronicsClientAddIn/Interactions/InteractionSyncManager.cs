using ININ.InteractionClient.AddIn;
using PlantronicsClientAddIn.Connection;
using PlantronicsClientAddIn.Plantronics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Interactions
{
    public class InteractionSyncManager
    {
        /// <summary>
        /// Internal class used to keep track of interaction state. 
        /// </summary>
        private class InteractionState
        {
            public string State { get; set; }
            public bool IsMuted { get; set; }

            public InteractionState(IInteraction interaction)
            {
                State = interaction.GetAttribute(InteractionAttributes.State);
                IsMuted = interaction.GetAttribute(InteractionAttributes.Muted) == "1";
            }
        }

        private DateTime _lastSwitchChange = new DateTime();
        private IInteractionManager _interactionManager = null;
        private IDeviceManager _deviceManager = null;
        private IQueue _myInteractions = null;
        private ITraceContext _traceContext = null;

        private Dictionary<string, InteractionState> _interactions = new Dictionary<string, InteractionState>();
        private IConnection _connection;

        public InteractionSyncManager(IInteractionManager interactionManager, IDeviceManager deviceManager, IQueueService queueService, ITraceContext traceContext, IConnection connection)
        {
            _interactionManager = interactionManager;
            _deviceManager = deviceManager;
            _traceContext = traceContext;
            
            _connection = connection;
            _connection.UpChanged += OnConnectionUpChanged;


            _deviceManager.CallAnsweredByDevice += OnCallAnsweredByDevice;
            _deviceManager.CallEndedByDevice += OnCallEndedByDevice;
            _deviceManager.MuteChanged += OnMuteChangedByDevice;
            _deviceManager.OnCall += OnOnCall;
        //    _deviceManager.TalkButtonPressed += OnTalkButtonPressed;
            _deviceManager.TalkButtonHeld += OnTalkButtonHeld;

            _myInteractions = queueService.GetMyInteractions(new[] { InteractionAttributes.State, InteractionAttributes.Muted });
            _myInteractions.InteractionAdded += OnInteractionAdded;
            _myInteractions.InteractionChanged += OnInteractionChanged;
            _myInteractions.InteractionRemoved += OnInteractionRemoved;
            
        }

        private void OnConnectionUpChanged(object sender, EventArgs e)
        {
            if (!_connection.IsConnected)
            {
                Teardown();
            }
        }

        public void Teardown()
        {
            foreach (var id in _interactions.Keys)
            {
                _deviceManager.CallEnded(id);
            }
        }

        private void OnOnCall(object sender, global::Plantronics.UC.SpokesWrapper.OnCallArgs e)
        {
            var callIdString = _interactions.Keys.FirstOrDefault((id) => id.Contains(e.CallId.ToString()));

            if (!String.IsNullOrEmpty(callIdString))
            {
              //  _deviceManager.CallEnded(e.CallId.ToString());
            }
        }

        private void OnMuteChangedByDevice(object sender, global::Plantronics.UC.SpokesWrapper.MuteChangedArgs e)
        {
            //var plantronicsIdString = plantronicsId.ToString();

           // var callIdString = _interactions.Keys.FirstOrDefault((id) => id.Contains(plantronicsIdString));
            try
            {
                _interactionManager.MuteInteraction(_deviceManager.CurrentCallId, e.m_muteon);
            }
            catch
            {
                //unable to do the mute call, reset the device
                _deviceManager.ToggleMute();
            }
        }

        private string GetInteractionIdFromPlantronicsId(int plantronicsId)
        {
            var plantronicsIdString = plantronicsId.ToString();

            var callIdString = _interactions.Keys.FirstOrDefault((id) => id.Contains(plantronicsIdString));

            return callIdString;
        }

        private void OnCallEndedByDevice(object sender, global::Plantronics.UC.SpokesWrapper.CallEndedArgs e)
        {
            _interactionManager.DisconnectCall(GetInteractionIdFromPlantronicsId(e.CallId));
        }

        private void OnCallAnsweredByDevice(object sender, global::Plantronics.UC.SpokesWrapper.CallAnsweredArgs e)
        {
            _deviceManager.CurrentCallId = GetInteractionIdFromPlantronicsId(e.CallId);
            _interactionManager.PickupCall(GetInteractionIdFromPlantronicsId(e.CallId));
            //_interactionManager.PickupOrHoldCall(GetInteractionIdFromPlantronicsId(e.CallId));
        }

        private void OnInteractionRemoved(object sender, InteractionEventArgs e)
        {
            if(_interactions.ContainsKey(e.Interaction.InteractionId))
            {
                var callDetails = _interactions[e.Interaction.InteractionId];

                if (callDetails.State != InteractionAttributeValues.State.ExternalDisconnect &&
                    callDetails.State != InteractionAttributeValues.State.InternalDisconnect)
                {
                    _deviceManager.CallEnded(e.Interaction.InteractionId);
                }

                _interactions.Remove(e.Interaction.InteractionId);
            }
        }

        private void OnInteractionChanged(object sender, InteractionEventArgs e)
        {
            var interaction = e.Interaction;

            if (interaction.GetAttribute(InteractionAttributes.InteractionType) != InteractionAttributeValues.InteractionType.Call)
            {
                return;
            }

            var oldInteractionState = _interactions[interaction.InteractionId];
            var newInteractionState = new InteractionState(interaction);

            if (oldInteractionState.State != newInteractionState.State)
            {
                if (newInteractionState.State == InteractionAttributeValues.State.ExternalDisconnect ||
                    newInteractionState.State == InteractionAttributeValues.State.InternalDisconnect)
                {
                    _deviceManager.CallEnded(interaction.InteractionId);
                }
                else if (oldInteractionState.State == InteractionAttributeValues.State.Held &&
                    newInteractionState.State == InteractionAttributeValues.State.Connected)
                {
                    _deviceManager.CallResumed(interaction.InteractionId);
                }
                else if (newInteractionState.State == InteractionAttributeValues.State.Held)
                {
                    _deviceManager.CallHeld(interaction.InteractionId);
                }
                else if (newInteractionState.State == InteractionAttributeValues.State.Connected &&
                    oldInteractionState.State == InteractionAttributeValues.State.Alerting)
                {
                    _deviceManager.CallAnswered(interaction.InteractionId);
                }
            }

            _interactions[interaction.InteractionId] = newInteractionState;
        }




        private void OnInteractionAdded(object sender, InteractionEventArgs e)
        {
            var interaction = e.Interaction;

            if (interaction.GetAttribute(InteractionAttributes.InteractionType) != InteractionAttributeValues.InteractionType.Call)
            {
                return;
            }

            if (interaction.GetAttribute(InteractionAttributes.State) ==
                    InteractionAttributeValues.State.ExternalDisconnect ||
                    interaction.GetAttribute(InteractionAttributes.State) ==
                    InteractionAttributeValues.State.InternalDisconnect)
            {
                return;
            }

            _interactions.Add(e.Interaction.InteractionId, new InteractionState(e.Interaction));
            
            if (interaction.GetAttribute(InteractionAttributes.Direction) == 
                InteractionAttributeValues.Direction.Outgoing)
            {
                _deviceManager.OutgoingCall(interaction.InteractionId);
            }
            else if (interaction.GetAttribute(InteractionAttributes.Direction) ==
                InteractionAttributeValues.Direction.Incoming 
                )
            {
                _deviceManager.IncomingCall(interaction.InteractionId);

                if (interaction.GetAttribute(InteractionAttributes.State) !=
                    InteractionAttributeValues.State.Alerting &&
                    interaction.GetAttribute(InteractionAttributes.State) !=
                    InteractionAttributeValues.State.Offering)
                {
                    _deviceManager.CallAnswered(interaction.InteractionId);
                }
            }
        }



        private void OnTalkButtonHeld(object sender, EventArgs e)
        {
            //PerformButtonAction(_interactionManager.HoldCall);

            _interactionManager.HoldCall(_deviceManager.CurrentCallId);
        }

        private void PerformButtonAction(Action buttonAction)
        {
            //Getting an issue where when we press the button, we get two events right
            //in a row, trying to prevent that by requiring a min time between presses
            if (DateTime.Now - _lastSwitchChange > TimeSpan.FromSeconds(1))
            {
                buttonAction.Invoke();
            }

            _lastSwitchChange = DateTime.Now;
        }

    }
}
