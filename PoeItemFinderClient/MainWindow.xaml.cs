using System;
using System.Net.Http;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;

namespace PoeItemFinderClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public IHubProxy HubProxy { get; set; }
        const string ServerURI = "http://localhost:8080/signalr";
        public HubConnection Connection { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            increase_button.IsEnabled = false;
        }

        private async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);
            Connection.Closed += Connection_Closed;
            HubProxy = Connection.CreateHubProxy("TestHub");
            //Handle incoming event from server: use Invoke to write to console from SignalR's thread
            HubProxy.On<int>("UpdateCounter", (counter) =>
                this.Dispatcher.Invoke(() =>
                    counter_label.Content = "" + counter
                )
            );
            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException)
            {
                //StatusText.Content = "Unable to connect to server: Start server before connecting clients.";
                //No connection: Don't enable Send button or show chat UI
                return;
            }
            increase_button.IsEnabled = true;
        }

        void Connection_Closed()
        {

        }

        private void connect_button_Click(object sender, RoutedEventArgs e)
        {
            ConnectAsync();
        }

        private void increase_button_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Increase");
        }
    }
}
