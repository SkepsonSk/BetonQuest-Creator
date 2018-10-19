using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.logic.control;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetonQuest_Editor_Seasonal.pages.editor
{
    public partial class EventsPage : Page, ControllablePage
    {
        private static EventsPage instance;

        // -------- Initializator --------

        public EventsPage()
        {
            InitializeComponent();
            instance = this;

            if (Project.Quest.Events.Count == 0) Tools.PropertyListManagement.AddEmptyView(Properties, "No events created!");
            else Tools.PropertyListManagement.LoadPropertiesToList(Properties, Project.Quest.Events, PropertyView_Interact);

            EventsTitle.Text = "Created events (" + Project.Quest.Events.Count + "): ";

            ControlManager.RegisteredPages.Add(this);
        }

        // -------- Editor Frame Operation --------

        public void OpenEditor(Page page)
        {
            Editor.Visibility = Visibility.Visible;
            Editor.Navigate(page);

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0d;
            animation.To = 10d;
            animation.Duration = TimeSpan.FromSeconds(0.25d);

            BlurEffect effect;

            if (LeftPanel.Effect != null && LeftPanel.Effect is BlurEffect) effect = (BlurEffect)LeftPanel.Effect;
            else effect = new BlurEffect();

            effect.Radius = 0;

            effect.BeginAnimation(BlurEffect.RadiusProperty, animation);

            LeftPanel.Effect = effect;
            RightPanel.Effect = effect;

            EditorHub.HubInstance.HideNavigatorPanel();

        }

        public void CloseEditor()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 10d;
            animation.To = 0d;
            animation.Duration = TimeSpan.FromSeconds(0.5d);

            BlurEffect effect = (BlurEffect) LeftPanel.Effect;

            animation.Completed += HideEditorFrame;
            effect.BeginAnimation(BlurEffect.RadiusProperty, animation);

            LeftPanel.Effect = effect;
            RightPanel.Effect = effect;
        }

        private void HideEditorFrame(object sender, EventArgs e)
        {
            Editor.Visibility = Visibility.Hidden;
        }

        // -------- Access --------

        public static EventsPage Instance {
            get {
                return instance;
            }
            set {
                instance = value;
            }
        }

        // -------- Events --------

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string type = (sender as Button).Name;

            if (type == "Default") OpenEditor(new EventDefaultEditor());
            else if (type == "Outer") OpenEditor(new EventOuterEditor());

        }

        public void PropertyView_Interact(object sender, MouseButtonEventArgs e)
        {
            View view = sender as View;

            bool removingMode = (bool)view.Data[1];

            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (!removingMode)
                {
                    view.Data[1] = true;

                    view.Head.Foreground = new SolidColorBrush(Colors.White);
                    view.Body.Foreground = new SolidColorBrush(Colors.White);

                    view.Background = new SolidColorBrush(Colors.Firebrick);
                }
                else
                {
                    view.Data[1] = false;

                    view.Head.Foreground = new SolidColorBrush(Color.FromRgb(115, 115, 115));
                    view.Body.Foreground = new SolidColorBrush(Color.FromRgb(115, 115, 115));

                    view.Background = new SolidColorBrush(Color.FromRgb(166, 166, 166));
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                Property property = view.Data[0] as Property;

                if (removingMode)
                {
                    Project.QuestUndoOperations.Push(new QuestDataImage(Project.Quest));

                    Project.Quest.Events.Remove(property);
                    Tools.PropertyListManagement.RemoveFromPropertiesList(property, Properties, Project.Quest.Events, "No events created!");
                    EventsTitle.Text = "Created events (" + Project.Quest.Events.Count + "): ";
                }
                else OpenEditor(new EventDefaultEditor(property, true));

            }
        }

        // -------- Controllable Page --------

        public void ReloadProperties()
        {
            Properties.Children.Clear();

            if (Project.Quest.Events.Count == 0) Tools.PropertyListManagement.AddEmptyView(Properties, "No events created!");
            else Tools.PropertyListManagement.LoadPropertiesToList(Properties, Project.Quest.Events, PropertyView_Interact);
        }
    }
}