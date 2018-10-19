using BetonQuest_Editor_Seasonal.logic.structure.items;
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
    public partial class EnchantEditingView : UserControl
    {
        private EnchantSet enchantSet;

        // -------- Initializator --------

        public EnchantEditingView(EnchantSet enchantSet)
        {
            InitializeComponent();

            this.enchantSet = enchantSet;

            ID.Text = enchantSet.Name;
        }

        // -------- Access --------

        public EnchantSet EnchantSet {
            get {
                return enchantSet;
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
