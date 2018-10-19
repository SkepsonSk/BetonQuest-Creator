using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.colors;
using BetonQuest_Editor_Seasonal.logic.control;
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

namespace BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors
{
    public partial class JournalPage : Page, ControllablePage
    {

        private static JournalPage instance;

        private Property entry;
        private bool editing = false;

        private bool doneButtonVisible = false;

        private ColoredText coloredText;

        // -------- Initializator --------

        public JournalPage()
        {
            InitializeComponent();

            instance = this;

            RefreshCreator();

            if (Project.Quest.JournalEntries.Count == 0) Tools.PropertyListManagement.AddEmptyView(Properties, "No journal entries created!");
            else Tools.PropertyListManagement.LoadPropertiesToList(Properties, Project.Quest.JournalEntries, PropertyView_Interact);

            JournalEntriesTitle.Text = "Created journal entries (" + Project.Quest.JournalEntries.Count + "):";

            ControlManager.RegisteredPages.Add(this);
        }

        // ----

        public void RefreshCreator()
        {
            if (entry == null) return;
            if (!string.IsNullOrEmpty(entry.ID)) ID.Text = entry.ID;
            if (!string.IsNullOrEmpty(entry.Command)) Content.Document = new FlowDocument(new Paragraph(new Run(entry.Command)));
        }

        // -------- Editing Operations Managing --------

        public void OpenEntryEditing(Property entry)
        {
            this.entry = entry;
            editing = true;

            Editing.Visibility = Visibility.Visible;
            Editing.Text = "Editing '" + entry.ID + "'";

            Cancel.Visibility = Visibility.Visible;
            Cancel.IsEnabled = true;

            RefreshCreator();
        }

        public void CloseEntryEditing()
        {
            entry = null;
            editing = false;

            Editing.Visibility = Visibility.Hidden;

            Cancel.Visibility = Visibility.Hidden;
            Cancel.IsEnabled = false;

            ID.Text = string.Empty;
            Content.Document = new FlowDocument(new Paragraph(new Run(string.Empty)));
        }

        // -------- Instance --------

        public static JournalPage Instance {
            get {
                return instance;
            }
        }

        // -------- Events --------

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

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (!editing)
            {
                if (Project.Quest.GetJournalEntry(ID.Text) != null)
                {
                    EditorHub.HubInstance.Alert("A journal entry with such name already exists!", AlertType.Error);
                    return;
                }

                Project.QuestUndoOperations.Push(new QuestDataImage(Project.Quest));

                Property entry = new Property(ID.Text, new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd).Text.Replace(Environment.NewLine, string.Empty));

                Project.Quest.JournalEntries.Add(entry);
                Tools.PropertyListManagement.AddToPropertiesList(entry, Properties, PropertyView_Interact);

                JournalEntriesTitle.Text = "Created journal entries (" + Project.Quest.JournalEntries.Count + "):";

                EditorHub.HubInstance.CallOffPriorityAlert();
            }
            else
            {

                if (Project.Quest.GetJournalEntry(ID.Text, new string[] { entry.ID }) != null)
                {
                    EditorHub.HubInstance.Alert("A journal entry with such name already exists!", AlertType.Error);
                    return;
                }

                Project.QuestUndoOperations.Push(new QuestDataImage(Project.Quest));

                entry.ID = ID.Text;
                entry.Command = new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd).Text.Replace(Environment.NewLine, string.Empty);

                Tools.PropertyListManagement.ReloadPropertyView(entry, Properties);
                CloseEntryEditing();

                EditorHub.HubInstance.CallOffPriorityAlert();
            }

        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseEntryEditing();
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
                    Project.Quest.JournalEntries.Remove(property);
                    Tools.PropertyListManagement.RemoveFromPropertiesList(property, Properties, Project.Quest.JournalEntries, "No journal entries created!");

                    CloseEntryEditing();
                    JournalEntriesTitle.Text = "Created journal entries (" + Project.Quest.JournalEntries.Count + "):";
                }
                else
                {
                    OpenEntryEditing(property);
                }
            }
        }

        // -------- Controllable Page --------

        public void ReloadProperties()
        {
            Properties.Children.Clear();

            if (Project.Quest.Events.Count == 0) Tools.PropertyListManagement.AddEmptyView(Properties, "No events created!");
            else Tools.PropertyListManagement.LoadPropertiesToList(Properties, Project.Quest.Events, PropertyView_Interact);
        }

        // --------

        private void ColorSelect(Brush color)
        {
            TextRange range = new TextRange(Content.Selection.Start, Content.Selection.End);
            TextRange document = new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd);

            TextRange start = new TextRange(Content.Document.ContentStart, Content.Selection.Start);
            TextRange end = new TextRange(Content.Document.ContentStart, Content.Selection.End);

            if (!Content.Selection.IsEmpty)
            {
                range.ApplyPropertyValue(TextElement.ForegroundProperty, color);

                /* if (coloredText == null) coloredText = new ColoredText(document.Text);
                 else coloredText.SetText(document.Text);

                 coloredText.Color(start.Text.Length, end.Text.Length, color);

                 range.ApplyPropertyValue(TextElement.ForegroundProperty, color);

                 MessageBox.Show(coloredText.ToString());*/
            }
        }

        private void ColorAtPosition(int start, int end, Brush color)
        {
            TextRange range = new TextRange(Content.Document.ContentStart.GetPositionAtOffset(start + 1), Content.Document.ContentStart.GetPositionAtOffset(end + 2));
            range.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        private void Content_GotFocus(object sender, RoutedEventArgs e)
        {
            MainWindow.ColorSelect += ColorSelect;
            MainWindow.Instance.ShowColorBar();
        }

        private void Content_LostFocus(object sender, RoutedEventArgs e)
        {
            MainWindow.ColorSelect -= ColorSelect;
            MainWindow.Instance.HideColorBar();

            TextRange document = new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd);
            TextPointer start = Content.Document.ContentStart;

            for (int n = 0; n < 10 ; n++)
            {
                TextRange color = new TextRange(start.GetPositionAtOffset(n), start.GetPositionAtOffset(n + 1));

                if (color.GetPropertyValue(TextElement.ForegroundProperty) is Brush)
                {
                    Brush brush = color.GetPropertyValue(TextElement.ForegroundProperty) as Brush;
                    Console.WriteLine(brush.ToString() + ": " + color.Text + " / " + n + " <> " + (n+1) );
                }
                else
                {
                    Console.WriteLine("nc");
                }
            }

        }
    }
}
