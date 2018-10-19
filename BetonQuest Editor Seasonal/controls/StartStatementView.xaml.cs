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

namespace BetonQuest_Editor_Seasonal.controls.mini
{
    public partial class StartStatementView : UserControl
    {
        private Statement statement;
        private Conversation conversation;

        private int index;

        // -------- Initializator --------

        public StartStatementView(Statement statement, Conversation conversation, int index)
        {
            InitializeComponent();
            ID.Text = statement.ID;

            this.statement = statement;
            this.conversation = conversation;

            this.index = index;

            Refresh();
        }

        private void Refresh()
        {
            TextBlock firstOperation = new TextBlock(), secondOperation = new TextBlock();

            firstOperation.FontSize = 30d;
            secondOperation.FontSize = 30d;
            firstOperation.Margin = new Thickness(0d, 0d, -2.5d, 0d);
            secondOperation.Margin = new Thickness(-2.5d, 0d, 0d, 0d);

            Operations.Children.Clear();

            if (index == 0)
            {
                firstOperation.Text = "⇃";
                secondOperation.Text = "⇂";

                firstOperation.Name = "Down";
                secondOperation.Name = "Down";

                firstOperation.MouseDown += MouseDown_Switch;
                secondOperation.MouseDown += MouseDown_Switch;
            }
            else if (index == conversation.StartStatements.Count - 1)
            {
                firstOperation.Text = "↿";
                secondOperation.Text = "↾";

                firstOperation.Name = "Up";
                secondOperation.Name = "Up";

                firstOperation.MouseDown += MouseDown_Switch;
                secondOperation.MouseDown += MouseDown_Switch;
            }
            else
            {
                firstOperation.Text = "↿";
                secondOperation.Text = "⇂";

                firstOperation.Name = "Up";
                secondOperation.Name = "Down";

                firstOperation.MouseDown += MouseDown_Switch;
                secondOperation.MouseDown += MouseDown_Switch;
            }

            Operations.Children.Add(firstOperation);
            Operations.Children.Add(secondOperation);
        }

        // -------- Events --------

        private void MouseDown_Switch(object sender, MouseButtonEventArgs e)
        {
            TextBlock operation = sender as TextBlock;
            bool up = operation.Name.Equals("Up");

            Statement buffor;

            if (up)
            {
                buffor = conversation.StartStatements[index - 1];

                conversation.StartStatements[index] = buffor;
                conversation.StartStatements[index - 1] = statement;
            }
            else
            {
                buffor = conversation.StartStatements[index + 1];

                conversation.StartStatements[index] = buffor;
                conversation.StartStatements[index + 1] = statement;
            }

            ConversationEditor.Instance.RefreshStartStatements();
        }

        private void MouseDown_OpenStatementEditor(object sender, MouseButtonEventArgs e)
        {
            ConversationsPage.Instance.OpenEditorNoEffect(StatementEditor.Open(statement, true, true));
        }

    }
}
