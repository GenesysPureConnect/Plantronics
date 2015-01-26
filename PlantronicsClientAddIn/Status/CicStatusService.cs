using ININ.IceLib.Connection;
using ININ.IceLib.People;
using ININ.InteractionClient.AddIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace PlantronicsClientAddIn.Status
{
    /// <summary>
    /// StatusManager uses IceLib to set the users's status to either available or away from desk.  
    /// </summary>
    internal class CicStatusService : ICicStatusService
    {
        private const string AwayFromDesk = "away from desk";
        private const string Available = "available";

        private readonly PeopleManager _peopleManager;
        private readonly UserStatusList _userStatusList;
        private readonly StatusMessageList _statusMessageList;
        private readonly FilteredStatusMessageList _filteredStatusList;
 
        private readonly ITraceContext _traceContext;
        private readonly string _userId;

        public CicStatusService(Session session, ITraceContext traceContext)
        {
            _userId = session.UserId;
            _traceContext = traceContext;
            _peopleManager = PeopleManager.GetInstance(session);
            _userStatusList = new UserStatusList(_peopleManager);
            _userStatusList.StartWatching(new[] { _userId });

            _statusMessageList = new StatusMessageList(_peopleManager);
            _statusMessageList.StartWatching();

            _filteredStatusList = new FilteredStatusMessageList(_peopleManager);
            _filteredStatusList.StartWatching(new[] { _userId });
        }

        public IList<Status> GetSettableStatuses()
        {
            var statuses = new List<Status>();
            var icStatuses = _filteredStatusList.GetList();

            var icStatusList = icStatuses[_userId].Where((s, x) => s.IsSelectableStatus).OrderBy(status => status.MessageText);

            foreach (var icStatus in icStatusList)
            {
                if (!icStatus.IsSelectableStatus)
                {
                    continue;
                }

                Bitmap bitmap = icStatus.Icon.ToBitmap();
                IntPtr hBitmap = bitmap.GetHbitmap();

                ImageSource wpfBitmap =
                     Imaging.CreateBitmapSourceFromHBitmap(
                          hBitmap, IntPtr.Zero, Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());

                statuses.Add(new Status
                {
                    Key = icStatus.Id,
                    Text = icStatus.MessageText,
                    Image = wpfBitmap


                });
            }

            return statuses;
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

        public void SetStatus(string statusId)
        {
            try
            {
                _traceContext.Status("Setting status to " + statusId);
                var statusDetails = _statusMessageList.GetList().FirstOrDefault(s => s.Id.ToLower() == statusId.ToLower());
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
            catch (Exception ex)
            {
                _traceContext.Exception(ex, "Unable to set status");
            }
        }
    }
}
