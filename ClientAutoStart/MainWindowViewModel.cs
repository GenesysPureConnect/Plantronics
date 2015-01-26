using Plantronics.UC.SpokesWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClientAutoStart
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string Key = "PlantronicsKey";

        public event PropertyChangedEventHandler PropertyChanged;
        public string FilePath { get; private set; }
        protected XmlDocument _xmlSettings;

        private Spokes _spokes = null;

        public MainWindowViewModel()
        {
            _spokes = Spokes.Instance;
            _spokes.Connected += OnHeadsetConnected;
            _spokes.Connect("Interaction Client Launcher");

            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FilePath = Path.Combine(assemblyFolder, "AutoStartSettings.xml");

            _xmlSettings = new XmlDocument();

            if (File.Exists(FilePath))
            {
                _xmlSettings.Load(FilePath);
            }
            else
            {
                _xmlSettings.LoadXml("<settings/>");
            }

            StartClientWhenHeadsetIsConnected = GetBoolSetting("StartClientWhenHeadsetIsConnected", false);
            NtAuth = GetBoolSetting("NtAuth", true);
            CicServer = GetStringSetting("CicServer", String.Empty);
            Station = GetStringSetting("Station", String.Empty);
          
        }

        void _spokes_Attached(object sender, AttachedArgs e)
        {
            
        }

        private void OnHeadsetConnected(object sender, ConnectedStateArgs e)
        {
            if (StartClientWhenHeadsetIsConnected)
            {
                if (NtAuth)
                {
                    Launcher.LaunchWithNtAuth(CicServer, Station);
                }
                else
                {
                    Launcher.LaunchWithIcAuth(CicServer, Station);
                }
            }
        }

        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool _startClientWhenHeadsetIsConnected;
        public bool StartClientWhenHeadsetIsConnected
        {
            get
            {
                return _startClientWhenHeadsetIsConnected;
            }
            set
            {
                _startClientWhenHeadsetIsConnected = value;
                SaveSetting("StartClientWhenHeadsetIsConnected", value);
                RaisePropertyChanged("StartClientWhenHeadsetIsConnected");
            }
        }

        private bool _ntAuth;
        public bool NtAuth
        {
            get
            {
                return _ntAuth;
            }
            set
            {
                PasswordEnabled = !value;
                _ntAuth = value;
                SaveSetting("NtAuth", value);
                RaisePropertyChanged("NtAuth");
            }
        }

        private bool _passwordEnabled;
        public bool PasswordEnabled
        {
            get
            {
                return _passwordEnabled;
            }
            set
            {
                _passwordEnabled = value;
                RaisePropertyChanged("PasswordEnabled");
            }
        }

        private string _cicServer;
        public string CicServer
        {
            get
            {
                return _cicServer;
            }
            set
            {
                _cicServer = value;
                SaveSetting("CicServer", value);
                RaisePropertyChanged("CicServer");
            }
        }

        private string _station;
        public string Station
        {
            get
            {
                return _station;
            }
            set
            {
                _station = value;
                SaveSetting("Station", value);
                RaisePropertyChanged("Station");
            }
        }


        private bool GetBoolSetting(string key, bool defaultValue)
        {
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
            if (elements == null || elements.Count == 0)
            {
                return defaultValue;
            }

            return elements[0].InnerText;
        }

        private void SaveSetting(string key, object value)
        {
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
