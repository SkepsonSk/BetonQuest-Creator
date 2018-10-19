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

    public partial class PropertyNegableSelectingView : UserControl
    {

        private bool selected;
        private bool negated;

        private Property property;

        // -------- Initializator --------

        public PropertyNegableSelectingView(Property property, bool selected = false, bool negated = false)
        {
            InitializeComponent();

            this.selected = selected;
            this.negated = negated;

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

            if (selected && negated)
            {
                Negation.Background = new SolidColorBrush(Colors.ForestGreen);
                Negation.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                Negation.Background = new SolidColorBrush(Color.FromRgb(166, 166, 166));
                Negation.Foreground = new SolidColorBrush(Color.FromRgb(115, 115, 115));
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

        public bool Negated {
            get {
                return negated;
            }
            set {
                negated = value;
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

        private void Select(object sender, MouseButtonEventArgs e)
        {
            selected = !selected;
            if (!selected) negated = false;

            Refresh();
        }

        private void Negation_Click(object sender, RoutedEventArgs e)
        {
            if (selected) negated = !negated;
            Refresh();
        }
    }
}
