using ININ.IceLib.Connection;
using ININ.IceLib.People;
using ININ.InteractionClient.AddIn;
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
        private readonly ITraceContext _traceContext;
        private readonly string _userId;

        public StatusManager(Session session, ITraceContext traceContext)
        {
            _userId = session.UserId;
            _traceContext = traceContext;
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
            _traceContext.Status("Setting status to " + statusId);
            var statusDetails = _statusMessageList.GetList().FirstOrDefault(s => s.Id.ToLower() == statusId);
            if (statusDetails != null)
            {
                UserStatusUpdate statusUpdate = new UserStatusUpdate(_peopleManager);
                statusUpdate.UserId = _userId;
                statusUpdate.StatusMessageDetails = statusDetails;
                _traceContext.Status("Sending status update request");
                statusUpdate.UpdateRequest();
            }
            else
            {
                _traceContext.Warning("status message not found");
                _statusMessageList.GetList().ToList().ForEach(s => _traceContext.Status("Status: " + s.Id));
            }
        }
    }
}
