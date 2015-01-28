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
        private ICicStatusService _statusManager;
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

        public SettingsViewModel(ISettingsManager settingsManager, ICicStatusService statusManager)
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
            
            this._deviceConnectChangeStatus = _settingsManager.DeviceConnectChangeStatus;
            this._deviceConnectNotification = _settingsManager.DeviceConnectNotification;
            var connectStatus = statusList.FirstOrDefault(s => s.Key.ToLower() == _settingsManager.DeviceConnectStatusKey.ToLower());
            if (connectStatus != null)
            {
                this._deviceConnectStatus = connectStatus;
            }

            this._deviceDisconnectChangeStatus = _settingsManager.DeviceDisconnectChangeStatus;
            this._deviceDisconnectNotification = _settingsManager.DeviceDisconnectNotification;

            var disconnectStatus = statusList.FirstOrDefault(s => s.Key.ToLower() == _settingsManager.DeviceDisconnectStatusKey.ToLower());
            if (disconnectStatus != null)
            {
                this._deviceDisconnectStatus = disconnectStatus;
            }
            this._shouldLogOutOnDeviceDisconnect = _settingsManager.ShouldLogOutOnDeviceDisconnect;
            
            this._headsetDisconnectChangeStatus = _settingsManager.HeadsetDisconnectChangeStatus;
            this._headsetDisconnectNotification = _settingsManager.HeadsetDisconnectNotification;
            var headsetDisconnectStatus = statusList.FirstOrDefault(s => s.Key.ToLower() == _settingsManager.HeadsetDisconnectStatusKey.ToLower());
            if (headsetDisconnectStatus != null)
            {
                this._headsetDisconnectStatus = headsetDisconnectStatus;
            }
            this._shouldLogOutOnHeadsetDisconnect = _settingsManager.ShouldLogOutOnHeadsetDisconnect;

            this._headsetConnectChangeStatus = _settingsManager.HeadsetConnectChangeStatus;
            this._headsetConnectNotification = _settingsManager.HeadsetConnectNotification;
            var headsetConnectedStatus = statusList.FirstOrDefault(s => s.Key.ToLower() == _settingsManager.HeadsetConnectStatusKey.ToLower());
            if (headsetConnectedStatus != null)
            {
                this._headsetConnectStatus = headsetConnectedStatus;
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

        private Status.Status _deviceDisconnectStatus;
        public Status.Status DeviceDisconnectStatus
        {
            get
            {
                return _deviceDisconnectStatus;
            }
            set
            {
                _settingsManager.DeviceDisconnectStatusKey = value.Key;
                _deviceDisconnectStatus = value;
                RaisePropertyChanged("DeviceDisconnectStatus");
            }
        }

        private bool _deviceDisconnectChangeStatus;
        public bool DeviceDisconnectChangeStatus
        {
            get
            {
                return _deviceDisconnectChangeStatus;
            }
            set
            {
                _settingsManager.DeviceDisconnectChangeStatus = value;
                _deviceDisconnectChangeStatus = value;
                RaisePropertyChanged("DeviceDisconnectChangeStatus");
            }
        }

        private bool _deviceDisconnectNotification;
        public bool DeviceDisconnectNotification
        {
            get
            {
                return _deviceDisconnectNotification;
            }
            set
            {
                _settingsManager.DeviceDisconnectNotification = value;
                _deviceDisconnectNotification = value;
                RaisePropertyChanged("DeviceDisconnectNotification");
            }
        }

        private bool _shouldLogOutOnDeviceDisconnect;
        public bool ShouldLogOutOnDeviceDisconnect
        {
            get
            {
                return _shouldLogOutOnDeviceDisconnect;
            }
            set
            {
                _settingsManager.ShouldLogOutOnDeviceDisconnect = value;
                _shouldLogOutOnDeviceDisconnect = value;
                RaisePropertyChanged("ShouldLogOutOnDeviceDisconnect");
            }
        }

        private Status.Status _deviceConnectStatus;
        public Status.Status DeviceConnectStatus
        {
            get
            {
                return _deviceConnectStatus;
            }
            set
            {
                _settingsManager.DeviceConnectStatusKey = value.Key;
                _deviceConnectStatus = value;
                RaisePropertyChanged("DeviceConnectStatus");
            }
        }

        private bool _deviceConnectChangeStatus;
        public bool DeviceConnectChangeStatus
        {
            get
            {
                return _deviceConnectChangeStatus;
            }
            set
            {
                _settingsManager.DeviceConnectChangeStatus = value;
                _deviceConnectChangeStatus = value;
                RaisePropertyChanged("DeviceConnectChangeStatus");
            }
        }

        private bool _deviceConnectNotification;
        public bool DeviceConnectNotification
        {
            get
            {
                return _deviceConnectNotification;
            }
            set
            {
                _settingsManager.DeviceConnectNotification = value;
                _deviceConnectNotification = value;
                RaisePropertyChanged("DeviceConnectNotification");
            }
        }

        private Status.Status _headsetDisconnectStatus;
        public Status.Status HeadsetDisconnectStatus
        {
            get
            {
                return _headsetDisconnectStatus;
            }
            set
            {
                _settingsManager.HeadsetDisconnectStatusKey = value.Key;
                _headsetDisconnectStatus = HeadsetDisconnectStatus;
                RaisePropertyChanged("HeadsetDisconnectStatus");
            }
        }

        private bool _headsetDisconnectChangeStatus;
        public bool HeadsetDisconnectChangeStatus
        {
            get
            {
                return _headsetDisconnectChangeStatus;
            }
            set
            {
                _settingsManager.HeadsetDisconnectChangeStatus = value;
                _headsetDisconnectChangeStatus = value;
                RaisePropertyChanged("HeadsetDisconnectChangeStatus");
            }
        }

        private bool _headsetDisconnectNotification;
        public bool HeadsetDisconnectNotification
        {
            get
            {
                return _headsetDisconnectNotification;
            }
            set
            {
                _settingsManager.HeadsetDisconnectNotification = value;
                _headsetDisconnectNotification = value;
                RaisePropertyChanged("HeadsetDisconnectNotification");
            }
        }

        private bool _shouldLogOutOnHeadsetDisconnect;
        public bool ShouldLogOutOnHeadsetDisconnect
        {
            get
            {
                return _shouldLogOutOnHeadsetDisconnect;
            }
            set
            {
                _settingsManager.ShouldLogOutOnHeadsetDisconnect = value;
                _shouldLogOutOnHeadsetDisconnect = value;
                RaisePropertyChanged("ShouldLogOutOnHeadsetDisconnect");
            }
        }

        private Status.Status _headsetConnectStatus;
        public Status.Status HeadsetConnectStatus
        {
            get
            {
                return _headsetConnectStatus;
            }
            set
            {
                _settingsManager.HeadsetConnectStatusKey = value.Key;
                _headsetConnectStatus = value;
                RaisePropertyChanged("HeadsetConnectStatus");
            }
        }

        private bool _headsetConnectChangeStatus;
        public bool HeadsetConnectChangeStatus
        {
            get
            {
                return _headsetConnectChangeStatus;
            }
            set
            {
                _settingsManager.HeadsetConnectChangeStatus = value;
                _headsetConnectChangeStatus = value;
                RaisePropertyChanged("HeadsetConnectChangeStatus");
            }
        }

        private bool _headsetConnectNotification;
        public bool HeadsetConnectNotification
        {
            get
            {
                return _headsetConnectNotification;
            }
            set
            {
                _settingsManager.HeadsetConnectNotification = value;
                _headsetConnectNotification = value;
                RaisePropertyChanged("HeadsetConnectNotification");
            }
        }




    }
}
