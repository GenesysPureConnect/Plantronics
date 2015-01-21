using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ININ.InteractionClient.AddIn;
using Interop.Plantronics;

namespace PlantronicsClientAddIn.Plantronics
{
    public class DeviceStatus : IDeviceStatus
    {
        public event EventHandler SettingsChanged;
        private ITraceContext _traceContext;

        public DeviceStatus(ITraceContext traceContext)
        {
            _traceContext = traceContext;
            IsConnected = false;
        }

        public bool IsConnected { get; private set; }
        public string InternalName
        {
            get;
            private set;
        }

        public string ManufacturerName
        {
            get;
            private set;
        }

        public string ProductName
        {
            get;
            private set;
        }

        public ushort VersionNumber
        {
            get;
            private set;
        }

        public string SerialNumber
        {
            get;
            private set;
        }

        public void TraceSettings()
        {
            _traceContext.Status("Plantronics connected device information");
            _traceContext.Status("Plantronics: Internal Name- " + InternalName);
            _traceContext.Status("Plantronics: Manufacturer Name- " + ManufacturerName);
            _traceContext.Status("Plantronics: Product Name- " + ProductName);
            _traceContext.Status("Plantronics: Serial Number- " + SerialNumber);
            _traceContext.Status("Plantronics: Version Number- " + VersionNumber);
        }

        public void DeviceConnected(ICOMDevice device)
        {
            InternalName = device.InternalName;
            ManufacturerName = device.ManufacturerName;
            ProductName = device.ProductName;
            SerialNumber = device.SerialNumber;
            VersionNumber = device.VersionNumber;
            IsConnected = true;

            if (SettingsChanged != null)
            {
                SettingsChanged(this, EventArgs.Empty);
            }
        }

        public void DeviceDisconnected()
        {
            IsConnected = false;
            InternalName = String.Empty;
            ManufacturerName = String.Empty;
            ProductName = String.Empty;
            SerialNumber = String.Empty;
            VersionNumber = 0;

            if (SettingsChanged != null)
            {
                SettingsChanged(this, EventArgs.Empty);
            }
        }
    }
}
