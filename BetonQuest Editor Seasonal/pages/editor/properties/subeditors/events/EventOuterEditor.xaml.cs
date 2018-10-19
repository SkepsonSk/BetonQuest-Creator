using BetonQuest_Editor_Seasonal.logic;
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

namespace BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.events
{
    public partial class EventOuterEditor : Page
    {

        private bool doneButtonVisible = false;

        private Property property;
        private bool editing = false;

        // -------- Initializator --------

        public EventOuterEditor(Property property = null, bool editing = false)
        {
            InitializeComponent();

            this.property = property;
            this.editing = editing;

            if (editing)
            {
                Editing.Visibility = Visibility.Visible;
                Editing.Text = "Editing '" + property.Command + "'!";

                PathToCommand.Text = property.Command;
            }

            Tools.Animations.FadeIn(this, .25d, null);
        }

        // -------- Events --------

            private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PathToCommand.Text))
            {
                EditorHub.HubInstance.Alert("Property text box cannot be empty!", AlertType.Error);
                return;
            }
            if (property != null && Project.Quest.GetEvent(PathToCommand.Text, new string[] { property.ID }) != null)
            {
                EditorHub.HubInstance.Alert("An event with such name (meaning: path) already exists!", AlertType.Error);
                return;
            }
            else if (property == null && Project.Quest.GetEvent(PathToCommand.Text) != null)
            {
                EditorHub.HubInstance.Alert("An event with such name (meaning: path) already exists!", AlertType.Error);
                return;
            }

            else if (editing)
            {
                property.ID = PathToCommand.Text;

                Tools.PropertyListManagement.ReloadPropertyView(property, EventsPage.Instance.Properties);

                EditorHub.HubInstance.CallOffPriorityAlert();
                Tools.Animations.BackgroundColorAnimation(Panel, Colors.ForestGreen, .25d, true);
            }
            else
            {
                Property property = new Property(PathToCommand.Text, string.Empty);

                Project.Quest.Events.Add(property);
                Tools.PropertyListManagement.AddToPropertiesList(property, EventsPage.Instance.Properties, EventsPage.Instance.PropertyView_Interact);

                EditorHub.HubInstance.CallOffPriorityAlert();
                Tools.Animations.BackgroundColorAnimation(Panel, Colors.ForestGreen, .25d, true);

                EventsPage.Instance.EventsTitle.Text = "Created events (" + Project.Quest.Events.Count + "): ";
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(PathToCommand.Text) && doneButtonVisible)
            {
                doneButtonVisible = false;
                Tools.Animations.FadeOut(DoneButton, .25d, null);
            }
            else if (!string.IsNullOrEmpty(PathToCommand.Text) && !doneButtonVisible)
            {
                doneButtonVisible = true;
                Tools.Animations.FadeIn(DoneButton, .25d, null);
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EventsPage.Instance.CloseEditor();
            Tools.Animations.FadeOut(this, .25d, null);

            EditorHub.HubInstance.CallOffPriorityAlert();
            EditorHub.HubInstance.ShowNavigatorPanel();
        }

    }
}
