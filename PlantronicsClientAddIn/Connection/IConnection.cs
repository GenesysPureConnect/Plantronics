using System;

namespace PlantronicsClientAddIn.Connection
{
    public interface IConnection
    {
        bool IsConnected { get; }
        event EventHandler UpChanged;
    }
}
