using ServerNotifications.Interfaces;
using ServerNotifications.Model;
using SSQLib;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace ServerNotifications.Controllers
{
    public class BrowserController
    {
        private List<IServer> _servers;
        private Settings settings;
        private string _errors;

        public BrowserController()
        {
            settings = new Settings(10, 10);
            Servers = readServers();
        }

        public List<IServer> Servers
        {
            get { return _servers; }
            set { _servers = value; }
        }

        public List<string> Filters
        {
            get { return settings.Filters; }
        }

        public int RefreshTimer
        {
            get { return settings.RefreshTimer; }
            set { settings.RefreshTimer = value; }
        }

        public int NotificationMargin
        {
            get { return settings.NotificationMargin; }
            set { settings.NotificationMargin = value; }
        }

        public string Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }

        public void AddServer(string serverString)
        {
            Server server = new Server(serverString);
            Servers.Add(server);
            writeServers();

            Errors = "";
            updateServerInfo(server);
        }

        public void RemoveServer(IServer server)
        {
            Servers.Remove((Server)server);
            writeServers();
        }

        public List<IServer> UpdateAllServers()
        {
            Errors = "";
            foreach (Server server in Servers)
            {
                updateServerInfo(server);
            }
            return Servers;
        }

        public void AddFilter(string filter)
        {
            settings.Filters.Add(filter);
            settings.Write("Resources/filters.txt", settings.Filters);
        }

        public void RemoveFilter(string filter)
        {
            settings.Filters.Remove(filter);
            settings.Write("Resources/filters.txt", settings.Filters);
        }

        private void updateServerInfo(Server server)
        {
            try
            {
                string[] serverSplit = server.Host.Split(':');
                string host = serverSplit[0];
                int port = int.Parse(serverSplit[1]);

                //string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
                string ValidHostnameRegex = @"^(([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$";

                string ip = host;
                if (Regex.IsMatch(host, ValidHostnameRegex))
                {
                    // the string is a host
                    IPHostEntry ipHost = Dns.GetHostEntry(host);
                    ip = ipHost.AddressList[0].ToString();
                }

                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);

                SSQL query = new SSQL();
                ServerInfo serverInformation = query.Server(endpoint);

                List<PlayerInfo> players = query.Players(endpoint);
                List<string> allPlayers = new List<string>();
                List<string> filteredPlayers = new List<string>();

                foreach (var player in players)
                {
                    allPlayers.Add(player.Name);
                    foreach (var filter in settings.Filters)
                    {
                        if (player.Name.IndexOf(filter) > -1 && !filteredPlayers.Contains(player.Name))
                        {
                            filteredPlayers.Add(player.Name);
                        }
                    }
                }

                server.Name = serverInformation.Name;
                server.PlayerCount = serverInformation.PlayerCount + "/" + serverInformation.MaxPlayers;
                server.AllPlayers = allPlayers;
                server.FilteredPlayers = filteredPlayers;
            }
            catch
            {
                Errors += server.Host + " timed out.\n";
            }
        }

        private void writeServers()
        {
            using (StreamWriter writetext = new StreamWriter("Resources/servers.txt"))
            {
                foreach (Server server in Servers)
                {
                    if (server != null)
                    {
                        writetext.Write(server.Host + "|");
                    }
                }
            }
        }

        private List<IServer> readServers()
        {
            List<IServer> servers = new List<IServer>();
            if (File.Exists("Resources/servers.txt"))
            {
                using (StreamReader readtext = new StreamReader("Resources/servers.txt"))
                {
                    string text = readtext.ReadToEnd();
                    string[] array = text.Split('|');

                    foreach (string host in array)
                    {
                        if (host != "")
                        {
                            Server server = new Server(host);
                            servers.Add(server);
                        }
                    }
                }
            }
            return servers;
        }
    }
}
