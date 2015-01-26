using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace PlantronicsClientAddIn.Settings
{
    public class SettingsManager : ISettingsManager
    {
        public string FilePath{get; private set;}
        protected XmlDocument _xmlSettings;

        public const string AwayFromDeskKey = "away from desk";
        public const string AvailableKey = "available";

        public SettingsManager()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FilePath = Path.Combine(assemblyFolder, "PlantronicsAddInSettings.xml");

            _xmlSettings = new XmlDocument();

            if (File.Exists(FilePath))
            {
                _xmlSettings.Load(FilePath);
            }
            else
            {
                _xmlSettings.LoadXml("<settings/>");
            }
        }


        public string DeviceDisconnectStatusKey
        {
            get
            {
                return GetStringSetting("DeviceDisconnectStatusKey", AwayFromDeskKey);
            }
            set
            {
                SaveSetting("DeviceDisconnectStatusKey", value);
            }
        }

        public bool DeviceDisconnectChangeStatus
        {
            get
            {
                return GetBoolSetting("DeviceDisconnectChangeStatus", true);
            }
            set
            {
                SaveSetting("DeviceDisconnectChangeStatus", value);
            }
        }

        public bool DeviceDisconnectNotification
        {
            get
            {
                return GetBoolSetting("DeviceDisconnectNotification", true);
            }
            set
            {
                SaveSetting("DeviceDisconnectNotification", value);
            }
        }

        public bool ShouldLogOutOnDeviceDisconnect
        {
            get
            {
                return GetBoolSetting("ShouldLogOutOnDeviceDisconnect", true);
            }
            set
            {
                SaveSetting("ShouldLogOutOnDeviceDisconnect", value);
            }
        }

        public string DeviceConnectStatusKey
        {
            get
            {
                return GetStringSetting("DeviceConnectStatusKey", AwayFromDeskKey);
            }
            set
            {
                SaveSetting("DeviceConnectStatusKey", value);
            }
        }

        public bool DeviceConnectChangeStatus
        {
            get
            {
                return GetBoolSetting("DeviceConnectChangeStatus", true);
            }
            set
            {
                SaveSetting("DeviceConnectChangeStatus", value);
            }
        }

        public bool DeviceConnectNotification
        {
            get
            {
                return GetBoolSetting("DeviceConnectNotification", true);
            }
            set
            {
                SaveSetting("DeviceConnectNotification", value);
            }
        }

        public string HeadsetDisconnectStatusKey
        {
            get
            {
                return GetStringSetting("HeadsetDisconnectStatusKey", AwayFromDeskKey);
            }
            set
            {
                SaveSetting("HeadsetDisconnectStatusKey", value);
            }
        }

        public bool HeadsetDisconnectChangeStatus
        {
            get
            {
                return GetBoolSetting("HeadsetDisconnectChangeStatus", true);
            }
            set
            {
                SaveSetting("HeadsetDisconnectChangeStatus", value);
            }
        }

        public bool HeadsetDisconnectNotification
        {
            get
            {
                return GetBoolSetting("HeadsetDisconnectNotification", true);
            }
            set
            {
                SaveSetting("HeadsetDisconnectNotification", value);
            }
        }

        public bool ShouldLogOutOnHeadsetDisconnect
        {
            get
            {
                return GetBoolSetting("ShouldLogOutOnHeadsetDisconnect", true);
            }
            set
            {
                SaveSetting("ShouldLogOutOnHeadsetDisconnect", value);
            }
        }

        public string HeadsetConnectStatusKey
        {
            get
            {
                return GetStringSetting("HeadsetConnectStatusKey", AwayFromDeskKey);
            }
            set
            {
                SaveSetting("HeadsetConnectStatusKey", value);
            }
        }

        public bool HeadsetConnectChangeStatus
        {
            get
            {
                return GetBoolSetting("HeadsetConnectChangeStatus", true);
            }
            set
            {
                SaveSetting("HeadsetConnectChangeStatus", value);
            }
        }

        public bool HeadsetConnectNotification
        {
            get
            {
                return GetBoolSetting("HeadsetConnectNotification", true);
            }
            set
            {
                SaveSetting("HeadsetConnectNotification", value);
            }
        }

        
        private bool GetBoolSetting(string key, bool defaultValue){
            var elements = _xmlSettings.GetElementsByTagName(key);
            if (elements == null || elements.Count == 0)
            {
                return defaultValue;    
            }

            bool result = true;

            Boolean.TryParse(elements[0].InnerText, out result);

            return result;
        }

        private string GetStringSetting(string key, string defaultValue)
        {
            var elements = _xmlSettings.GetElementsByTagName(key);
            if (elements == null || elements.Count ==0 )
            {
                return defaultValue;
            }

            return elements[0].InnerText;
        }

        private void SaveSetting(string key, object value){
            var elements = _xmlSettings.GetElementsByTagName(key);
            XmlNode element = null;
            if (elements == null || elements.Count == 0)
            {
                 element = _xmlSettings.CreateElement(key);
                _xmlSettings.DocumentElement.AppendChild(element);
            }
            else
            {
                element = elements[0];
            }
            element.InnerText = value.ToString();
            _xmlSettings.Save(FilePath);

            
        }
    }
}
