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


        public string DisconnectStatusKey
        {
            get
            {
                return GetStringSetting("DisconnectStatusKey", AwayFromDeskKey);
            }
            set
            {
                SaveSetting("DisconnectStatusKey", value);
            }
        }

        public bool DisconnectChangeStatus
        {
            get
            {
                return GetBoolSetting("DisconnectChangeStatus", true);
            }
            set
            {
                SaveSetting("DisconnectChangeStatus", value);
            }
        }

        public bool DisconnectNotification
        {
            get
            {
                return GetBoolSetting("DisconnectNotification", true);
            }
            set
            {
                SaveSetting("DisconnectNotification", value);
            }
        }

        public string ConnectStatusKey
        {
            get
            {
                return GetStringSetting("ConnectStatusKey", AwayFromDeskKey);
            }
            set
            {
                SaveSetting("ConnectStatusKey", value);
            }
        }

        public bool ConnectChangeStatus
        {
            get
            {
                return GetBoolSetting("ConnectChangeStatus", true);
            }
            set
            {
                SaveSetting("ConnectChangeStatus", value);
            }
        }

        public bool ConnectNotification
        {
            get
            {
                return GetBoolSetting("ConnectNotification", true);
            }
            set
            {
                SaveSetting("ConnectNotification", value);
            }
        }

        public string OutOfRangeStatusKey
        {
            get
            {
                return GetStringSetting("OutOfRangeStatusKey", AwayFromDeskKey);
            }
            set
            {
                SaveSetting("OutOfRangeStatusKey", value);
            }
        }

        public bool OutOfRangeChangeStatus
        {
            get
            {
                return GetBoolSetting("OutOfRangeChangeStatus", true);
            }
            set
            {
                SaveSetting("OutOfRangeChangeStatus", value);
            }
        }

        public bool OutOfRangeNotification
        {
            get
            {
                return GetBoolSetting("OutOfRangeNotification", true);
            }
            set
            {
                SaveSetting("OutOfRangeNotification", value);
            }
        }

        public string InRangeStatusKey
        {
            get
            {
                return GetStringSetting("InRangeStatusKey", AwayFromDeskKey);
            }
            set
            {
                SaveSetting("InRangeStatusKey", value);
            }
        }

        public bool InRangeChangeStatus
        {
            get
            {
                return GetBoolSetting("InRangeChangeStatus", true);
            }
            set
            {
                SaveSetting("InRangeChangeStatus", value);
            }
        }

        public bool InRangeNotification
        {
            get
            {
                return GetBoolSetting("InRangeNotification", true);
            }
            set
            {
                SaveSetting("InRangeNotification", value);
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
