using System;

namespace PlantronicsClientAddIn.Interactions
{
	public interface IInteractionManager
	{
		void PickupOrDisconnectCall();
        void HoldCall();
        void PickupHeldCall();
	}
}

