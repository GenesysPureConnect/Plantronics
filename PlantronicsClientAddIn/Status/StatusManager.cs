using ININ.IceLib.Connection;
using ININ.IceLib.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Status
{
    internal class StatusManager : IStatusManager
    {
        const string AwayFromDesk = "away from desk";
        const string Available = "available";

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
            //var previousStatusInfo = _userStatusList.GetPreviousStatusInfo().Take(0);
            //if (previousStatusInfo != null && previousStatusInfo.Count() >= 0)
            //{
            //    var previousStatus = previousStatusInfo.fr ;
            //    SetStatus(previousStatus.StatusId.ToLower());
            //}
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
