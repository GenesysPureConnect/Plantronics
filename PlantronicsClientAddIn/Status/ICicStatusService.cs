using ININ.IceLib.People;
using System;
using System.Collections.Generic;

namespace PlantronicsClientAddIn.Status
{
	public interface ICicStatusService
	{
        event EventHandler UserStatusChanged;
		void SetToAwayFromDesk();
		void SetToAvailable();
		void SetLastStatus();
        void SetStatus(string key);
        IList<Status> GetSettableStatuses();
        UserStatus GetStatus();
	}
}

