using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.controls.mini;
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

namespace BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations
{
    public partial class ConversationEditor : Page
    {
        private static ConversationEditor instance;

        private Conversation conversation;
        private Statement newStatement;

        // -------- Initializator --------

        public ConversationEditor(Conversation conversation, bool fadeIn = true)
        {
            InitializeComponent();

            instance = this;

            this.conversation = conversation;

            ConversationData.Text = conversation.NPCName + " (" + conversation.NPCID + ")";

            RefreshStartStatements();
            RefreshNPCStatements();
            RefreshPlayerStatements();

            StatementEditor.Instance.Conversation = conversation;

            if (fadeIn) Tools.Animations.FadeIn(ConversationsPage.Instance.Editor, .25d, null);
        }

        // -------- Refreshing --------

        public void RefreshStartStatements()
        {
            StartStatements.Children.Clear();
            if (conversation.StartStatements.Count == 0) StartStatements.Children.Add(new NoPropertyView());

            for (int n = 0; n < conversation.StartStatements.Count; n++) StartStatements.Children.Add(new StartStatementView(conversation.StartStatements[n], conversation, n));
        }

        public void RefreshNPCStatements()
        {
            NPC.Children.Clear();

            if (conversation.NPCStatements.Count == 0) NPC.Children.Add(new NoPropertyView());

            foreach (Statement statement in conversation.NPCStatements)
            {
                NPC.Children.Add(new PropertyView(statement, PropertyType.NPCStatement, conversation));
            }

        }

        public void RefreshPlayerStatements()
        {
            Player.Children.Clear();

            if (conversation.PlayerStatements.Count == 0) Player.Children.Add(new NoPropertyView());

            foreach (Statement statement in conversation.PlayerStatements)
            {
                Player.Children.Add(new PropertyView(statement, PropertyType.PlayerStatement, conversation));
            }

        }

        // -------- Instance --------

        public static ConversationEditor Instance {
            get {
                return instance;
            }
        }

        // -------- Events --------

        private void CloseTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConversationsPage.Instance.CloseEditor();
        }

        private void NPCStatementButton_Click(object sender, RoutedEventArgs e)
        {
            newStatement = new Statement(string.Empty, string.Empty);
            ConversationsPage.Instance.OpenEditorNoEffect(StatementEditor.Open(newStatement, true));
        }

        private void PlayerStatementButton_Click(object sender, RoutedEventArgs e)
        {
            newStatement = new Statement(string.Empty, string.Empty);
            ConversationsPage.Instance.OpenEditorNoEffect(StatementEditor.Open(newStatement, false));
        }
    }
}
