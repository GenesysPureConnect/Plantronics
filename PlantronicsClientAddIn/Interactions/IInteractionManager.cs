using ININ.InteractionClient.AddIn;
using System;

namespace PlantronicsClientAddIn.Interactions
{
	public interface IInteractionManager
	{
		void PickupOrHoldCall(string callId);
        void HoldCall(string callId);
        void PickupCall(string callId);
        void DisconnectCall(string callid);
        void MuteInteraction(string callId, bool muteOn);
	}
}

