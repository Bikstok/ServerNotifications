using ServerNotifications.Controllers;
using ServerNotifications.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace ServerNotifications
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BrowserController controller;
        private Timer aTimer;
        private DateTime lastRefreshed;
        private List<IServer> servers;

        public MainWindow()
        {
            InitializeComponent();
            controller = new BrowserController();
            
            filtersListBox.ItemsSource = controller.Filters;
            refreshServers();

            aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(onTimedEvent);
            aTimer.Interval = controller.RefreshTimer * 60000;
            aTimer.Enabled = true;
            refreshTimerTextBox.Text = (aTimer.Interval / 60000).ToString();
            notificationMarginTextBox.Text = controller.NotificationMargin.ToString();
        }

        private void refreshServers()
        {
            refreshButton.IsEnabled = false;
            serversDataGrid.ItemsSource = null;
            filteredPlayersDataGrid.ItemsSource = null;
            allPlayersDataGrid.ItemsSource = null;
            lastRefreshed = DateTime.Now;

            servers = new List<IServer>();
            var bw = new BackgroundWorker();
            bw.DoWork += (s, _) =>
            {
                servers = controller.UpdateAllServers();
            };
            bw.RunWorkerCompleted += (s, _) => refreshServersUI(servers);
            bw.RunWorkerAsync();
        }

        private void refreshServersUI(List<IServer> servers)
        {
            serversDataGrid.ItemsSource = servers;
            statusLabel.Content = "";
            bool notification = false;
            for (int i = 0; i < servers.Count; i++)
            {
                if (servers[i].FilteredPlayers != null && servers[i].FilteredPlayers.Count > controller.NotificationMargin)
                {
                    notification = true;
                    statusLabel.Content += "Lots of activity on " + servers[i].Name + " \n";

                    try
                    {
                        SoundPlayer player = new SoundPlayer("Resources/sound.wav");
                        player.Play();
                    }
                    catch
                    {
                        MessageBox.Show("Could not locate Resources/sound.wav.");
                    }
                }
            }
            if (notification == false)
            {
                statusLabel.Content = "Nothing happening..";
            }
            refreshButton.IsEnabled = true;
            refreshLabel.Content = "Last refreshed: " + lastRefreshed.ToShortTimeString();
            errorLabel.Content = controller.Errors;
        }
        
        private void onTimedEvent(object source, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
              System.Windows.Threading.DispatcherPriority.Background,
              new Action(() => refreshServers()));
        }

        private void serversDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (serversDataGrid.SelectedItem != null)
            {
                IServer selectedServer = (IServer)serversDataGrid.SelectedItem;
                filteredPlayersDataGrid.ItemsSource = selectedServer.FilteredPlayers;
                allPlayersDataGrid.ItemsSource = selectedServer.AllPlayers;
            }
        }

        private void refreshButtonClick(object sender, RoutedEventArgs e)
        {
            refreshServers();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            controller.AddFilter(newFilterTextBox.Text);
            filtersListBox.Items.Refresh();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (filtersListBox.SelectedItem != null)
            {
                controller.RemoveFilter(filtersListBox.SelectedItem.ToString());
                filtersListBox.Items.Refresh();
            }
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            int interval;
            int.TryParse(refreshTimerTextBox.Text, out interval);

            if (interval > 0)
            {
                int intervalMS = interval * 60000;
                controller.RefreshTimer = intervalMS;
                aTimer.Interval = controller.RefreshTimer;
            }
            else
            {
                MessageBox.Show("Refresh timer must be higher than 0.");
            }

            int notificationMargin;
            int.TryParse(notificationMarginTextBox.Text, out notificationMargin);

            if (notificationMargin > 0 && notificationMargin < 100)
            {
                controller.NotificationMargin = notificationMargin;
            }
            else
            {
                MessageBox.Show("Notification margin must be higher than 0 and lower than 100.");
            }
        }

        private void addServerButton_Click(object sender, RoutedEventArgs e)
        {


            addServerButton.IsEnabled = false;
            string newServer = newServerTextBox.Text;

            var bw = new BackgroundWorker();
            bw.DoWork += (s, _) =>
            {
                controller.AddServer(newServer);
            };
            bw.RunWorkerCompleted += (s, _) => addServerUI();
            bw.RunWorkerAsync();
        }

        private void addServerUI()
        {
            addServerButton.IsEnabled = true;
            serversDataGrid.Items.Refresh();
            errorLabel.Content = controller.Errors;
        }

        private void deleteServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (serversDataGrid.SelectedItem != null)
            {
                IServer selectedServer = (IServer) serversDataGrid.SelectedItem;
                controller.RemoveServer(selectedServer);
                serversDataGrid.Items.Refresh();
            }
        }
    }
}
