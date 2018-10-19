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

namespace BetonQuest_Editor_Seasonal.pages.settings
{
    public partial class SettingsHub : Page
    {

        // -------- Initializator --------

        public SettingsHub()
        {
            InitializeComponent();
        }

        // -------- Events --------

        private void MenuButton_Click(object sender, MouseButtonEventArgs e)
        {
            string name = ((WrapPanel) sender).Tag as string;
        
            if (name.Equals("EnchantPacks"))
            {
                NavigatorFrame.Navigate(new EnchantsSettingsPage());
            }
        }
    }
}
