using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.events;
using BetonQuest_Editor_Seasonal.pages.online.market.subpages;
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

namespace BetonQuest_Editor_Seasonal.pages.online.market
{
    public partial class MarketMainPage : Page
    {

        private static MarketMainPage marketInstance;

        public MarketMainPage()
        {
            InitializeComponent();

            marketInstance = this;

            Username.Text = ServerSession.CurrentSession.Username;
            Navigator.Navigate(new MarketWelcomePage());

            Tools.Animations.FadeIn(this, .25d, null);
        }

        // -------- Access --------

        public static MarketMainPage MarketInstance {
            get {
                return marketInstance;
            }
        }

        // -------- Events --------

        private void MarketMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(new QuestsPage());
        }

        private void WelcomePageButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToWelcomePage);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // -------- Event-Based Operations --------

        private void SwitchToWelcomePage(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Content = new WelcomePage();
        }


    }
}
