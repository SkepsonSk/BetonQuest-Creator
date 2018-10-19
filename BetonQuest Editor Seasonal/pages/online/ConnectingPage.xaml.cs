using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BetonQuest_Editor_Seasonal.pages.online
{
    public partial class ConnectingPage : Page
    {

        // -------- Initializator --------

        public ConnectingPage()
        {
            InitializeComponent();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Test;
            worker.RunWorkerAsync();

            Tools.Animations.FadeIn(this, .25d, null);
        }

        // --------

        private void Test(object sender, EventArgs e)
        {

            WebRequest request = WebRequest.Create("http://localhost/bqe_online");

            try
            {
                using (WebResponse response = request.GetResponse()) Dispatcher.Invoke(Online);
            }
            catch (WebException exception)
            {
                using (WebResponse response = exception.Response)
                {
                    if (response == null) Dispatcher.Invoke(Offline);
                }
            }

        }

        // -------- UI --------

        private void Online()
        {
            Tools.Animations.FadeOut(Title, .25d, SwitchToAuthorizationPage);
            ServerSession.ServerUp = true;
        }

        private void Offline()
        {
            Title.Text = "OFFLINE!";
            Description.Text = "Editor could not connect to the server!";

            Title.Foreground = new SolidColorBrush(Colors.DarkRed);
            Description.Foreground = new SolidColorBrush(Colors.Firebrick);

            ServerSession.ServerUp = false;

            OKButton.IsEnabled = true;
            Tools.Animations.FadeIn(OKButton, .25d, null);
        }

        // -------- Events --------

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToWelcomePage);
        }

        // -------- Event-Based Operations --------

        private void SwitchToAuthorizationPage(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Content = new LoginPage();
        }

        private void SwitchToWelcomePage(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Content = new WelcomePage();
        }

    }
}
