using ServerNotifications.Interfaces;
using System.Collections.Generic;

namespace ServerNotifications.Model
{
    public class Server : IServer
    {
        public Server (string host)
        {
            this.Host = host;
            this.Name = host;
        }

        private string _host;
        private string _name;
        private string _playerCount;
        private List<string> _allPlayers;
        private List<string> _filteredPlayers;

        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string PlayerCount
        {
            get { return _playerCount; }
            set { _playerCount = value; }
        }

        public List<string> AllPlayers
        {
            get { return _allPlayers; }
            set { _allPlayers = value; }
        }

        public List<string> FilteredPlayers
        {
            get { return _filteredPlayers; }
            set { _filteredPlayers = value; }
        }
    }
}
