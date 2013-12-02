using ININ.IceLib.Connection;
using ININ.IceLib.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Status
{
    /// <summary>
    /// StatusManager uses IceLib to set the users's status to either available or away from desk.  
    /// </summary>
    internal class StatusManager : IStatusManager
    {
        private const string AwayFromDesk = "away from desk";
        private const string Available = "available";

        private readonly PeopleManager _peopleManager;
        private readonly UserStatusList _userStatusList;
        private readonly StatusMessageList _statusMessageList;
        private readonly string _userId;

        public StatusManager(Session session)
        {
            _userId = session.UserId;
            _peopleManager = PeopleManager.GetInstance(session);
            _userStatusList = new UserStatusList(_peopleManager);
            _userStatusList.StartWatching(new[] { _userId });

            _statusMessageList = new StatusMessageList(_peopleManager);
            _statusMessageList.StartWatching();
        }


        public void SetToAwayFromDesk()
        {
            SetStatus(AwayFromDesk);
        }

        public void SetToAvailable()
        {
            SetStatus(Available);
        }

        public void SetLastStatus()
        {
            SetToAvailable();
        }

        private void SetStatus(string statusId)
        {
            var statusDetails = _statusMessageList.GetList().FirstOrDefault(s => s.Id.ToLower() == statusId);
            if (statusDetails != null)
            {
                UserStatusUpdate statusUpdate = new UserStatusUpdate(_peopleManager);
                statusUpdate.UserId = _userId;
                statusUpdate.StatusMessageDetails = statusDetails;
                statusUpdate.UpdateRequest();
            }
        }
    }
}
