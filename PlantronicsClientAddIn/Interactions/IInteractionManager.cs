using System;

namespace PlantronicsClientAddIn.Interactions
{
	public interface IInteractionManager
	{
		void PickupOrDisconnectCall();
        void PickupAlertingCall();
        void HoldCall();
        void DisconnectCall();
        void PickupHeldCall();
	}
}

