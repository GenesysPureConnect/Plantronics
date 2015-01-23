using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using PlantronicsClientAddIn.Plantronics;

namespace PlantronicsClientAddIn.ViewModels
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private IDeviceManager _deviceManager;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public StatusViewModel(IDeviceManager deviceSettings)
        {
            _deviceManager = deviceSettings;
            UpdateValues();
            _deviceManager.HeadsetConnected +=OnSettingsChanged;
            _deviceManager.HeadsetDisconnected += OnSettingsChanged;
            _deviceManager.MuteChanged += OnSettingsChanged;
            _deviceManager.PlantronicsDeviceAttached += OnSettingsChanged;
            _deviceManager.PlantronicsDeviceDetached += OnSettingsChanged;
        }

        public StatusViewModel() : this(AddIn.DeviceSettings)
        {

        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            InternalName = _deviceManager.InternalName;
            ManufacturerName = _deviceManager.ManufacturerName;
            ProductName = _deviceManager.ProductName;
            VersionNumber = _deviceManager.VersionNumber;

            SerialNumber = _deviceManager.SerialNumber;
            ConnectionVisibility = _deviceManager.IsDeviceConnected ? Visibility.Visible: Visibility.Hidden;
            ErrorVisibility = !_deviceManager.IsDeviceConnected ? Visibility.Visible : Visibility.Hidden;
            MuteVisibility = _deviceManager.IsHeadsetMuted ? Visibility.Visible : Visibility.Hidden;
        }

        

        private string _internalName;
        public string InternalName
        {
            get
            {
                return _internalName;
            }
            set
            {
                _internalName = value;
                RaisePropertyChanged("InternalName");
            }
        }

        private string _manufacturerName;
        public string ManufacturerName
        {
            get
            {
                return _manufacturerName;
            }
            set
            {
                _manufacturerName = value;
                RaisePropertyChanged("ManufacturerName");
            }
        }

        private string _productName;
        public string ProductName
        {
            get
            {
                return _productName;
            }
            set
            {
                _productName = value;
                RaisePropertyChanged("ProductName");
            }
        }

        private ushort _versionNumber;
        public ushort VersionNumber
        {
            get
            {
                return _versionNumber;
            }
            set
            {
                _versionNumber = value;
                RaisePropertyChanged("VersionNumber");
            }
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }
            set
            {
                _serialNumber = value;
                RaisePropertyChanged("SerialNumber");
            }
        }

        private Visibility _connectionVisibility;
        public Visibility ConnectionVisibility
        {
            get
            {
                return _connectionVisibility;
            }
            set
            {
                _connectionVisibility = value;
                RaisePropertyChanged("ConnectionVisibility");
            }
        }

        private Visibility _errorVisibility;
        public Visibility ErrorVisibility
        {
            get
            {
                return _errorVisibility;
            }
            set
            {
                _errorVisibility = value;
                RaisePropertyChanged("ErrorVisibility");
            }
        }

        private Visibility _muteVisibility;
        public Visibility MuteVisibility
        {
            get
            {
                return _muteVisibility;
            }
            set
            {
                _muteVisibility = value;
                RaisePropertyChanged("MuteVisibility");
            }
        }
    }
}
