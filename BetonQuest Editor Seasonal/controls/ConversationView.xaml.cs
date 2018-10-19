using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using BetonQuest_Editor_Seasonal.pages.editor.properties;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations;
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
    public partial class ConversationView : UserControl
    {

        private Conversation conversation;
        
        // -------- Initializator --------

        public ConversationView(Conversation conversation)
        {
            InitializeComponent();

            this.conversation = conversation;

            ConversationData.Text = conversation.NPCName + " (" + conversation.NPCID + ")";
        }

        // -------- Events --------

        private void Grid_MouseEnter(object sender, MouseEventArgs e) 
        {
            Background = new SolidColorBrush(Color.FromRgb(166, 166, 166));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = new SolidColorBrush( Color.FromRgb(191, 191, 191) );
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConversationsPage.Instance.OpenEditor(new ConversationEditor(conversation));
        }
    }
}
