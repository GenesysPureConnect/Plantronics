using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using PlantronicsClientAddIn.Settings;
using PlantronicsClientAddIn.Status;

namespace PlantronicsClientAddIn.ViewModels
{
    public class SettingsViewModel 
    {
        private ISettingsManager _settingsManager;
        private IStatusManager _statusManager;
        private SynchronizationContext _synchronizationContext;

        public event PropertyChangedEventHandler PropertyChanged;
                
        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public SettingsViewModel() : this( AddIn.SettingsManager, AddIn.StatusManager)
        {
            
        }

        public SettingsViewModel(ISettingsManager settingsManager, IStatusManager statusManager)
        {
            if (settingsManager == null || statusManager == null)
            {
                return;
            }

            _statusManager = statusManager;
            _settingsManager = settingsManager;
            _synchronizationContext = SynchronizationContext.Current;

            var statusList = statusManager.GetSettableStatuses();
            
            StatusList = new ReadOnlyObservableCollection<Status.Status>(new ObservableCollection<Status.Status>(statusList));
            
            this._connectChangeStatus = _settingsManager.ConnectChangeStatus;
            this._connectNotification = _settingsManager.ConnectNotification;
            var connectStatus = statusList.FirstOrDefault(s => s.Key.ToLower() == _settingsManager.ConnectStatusKey.ToLower());
            if (connectStatus != null)
            {
                this._connectStatus = connectStatus;
            }

            this._disconnectChangeStatus = _settingsManager.DisconnectChangeStatus;
            this._disconnectNotification = _settingsManager.DisconnectNotification;

            var disconnectStatus = statusList.FirstOrDefault(s => s.Key.ToLower() == _settingsManager.DisconnectStatusKey.ToLower());
            if (disconnectStatus != null)
            {
                this._disconnectStatus = disconnectStatus;
            }
            
            this._outOfRangeChangeStatus = _settingsManager.OutOfRangeChangeStatus;
            this._outOfRangeNotification = _settingsManager.OutOfRangeNotification;
            var outOfRangeStatus = statusList.FirstOrDefault(s => s.Key.ToLower() == _settingsManager.OutOfRangeStatusKey.ToLower());
            if (outOfRangeStatus != null)
            {
                this._outOfRangeStatus = outOfRangeStatus;
            }

            this._inRangeChangeStatus = _settingsManager.InRangeChangeStatus;
            this._inRangeNotification = _settingsManager.InRangeNotification;
            var inRangeStatus = statusList.FirstOrDefault(s => s.Key.ToLower() == _settingsManager.InRangeStatusKey.ToLower());
            if (inRangeStatus != null)
            {
                this._inRangeStatus = inRangeStatus;
            }


        }

        private ReadOnlyObservableCollection<Status.Status> _statusList;
        public ReadOnlyObservableCollection<Status.Status> StatusList
        {
            get
            {
                return _statusList;
            }
            set
            {
                _statusList = value;
            }
        }

        private Status.Status _disconnectStatus;
        public Status.Status DisconnectStatus
        {
            get
            {
                return _disconnectStatus;
            }
            set
            {
                _settingsManager.DisconnectStatusKey = value.Key;
                _disconnectStatus = value;
                RaisePropertyChanged("DisconnectStatus");
            }
        }

        private bool _disconnectChangeStatus;
        public bool DisconnectChangeStatus
        {
            get
            {
                return _disconnectChangeStatus;
            }
            set
            {
                _settingsManager.DisconnectChangeStatus = value;
                _disconnectChangeStatus = value;
                RaisePropertyChanged("DisconnectChangeStatus");
            }
        }

        private bool _disconnectNotification;
        public bool DisconnectNotification
        {
            get
            {
                return _disconnectNotification;
            }
            set
            {
                _settingsManager.DisconnectNotification = value;
                _disconnectNotification = value;
                RaisePropertyChanged("DisconnectNotification");
            }
        }

        private Status.Status _connectStatus;
        public Status.Status ConnectStatus
        {
            get
            {
                return _connectStatus;
            }
            set
            {
                _settingsManager.ConnectStatusKey = value.Key;
                _connectStatus = value;
                RaisePropertyChanged("ConnectStatus");
            }
        }

        private bool _connectChangeStatus;
        public bool ConnectChangeStatus
        {
            get
            {
                return _connectChangeStatus;
            }
            set
            {
                _settingsManager.ConnectChangeStatus = value;
                _connectChangeStatus = value;
                RaisePropertyChanged("ConnectChangeStatus");
            }
        }

        private bool _connectNotification;
        public bool ConnectNotification
        {
            get
            {
                return _connectNotification;
            }
            set
            {
                _settingsManager.ConnectNotification = value;
                _connectNotification = value;
                RaisePropertyChanged("ConnectNotification");
            }
        }

        private Status.Status _outOfRangeStatus;
        public Status.Status OutOfRangeStatus
        {
            get
            {
                return _outOfRangeStatus;
            }
            set
            {
                _settingsManager.OutOfRangeStatusKey = value.Key;
                _outOfRangeStatus = value;
                RaisePropertyChanged("OutOfRangeStatus");
            }
        }

        private bool _outOfRangeChangeStatus;
        public bool OutOfRangeChangeStatus
        {
            get
            {
                return _outOfRangeChangeStatus;
            }
            set
            {
                _settingsManager.OutOfRangeChangeStatus = value;
                _outOfRangeChangeStatus = value;
                RaisePropertyChanged("OutOfRangeChangeStatus");
            }
        }

        private bool _outOfRangeNotification;
        public bool OutOfRangeNotification
        {
            get
            {
                return _outOfRangeNotification;
            }
            set
            {
                _settingsManager.OutOfRangeNotification = value;
                _outOfRangeNotification = value;
                RaisePropertyChanged("OutOfRangeNotification");
            }
        }

        private Status.Status _inRangeStatus;
        public Status.Status InRangeStatus
        {
            get
            {
                return _inRangeStatus;
            }
            set
            {
                _settingsManager.InRangeStatusKey = value.Key;
                _inRangeStatus = value;
                RaisePropertyChanged("InRangeStatus");
            }
        }

        private bool _inRangeChangeStatus;
        public bool InRangeChangeStatus
        {
            get
            {
                return _inRangeChangeStatus;
            }
            set
            {
                _settingsManager.InRangeChangeStatus = value;
                _inRangeChangeStatus = value;
                RaisePropertyChanged("InRangeChangeStatus");
            }
        }

        private bool _inRangeNotification;
        public bool InRangeNotification
        {
            get
            {
                return _inRangeNotification;
            }
            set
            {
                _settingsManager.InRangeNotification = value;
                _inRangeNotification = value;
                RaisePropertyChanged("InRangeNotification");
            }
        }




    }
}
