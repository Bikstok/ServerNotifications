using System.Collections.Generic;
using System.IO;

namespace ServerNotifications.Model
{
    public class Settings
    {
        private List<string> _filters;
        private int _refreshTimer;
        private int _notificationMargin;

        public Settings(int refreshTimer, int notificationMargin)
        {
            this.RefreshTimer = refreshTimer;
            this.NotificationMargin = notificationMargin;
            this.Filters = Read("Resources/filters.txt");
        }

        public void Write(string path, List<string> items)
        {
            using (StreamWriter writetext = new StreamWriter(path))
            {
                foreach (string item in items)
                {
                    if (item != "")
                    {
                        writetext.Write(item + ":");
                    }
                }
            }
        }

        public List<string> Read(string path)
        {
            List<string> items = new List<string>();
            if (File.Exists(path))
            {
                using (StreamReader readtext = new StreamReader(path))
                {
                    string text = readtext.ReadToEnd();
                    string[] array = text.Split(':');

                    foreach (string line in array)
                    {
                        if (line != "")
                        {
                            items.Add(line);
                        }
                    }
                }
            }
            return items;
        }

        public List<string> Filters
        {
            get { return _filters; }
            set { _filters = value; }
        }

        public int RefreshTimer
        {
            get { return _refreshTimer; }
            set { _refreshTimer = value; }
        }

        public int NotificationMargin
        {
            get { return _notificationMargin; }
            set { _notificationMargin = value; }
        }
    }
}
