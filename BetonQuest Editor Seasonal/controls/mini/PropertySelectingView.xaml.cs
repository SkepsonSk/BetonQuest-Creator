using BetonQuest_Editor_Seasonal.logic.structure;
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

namespace BetonQuest_Editor_Seasonal.controls.mini
{
    public partial class PropertySelectingView : UserControl
    {

        private bool selected;
        private Property property;

        // -------- Initializator --------

        public PropertySelectingView(Property property, bool selected = false)
        {
            InitializeComponent();

            this.selected = selected;
            this.property = property;

            Title.Text = property.ID;

            Refresh();
        }

        // ----

        public void Refresh()
        {
            if (selected)
            {
                Background = new SolidColorBrush(Colors.ForestGreen);
                Title.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                Background = new SolidColorBrush(Color.FromRgb(166, 166, 166));
                Title.Foreground = new SolidColorBrush(Color.FromRgb(115, 115, 115));
            }
        }

        // -------- Access --------

        public bool Selected {
            get {
                return selected;
            }
            set {
                selected = value;
            }
        }

        public Property Property {
            get {
                return property;
            }
            set {
                property = value;
            }
        }

        // -------- Events --------

        private void Click(object sender, MouseButtonEventArgs e)
        {
            selected = !selected;
            Refresh();
        }
    }
}
