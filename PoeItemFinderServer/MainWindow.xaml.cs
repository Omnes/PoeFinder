using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace PoeItemFinderServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public IDisposable SignalR { get; set; }
        const string ServerURI = "http://localhost:8080";
        private ItemRiver itemRiver = new ItemRiver();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            WriteToConsole("Starting server...");
            start_button.IsEnabled = false;
            Task.Run(() => StartServer());
            Task.Run(() => itemRiver.pullTask());
        }

        private void stop_button_Click(object sender, RoutedEventArgs e)
        {
            SignalR.Dispose();
            Close();
        }

        public void StartServer()
        {
            try
            {
                SignalR = WebApp.Start(ServerURI);
            }
            catch (TargetInvocationException)
            {
                WriteToConsole("A server is already running at " + ServerURI);
                this.Dispatcher.Invoke(() => start_button.IsEnabled = true);
                return;
            }
            this.Dispatcher.Invoke(() => stop_button.IsEnabled = true);
            WriteToConsole("Server started at " + ServerURI);
        }

        public void WriteToConsole(String message)
        {
            if (!(console_log.CheckAccess()))
            {
                this.Dispatcher.Invoke(() =>
                    WriteToConsole(message)
                );
                return;
            }
            console_log.AppendText(message + "\r");
        }
    }
}
