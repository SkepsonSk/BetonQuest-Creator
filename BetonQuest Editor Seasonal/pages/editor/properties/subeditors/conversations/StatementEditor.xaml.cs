using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.controls.mini;
using BetonQuest_Editor_Seasonal.logic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations
{
    public partial class StatementEditor : Page
    {

        private static StatementEditor instance;

        // --------

        private bool editing;

        private Conversation conversation;
        private Statement statement;

        private bool npc;
        private bool start = false;

        private bool doneButtonVisible = false;

        // -------- Initializator --------

        public StatementEditor()
        {
            InitializeComponent();

            LoadEvents();
            LoadConditions();

            instance = this;
        }

        public static StatementEditor Open(Statement statement, bool npc, bool editing = false)
        {
            instance.editing = editing;

            instance.statement = statement;

            instance.npc = npc;
            if (!npc)
            {
                instance.IsStart.Visibility = Visibility.Hidden;
                instance.IsStart.IsEnabled = false;
                instance.Type.Text = "Player statements";
            }
            else instance.Type.Text = "NPC statement";

            if (editing)
            {
                instance.Editing.Visibility = Visibility.Visible;
                instance.Editing.Text = "Editing '" + statement.ID + "'!";
            }

            instance.RefreshNameAndContent();
            instance.ReloadEvents(false);
            instance.ReloadConditions(false);

            instance.LoadNextStatements();

            return instance;
        }

        // -------- Loading and Reloading --------

        public void LoadConditions()
        {
            Conditions.Children.Clear();
            Console.WriteLine("Loading conditions...");

            if (Project.Quest.Conditions.Count == 0) Conditions.Children.Add(new NoPropertyView("None"));
            else
            {
                foreach (Property property in Project.Quest.Conditions)
                {
                    //bool selected = statement.Conditions.Contains(property);
                    //bool negated = statement.NegatedConditions.Contains(property.ID);

                    Conditions.Children.Add(new PropertyNegableSelectingView(property));
                }
            }
        }

        public void LoadEvents()
        {
            Events.Children.Clear();
            Console.WriteLine("Loading events...");

            if (Project.Quest.Events.Count == 0) Events.Children.Add(new NoPropertyView("None"));
            else
            {
                foreach (Property property in Project.Quest.Events)
                {
                    Events.Children.Add(new PropertySelectingView(property));
                }
            }
        }

        public void LoadNextStatements()
        {
            NextStatements.Children.Clear();
            Console.WriteLine("Loading next statements connections...");

            List<Statement> statements;
            if (npc) statements = conversation.PlayerStatements;
            else statements = conversation.NPCStatements;

            if (statements.Count == 0) NextStatements.Children.Add(new NoPropertyView("None"));
            else
            {
                foreach (Statement statement in statements)
                {
                    PropertySelectingView view = new PropertySelectingView(statement, this.statement.NextStatements.Contains(statement));
                    NextStatements.Children.Add(view);
                }
            }
        }

        public void RefreshNameAndContent()
        {
            if (!string.IsNullOrEmpty(statement.ID))
            {
                ID.Text = statement.ID;

                if (npc && conversation.GetStartStatement(statement.ID) != null)
                {
                    start = true;
                    IsStart.Background = new SolidColorBrush(Color.FromRgb(255, 153, 51));
                }
                else
                {
                    start = false;
                    IsStart.Background = new SolidColorBrush(Color.FromRgb(179, 179, 179));
                }

            }

            if (!string.IsNullOrEmpty(statement.Content)) Content.Document = new FlowDocument(new Paragraph(new Run(statement.Content)));
        }

        // ----

        public void ReloadEvents(bool reset)
        {
            if (Events.Children.Count == 1 && Events.Children[0] is NoPropertyView) return;

            if (reset)
            {
                foreach (UIElement uiElement in Events.Children)
                {
                    PropertySelectingView view = uiElement as PropertySelectingView;
                    if (view.Selected)
                    {
                        view.Selected = false;
                        view.Refresh();
                    }
                }
            }
            else
            {
                foreach (UIElement uiElement in Events.Children)
                {
                    PropertySelectingView view = uiElement as PropertySelectingView;
                    view.Selected = statement.Events.Contains(view.Property);
                    view.Refresh();
                }
            }
        }

        public void ReloadConditions(bool reset)
        {
            if (Conditions.Children.Count == 1 && Events.Children[0] is NoPropertyView) return;

            if (reset)
            {
                foreach (UIElement uiElement in Conditions.Children)
                {
                    PropertyNegableSelectingView view = uiElement as PropertyNegableSelectingView;
                    if (view.Selected)
                    {
                        if (view.Negated) view.Negated = false;
                        view.Selected = false;                 
                        view.Refresh();
                    }
                }
            }
            else
            {
                foreach (UIElement uiElement in Conditions.Children)
                {
                    PropertyNegableSelectingView view = uiElement as PropertyNegableSelectingView;
                    view.Selected = statement.Conditions.Contains(view.Property);
                    view.Negated = statement.NegatedConditions.Contains(view.Property.ID);
                    view.Refresh();
                }
            }
        }

        // -------- Access --------

        public Conversation Conversation {
            get {
                return conversation;
            }
            set {
                conversation = value;
            }
        }

        public static StatementEditor Instance {
            get {
                return instance;
            }
        }

        // -------- Events --------

        private void ReturnTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConversationsPage.Instance.OpenEditorNoEffect(new ConversationEditor(conversation, false));

            ReloadEvents(true);
            ReloadConditions(true);
        }

        private void CloseTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConversationsPage.Instance.CloseEditor();

            ReloadEvents(true);
            ReloadConditions(true);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(ID.Text) || string.IsNullOrEmpty(new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd).Text.Replace(Environment.NewLine, string.Empty)))
            {
                if (!doneButtonVisible) return;
                Tools.Animations.FadeOut(DoneButton, .25d, null);

                doneButtonVisible = false;
                DoneButton.IsEnabled = false;
            }
            else
            {
                if (doneButtonVisible) return;
                Tools.Animations.FadeIn(DoneButton, .25d, null);

                doneButtonVisible = true;
                DoneButton.IsEnabled = true;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!start)
            {
                start = true;
                IsStart.Background = new SolidColorBrush(Color.FromRgb(255, 153, 51));
            }
            else
            {
                start = false;
                IsStart.Background = new SolidColorBrush(Color.FromRgb(179, 179, 179));
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {

            if (!editing)
            {
                if (npc && conversation.GetNPCStatement(ID.Text) != null)
                {
                    EditorHub.HubInstance.Alert("An NPC statement with such name already exists!", AlertType.Error);
                    return;
                }
                else if (!npc && conversation.GetPlayerStatement(ID.Text) != null)
                {
                    EditorHub.HubInstance.Alert("A Player statement with such name already exists!", AlertType.Error);
                    return;
                }
            }
            else
            {
                if (npc && conversation.GetNPCStatement(ID.Text, new string[] { statement.ID }) != null)
                {
                    EditorHub.HubInstance.Alert("An NPC statement with such name already exists!", AlertType.Error);
                    return;
                }
                else if (!npc && conversation.GetPlayerStatement(ID.Text, new string[] { statement.ID }) != null)
                {
                    EditorHub.HubInstance.Alert("A Player statement with such name already exists!", AlertType.Error);
                    return;
                }
            }

            statement.ID = ID.Text;
            statement.Content = new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd).Text.Replace(Environment.NewLine, string.Empty);

            statement.Events.Clear();
            foreach (Control control in Events.Children)
            {
                if (!(control is PropertySelectingView)) continue;

                PropertySelectingView view = control as PropertySelectingView;
                if (view.Selected) statement.Events.Add(view.Property);
            }

            statement.Conditions.Clear();
            statement.NegatedConditions.Clear();
            foreach (Control control in Conditions.Children)
            {
                if (!(control is PropertyNegableSelectingView)) continue;

                PropertyNegableSelectingView view = control as PropertyNegableSelectingView;
                if (view.Selected) statement.Conditions.Add(view.Property);
                if (view.Negated) statement.NegatedConditions.Add(view.Property.ID);
            }

            statement.NextStatements.Clear();
            foreach(Control control in NextStatements.Children)
            {
                if (!(control is PropertySelectingView)) continue;

                PropertySelectingView view = control as PropertySelectingView;
                if (view.Selected) statement.NextStatements.Add( view.Property as Statement );
            }

            if (npc)
            {
                if (!conversation.NPCStatements.Contains(statement)) conversation.NPCStatements.Add(statement);

                if (start && !conversation.StartStatements.Contains(statement)) conversation.StartStatements.Add(statement);
                else if (!start && conversation.StartStatements.Contains(statement)) conversation.StartStatements.Remove(statement);

                ConversationEditor.Instance.RefreshNPCStatements();
                ConversationEditor.Instance.RefreshStartStatements();
            }
            else
            {
                if (!conversation.PlayerStatements.Contains(statement)) conversation.PlayerStatements.Add(statement);
                ConversationEditor.Instance.RefreshPlayerStatements();
            }

            Tools.Animations.BackgroundColorAnimation(Panel, Colors.ForestGreen, .25d, true);
            EditorHub.HubInstance.CallOffPriorityAlert();
        }

    }
}
