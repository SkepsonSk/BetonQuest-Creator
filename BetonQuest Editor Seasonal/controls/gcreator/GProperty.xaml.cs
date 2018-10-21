using BetonQuest_Editor_Seasonal.logic.gcreator;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetonQuest_Editor_Seasonal.controls.gcreator
{
    public partial class GProperty : UserControl, IPanel
    {

        private List<PanelConnection> panelConnections;

        private Property property;
        private PropertyType type;

        private bool negated;

        // -------- Start --------

        public GProperty(PropertyType type, Property property = null)
        {
            InitializeComponent();

            panelConnections = new List<PanelConnection>();

            DeleteItem.Tag = this;

            this.property = property;
            this.type = type;

            Width = 250d;

            if (property == null)
            {
                Height = 275d;

                Prepare();
                LoadChoosingPanel();
            }
            else
            {
                Prepare();
                Update();

                PropertiesScrollViewer.Visibility = Visibility.Collapsed;
                PropertiesDataBorder.Visibility = Visibility.Visible;

                Height = 190d;
            }

        }

        // -------- Events --------

        private void View_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Property property = (sender as View).Data[0] as Property;
            this.property = property;

            foreach (PanelConnection panel in panelConnections)
            {
                if (type == PropertyType.Condition) ((panel.First as IPanel).GetBoundProperty() as Statement).Conditions.Add(property);
                else if (type == PropertyType.Event) ((panel.First as IPanel).GetBoundProperty() as Statement).Events.Add(property);
            }

            Update();

            Tools.Animations.FadeOut(PropertiesScrollViewer, .25d, null);
            Tools.Animations.Slide(this, 250d, 190d, .25d, ChangeToPropertyDataDisplay);
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            foreach (PanelConnection panel in panelConnections)
            {
                Statement statement = (panel.First as IPanel).GetBoundProperty() as Statement;
                if (type == PropertyType.Condition)
                {
                    statement.Conditions.Remove(property);
                    if (statement.NegatedConditions.Contains(property.ID)) statement.NegatedConditions.Remove(property.ID);
                }
                else if (type == PropertyType.Event) statement.Events.Remove(property);
            }

            property = null;
            negated = false;

            Tools.Animations.FadeOut(PropertiesDataBorder, .25d, null);
            Tools.Animations.Slide(this, 190d, 250d, .25d, ChangeToPropertyChoosePanel);
        }

        private void Negation_Click(object sender, RoutedEventArgs e)
        {
            negated = !negated;

            foreach (PanelConnection panel in panelConnections)
            {
                Statement statement = (panel.First as IPanel).GetBoundProperty() as Statement;  

                if (negated) statement.NegatedConditions.Add(property.ID);
                else statement.NegatedConditions.Remove(property.ID);
            }

            if (negated) PropertyContent.Text = "!" + property.Command;
            else PropertyContent.Text =  property.Command;
        }

        // ----

        private void ChangeToPropertyDataDisplay(object sender, EventArgs e)
        {
            PropertiesDataBorder.Opacity = 0;

            PropertiesScrollViewer.Visibility = Visibility.Collapsed;
            PropertiesDataBorder.Visibility = Visibility.Visible;

            Tools.Animations.FadeIn(PropertiesDataBorder, .25d, null);
        }

        private void ChangeToPropertyChoosePanel(object sender, EventArgs e)
        {
            if (Properties.Children.Count == 0) LoadChoosingPanel();

            PropertiesScrollViewer.Opacity = 0;

            PropertiesDataBorder.Visibility = Visibility.Collapsed;
            PropertiesScrollViewer.Visibility = Visibility.Visible;

            Tools.Animations.FadeIn(PropertiesScrollViewer, .25d, null);
        }

        // -------- Access --------

        public void Prepare()
        {
            Title.Foreground = Brushes.White;
            TitleSeparator.Background = Brushes.White;
            PropertyName.Foreground = Brushes.White;
            PropertyContent.Foreground = Brushes.White;

            if (type == PropertyType.Condition)
            {
                Title.Text = "CONDITION";
                negated = false;

                Body.Background = new SolidColorBrush(Color.FromRgb(255, 173, 51));
                PropertiesDataBorder.Background = new SolidColorBrush(Color.FromRgb(255, 153, 0));
            }
            else if (type == PropertyType.Event)
            {
                Title.Text = "EVENT"; ;

                Negation.Visibility = Visibility.Collapsed;

                Body.Background = new SolidColorBrush(Color.FromRgb(0, 153, 0));
                PropertiesDataBorder.Background = new SolidColorBrush(Color.FromRgb(0, 179, 0));
            }
        }

        public void LoadChoosingPanel()
        {
            List<Property> properties = null;

            if (type == PropertyType.Condition) properties = Project.Quest.Conditions;
            else if (type == PropertyType.Event) properties = Project.Quest.Events;

            foreach (Property propertyData in properties)
            {
                View view = new View(propertyData.ID, new object[] { propertyData }, true, false);
                view.MouseDown += View_MouseDown;
                view.SetHeadFontSize(16d);
                view.Margin = new Thickness(0d, 0d, 0d, 2.5d);

                Properties.Children.Add(view);
            }
        }

        public void Update()
        {
            if (property == null) return;

            PropertyName.Text = property.ID;
            PropertyContent.Text = property.Command;
        }

        public PropertyType Type {
            get {
                return type;
            }
        }

        public bool Negated {
            get {
                return negated;
            }
        }

        // --------

        public List<PanelConnection> GetPanelConnections()
        {
            return panelConnections;
        }

        public Property GetBoundProperty()
        {
            return property;
        }

        public void BindProperty(Property property)
        {
            this.property = property;
        }

    }
}
