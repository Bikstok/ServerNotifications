using System.Collections.Generic;

namespace ServerNotifications.Interfaces
{
    public interface IServer
    {
        string Host { get; set; }
        string Name { get; set; }
        string PlayerCount { get; set; }
        List<string> AllPlayers { get; set; }
        List<string> FilteredPlayers { get; set; }
    }
}