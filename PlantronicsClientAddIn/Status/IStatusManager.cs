using System;

namespace PlantronicsClientAddIn.Status
{
	public interface IStatusManager
	{
		void SetToAwayFromDesk();
		void SetToAvailable();
		void SetLastStatus();
	}
}

