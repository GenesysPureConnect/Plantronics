using System;
using System.Collections.Generic;

namespace PlantronicsClientAddIn.Status
{
	public interface IStatusManager
	{
		void SetToAwayFromDesk();
		void SetToAvailable();
		void SetLastStatus();
        void SetStatus(string key);
        IList<Status> GetSettableStatuses();
	}
}

