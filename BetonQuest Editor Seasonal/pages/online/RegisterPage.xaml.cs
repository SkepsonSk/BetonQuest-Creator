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
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
            Tools.Animations.FadeIn(this, .25d, null);
        }

        // -------- Events --------

        private void ReturnButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToLoginPage);
        }

        private void QuitButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToWelcomePage);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ServerSession.CurrentSession.Register(Username.Text, Password.Password, RepeatPassword.Password, Mail.Text, RegisterSucceed, RegisterFailed);
        }

        // -------- Event-Based Operations --------

        private void RegisterSucceed()
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToWelcomePage);
        }

        private void RegisterFailed()
        {
            Respond.Foreground = new SolidColorBrush(Colors.Firebrick);
            Respond.Text = "Registering failed!";
            Respond.Visibility = Visibility.Visible;
        }

        private void SwitchToLoginPage(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Content = new LoginPage();
        }

        private void SwitchToWelcomePage(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Content = new WelcomePage();
        }


    }
}
