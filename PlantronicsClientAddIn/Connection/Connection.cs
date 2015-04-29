using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Connection
{
    public class Connection :IConnection
    {
        private ININ.IceLib.Connection.Session _icelibSession;
        public event EventHandler UpChanged;

        public Connection(ININ.IceLib.Connection.Session icelibSession)
        {
            _icelibSession = icelibSession;
            _icelibSession.ConnectionStateChanged += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object sender, ININ.IceLib.Connection.ConnectionStateChangedEventArgs e)
        {
            if (e.State == ININ.IceLib.Connection.ConnectionState.Up || e.State == ININ.IceLib.Connection.ConnectionState.Down)
            {
                if (UpChanged != null)
                {
                    UpChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool IsConnected
        {
            get 
            {
                return _icelibSession.ConnectionState == ININ.IceLib.Connection.ConnectionState.Up;
            }
        }
    }
}
