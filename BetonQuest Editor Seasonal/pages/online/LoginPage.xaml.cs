using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace BetonQuest_Editor_Seasonal.pages.online
{
    public partial class LoginPage : Page
    {

        private static LoginPage instance;

        // -------- Initializator --------

        public LoginPage()
        {
            InitializeComponent();

            instance = this;
            Tools.Animations.FadeIn(this, .25d, null);

            if (ServerSession.CurrentSession == null) new ServerSession();
        }

        // -------- Access --------

        public static LoginPage Instance {
            get {
                return instance;
            }
        }

        // -------- Events --------

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToWelcomePage);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ServerSession.CurrentSession.Login(Username.Text, Password.Password, LoginSucceed, LoginFailed);
        }

        private void RegisterButton_Click(object sender, MouseButtonEventArgs e)
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToRegisterPage);
        }

        // -------- Post Connect --------
        // ---- Login ----

        private void LoginSucceed()
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToWelcomePage);
        }

        private void LoginFailed()
        {
            Respond.Foreground = new SolidColorBrush(Colors.Firebrick);
            Respond.Text = "Login failed!";
            Respond.Visibility = Visibility.Visible;
        }

        // -------- Event-Based operations --------

        private void SwitchToWelcomePage(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Content = new WelcomePage();
        }

        private void SwitchToRegisterPage(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Content = new RegisterPage();
        }

    }
}
