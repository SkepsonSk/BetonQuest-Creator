using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.control;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetonQuest_Editor_Seasonal.pages.editor.properties
{
    public partial class ConversationsPage : Page, ControllablePage
    {
        private static ConversationsPage instance;
        private int npcIDTextBoxValue = 0;

        private bool doneButtonVisible = false;

        private Conversation conversationCreated;

        // -------- Initializator --------

        public ConversationsPage()
        {
            InitializeComponent();

            instance = this;

            if (Project.Quest.Conversations.Count == 0) Tools.PropertyListManagement.AddEmptyView(Conversations, "No conversations created!");
            else Tools.PropertyListManagement.LoadPropertiesToList(Conversations, Project.Quest.ConversationsProperties, PropertyView_Interact);

            IDTextBox.Text = npcIDTextBoxValue.ToString();

            ConversationsTitle.Text = "Created conversations (" + Project.Quest.Conversations.Count + "):";

            ControlManager.RegisteredPages.Add(this);
        }

        private bool SpecialButtonCanBeShown(object[] data)
        {
            return true;
        }

        // -------- Editor Frame Operations --------

        public void OpenEditorNoEffect(Page page)
        {
            Editor.Visibility = Visibility.Visible;
            Editor.Navigate(page);
        }

        public void OpenEditor(Page page)
        {
            Editor.Visibility = Visibility.Visible;
            Editor.Navigate(page);

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0d;
            animation.To = 10d;
            animation.Duration = TimeSpan.FromSeconds(0.5d);

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

            BlurEffect effect = (BlurEffect)LeftPanel.Effect;

            animation.Completed += HideEditorFrame;
            effect.BeginAnimation(BlurEffect.RadiusProperty, animation);

            LeftPanel.Effect = effect;
            RightPanel.Effect = effect;

            Tools.Animations.FadeOut(ConversationsPage.Instance.Editor, .25d, null);
            EditorHub.HubInstance.ShowNavigatorPanel();
        }

        private void HideEditorFrame(object sender, EventArgs e)
        {
            Editor.Visibility = Visibility.Hidden;
        }

        // -------- Instance --------

        public static ConversationsPage Instance {
            get {
                return instance;
            }
        }

        // -------- Events --------

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Project.Quest.GetConversation(NameTextBox.Text, int.Parse(IDTextBox.Text)) != null)
            {
                EditorHub.HubInstance.Alert("A conversation with such name already exists!", AlertType.Error);
                return;
            }

            conversationCreated = new Conversation(NameTextBox.Text, int.Parse(IDTextBox.Text));
            Project.Quest.Conversations.Add(conversationCreated);

            Tools.PropertyListManagement.AddToPropertiesList(conversationCreated, Conversations, PropertyView_Interact, true, "GRAPH", SpecialButton_Click, null);

            ConversationsTitle.Text = "Created conversations (" + Project.Quest.Conversations.Count + "):";

            GraphicalConversationEditor graphicalConversationEditor = new GraphicalConversationEditor(conversationCreated);
            conversationCreated.GraphicalConversationEditor = graphicalConversationEditor;

            MainWindow.Instance.DisplayFrame.Navigate(graphicalConversationEditor);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text) || string.IsNullOrEmpty(IDTextBox.Text))
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

        private void IDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(IDTextBox.Text)) return;

            int result;
            if (int.TryParse(IDTextBox.Text, out result))
            {
                npcIDTextBoxValue = result;
 
            }
            else
            {
                IDTextBox.Text = npcIDTextBoxValue.ToString();
            }
        }

        private void IDTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                npcIDTextBoxValue++;
                IDTextBox.Text = npcIDTextBoxValue.ToString();
            }
            else if (e.Key == Key.Down)
            {
                IDTextBox.Text = npcIDTextBoxValue.ToString();
                npcIDTextBoxValue--;
            }
        }

        private void SpecialButton_Click(object sender, EventArgs e)
        {
            Button specialButton = sender as Button;
            View view = specialButton.Tag as View;
            Conversation conversation = view.Data[0] as Conversation;

            if (conversation.GraphicalConversationEditor == null) return;

            conversation.GraphicalConversationEditor.InvokeWorkspace();
            MainWindow.Instance.DisplayFrame.Navigate(conversation.GraphicalConversationEditor);
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
                Conversation conversation = view.Data[0] as Property as Conversation;

                if (removingMode)
                {
                    Project.Quest.Conversations.Remove(conversation);
                    Tools.PropertyListManagement.RemoveFromPropertiesList(conversation, Conversations, Project.Quest.ConversationsProperties, "No conversations created!");

                    ConversationsTitle.Text = "Created conversations (" + Project.Quest.Conversations.Count + "):";
                }
                else
                {
                    OpenEditor(new ConversationEditor(conversation));
                }
            }
        }

        // -------- Controllable Page --------

        public void ReloadProperties()
        {
            Conversations.Children.Clear();

            if (Project.Quest.Conversations.Count == 0) Tools.PropertyListManagement.AddEmptyView(Conversations, "No conversations created!");
            else Tools.PropertyListManagement.LoadPropertiesToList(Conversations, Project.Quest.ConversationsProperties, PropertyView_Interact);
        }
    }
}
