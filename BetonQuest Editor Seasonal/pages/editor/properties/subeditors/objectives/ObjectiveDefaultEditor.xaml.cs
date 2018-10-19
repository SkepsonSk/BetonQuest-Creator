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

namespace BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.objectives
{
    public partial class ObjectiveDefaultEditor : Page
    {
        private bool doneButtonVisible = false;

        private Property property;
        private bool editing = false;

        // -------- Initializator --------

        public ObjectiveDefaultEditor(Property property = null, bool editing = false)
        {
            InitializeComponent();

            this.property = property;
            this.editing = editing;

            if (editing)
            {
                Editing.Visibility = Visibility.Visible;
                Editing.Text = "Editing '" + property.ID + "'!";

                ID.Text = property.ID;
                Command.Text = property.Command;
            }

            Tools.Animations.FadeIn(this, .25d, null);
        }

        // -------- Done Button (Event-Based) Operations --------

        private void EnableDoneButton(object sender, EventArgs e)
        {
            DoneButton.IsEnabled = true;
        }

        private void DisableDoneButton(object sender, EventArgs e)
        {
            DoneButton.IsEnabled = false;
        }

        // -------- Correction Checking --------

        private bool PropertyTextBoxesFilled()
        {
            if (string.IsNullOrEmpty(ID.Text) || string.IsNullOrEmpty(Command.Text)) return false;
            return true;
        }

        // -------- Events --------

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (!PropertyTextBoxesFilled())
            {
                EditorHub.HubInstance.Alert("Property text boxes cannot be empty!", AlertType.Error);
                return;
            }
            if (property != null && Project.Quest.GetObjective(ID.Text, new string[] { property.ID }) != null)
            {
                EditorHub.HubInstance.Alert("An objective with such name already exists!", AlertType.Error);
                return;
            }
            else if (property == null && Project.Quest.GetObjective(ID.Text) != null)
            {
                EditorHub.HubInstance.Alert("An objective with such name already exists!", AlertType.Error);
                return;
            }

            else if (editing)
            {
                Project.QuestUndoOperations.Push(new QuestDataImage(Project.Quest));

                property.ID = ID.Text;
                property.Command = Command.Text;

                Tools.PropertyListManagement.ReloadPropertyView(property, ObjectivesPage.Instance.Properties);

                EditorHub.HubInstance.CallOffPriorityAlert();
                Tools.Animations.BackgroundColorAnimation(Panel, Colors.ForestGreen, .25d, true);
            }
            else
            {
                Project.QuestUndoOperations.Push(new QuestDataImage(Project.Quest));

                Property property = new Property(ID.Text, Command.Text);

                Project.Quest.Objectives.Add(property);
                Tools.PropertyListManagement.AddToPropertiesList(property, ObjectivesPage.Instance.Properties, ObjectivesPage.Instance.PropertyView_Interact);

                EditorHub.HubInstance.CallOffPriorityAlert();
                Tools.Animations.BackgroundColorAnimation(Panel, Colors.ForestGreen, .25d, true);

                ObjectivesPage.Instance.ObjectivesTitle.Text = "Created objectives (" + Project.Quest.Objectives.Count + "):";
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ObjectivesPage.Instance.CloseEditor();
            Tools.Animations.FadeOut(this, .25d, null);

            EditorHub.HubInstance.CallOffPriorityAlert();
            EditorHub.HubInstance.ShowNavigatorPanel();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!PropertyTextBoxesFilled() && doneButtonVisible)
            {
                doneButtonVisible = false;
                Tools.Animations.FadeOut(DoneButton, .25d, DisableDoneButton);
            }
            else if (PropertyTextBoxesFilled() && !doneButtonVisible)
            {
                doneButtonVisible = true;
                Tools.Animations.FadeIn(DoneButton, .25d, EnableDoneButton);
            }
        }

    }
}
