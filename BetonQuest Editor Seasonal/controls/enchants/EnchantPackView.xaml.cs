using BetonQuest_Editor_Seasonal.logic.settings;
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

namespace BetonQuest_Editor_Seasonal.controls.enchants
{
    public partial class EnchantPackView : UserControl
    {
        private EnchantPack enchantPack;

        // -------- Initializator --------

        public EnchantPackView(EnchantPack enchantPack)
        {
            InitializeComponent();

            this.enchantPack = enchantPack;

            ID.Text = enchantPack.ID;

            Refresh();
        }

        public void Refresh()
        {
            if (enchantPack.Enabled)
            {
                ID.Foreground = new SolidColorBrush(Colors.White);
                Background = new SolidColorBrush(Colors.ForestGreen);
            }
            else
            {
                ID.Foreground = new SolidColorBrush(Color.FromRgb(115, 115, 115));
                Background = new SolidColorBrush(Color.FromRgb(166, 166, 166));
            }
        }

        // -------- Access --------

        public EnchantPack EnchantPack {
            get {
                return enchantPack;
            }
        }

    }
}
