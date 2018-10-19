using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using BetonQuest_Editor_Seasonal.pages.editor;
using BetonQuest_Editor_Seasonal.pages.editor.properties;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conditions;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.events;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.objectives;
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

namespace BetonQuest_Editor_Seasonal.controls
{

    public partial class PropertyView : UserControl
    {

        private Property property;
        private PropertyType type;

        private object data;

        // -------- Initializator --------

        public PropertyView(Property property, PropertyType type, object data = null)
        {
            InitializeComponent();

            this.property = property;
            this.type = type;
            this.data = data;

            ID.Text = property.ID;
            Value.Text = property.Command;
        }

        // -------- Events

        private void Click(object sender, MouseButtonEventArgs e)
        {
            if (type == PropertyType.NPCStatement) ConversationsPage.Instance.OpenEditorNoEffect(StatementEditor.Open(property as Statement, true, true));
            else if (type == PropertyType.PlayerStatement) ConversationsPage.Instance.OpenEditorNoEffect(StatementEditor.Open(property as Statement, false, true));

            else if (type == PropertyType.Event) EventsPage.Instance.OpenEditor(new EventDefaultEditor(property, true));
            else if (type == PropertyType.Condition) ConditionsPage.Instance.OpenEditor(new ConditionDefaultEditor(property, true));
            else if (type == PropertyType.Objective) ObjectivesPage.Instance.OpenEditor(new ObjectiveDefaultEditor(property, true));
            else if (type == PropertyType.Journal) JournalPage.Instance.OpenEntryEditing(property);
            else if (type == PropertyType.Item) ItemsPage.Instance.OpenItemEditing(property);

        }

    }
}
