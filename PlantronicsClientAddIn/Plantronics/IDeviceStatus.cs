using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interop.Plantronics;

namespace PlantronicsClientAddIn.Plantronics
{
    public interface IDeviceStatus
    {
        event EventHandler SettingsChanged;

         String InternalName{get;}
         String ManufacturerName{get;}
         String ProductName{get;}
         ushort VersionNumber{get;}
         String SerialNumber{get;}
         bool IsConnected { get; }


         void TraceSettings();
         void DeviceConnected(ICOMDevice device);
         void DeviceDisconnected();
            
    }
}
