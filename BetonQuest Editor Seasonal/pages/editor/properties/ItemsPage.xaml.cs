using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.controls.mini;
using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.control;
using BetonQuest_Editor_Seasonal.logic.settings;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.items;
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

namespace BetonQuest_Editor_Seasonal.pages.editor.properties
{
    public partial class ItemsPage : Page, ControllablePage
    {

        private static ItemsPage instance;

        // ----

        private Property item;
        private bool editing = false;

        private bool doneButtonVisible = false;

        // -------- Initializator --------

        public ItemsPage()
        {
            InitializeComponent();

            instance = this;
            
            foreach (EnchantSet enchantSet in EnchantPack.EnchantSets)
            {
                Enchants.Children.Add(new EnchantView(enchantSet));
            }

            if (Project.Quest.Items.Count == 0) Tools.PropertyListManagement.AddEmptyView(Properties, "No items created!");
            else Tools.PropertyListManagement.LoadPropertiesToList(Properties, Project.Quest.Items, PropertyView_Interact);

            ItemsTitle.Text = "Created items (" + Project.Quest.Items.Count + "):";

            ControlManager.RegisteredPages.Add(this);
        }

        // -------- Access --------

        public static ItemsPage Instance {
            get {
                return instance;
            }
        }

        // -------- Managing with the Editing Operation --------

        public void OpenItemEditing(Property property)
        {
            this.item = property;
            Item item = new Item(property);

            editing = true;

            Editing.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Visible;
            Editing.Text = "Editing '" + property.ID + "'";
            Cancel.IsEnabled = true;

            if (!string.IsNullOrEmpty(property.ID)) ID.Text = property.ID;
            Data.Text = item.Data.ToString();

            if (!string.IsNullOrEmpty(item.Type)) Type.Text = item.Type;
            if (!string.IsNullOrEmpty(item.CustomName)) CustomName.Document = new FlowDocument(new Paragraph(new Run(item.CustomName)));
            if (!string.IsNullOrEmpty(item.Lore)) Lore.Document = new FlowDocument(new Paragraph(new Run(item.Lore)));
            if (item.Enchants != null && item.Enchants.Count > 0)
            {
                EnchantView enchantView;
                foreach (SingleEnchant singleEnchant in item.Enchants)
                {
                    enchantView = GetEnchantView(singleEnchant);
                    if (enchantView == null) continue;

                    enchantView.Selected = true;
                    enchantView.SelectedLevel = singleEnchant.Level;

                    enchantView.Refresh();
                }
            }
        }

        public void CloseItemEditing()
        {
            item = null;

            editing = false;

            Editing.Visibility = Visibility.Hidden;
            Cancel.Visibility = Visibility.Hidden;
            Cancel.IsEnabled = false;

            ClearEditor();
        }

        public void ClearEditor()
        {
            ID.Text = string.Empty;
            Data.Text = "0";

            Type.Text = string.Empty;
            CustomName.Document = new FlowDocument(new Paragraph(new Run(string.Empty)));
            Lore.Document = new FlowDocument(new Paragraph(new Run(string.Empty)));

            EnchantView enchantView;
            foreach (UIElement uiElement in Enchants.Children)
            {
                if (!(uiElement is EnchantView)) continue;
                enchantView = uiElement as EnchantView;

                if (enchantView.Selected)
                {
                    enchantView.Selected = false;
                    enchantView.SelectedLevel = 1;

                    enchantView.Refresh();
                }
            }

            EditorHub.HubInstance.CallOffPriorityAlert();
        }

        // -------- Access --------

        private EnchantView GetEnchantView(SingleEnchant singleEnchant)
        {
            EnchantView enchantView;
            foreach (UIElement uiElement in Enchants.Children)
            {
                if (!(uiElement is EnchantView)) continue;
                enchantView = uiElement as EnchantView;

                if (enchantView.EnchantSet.ExportName.Equals(singleEnchant.ExportName)) return enchantView;
            }
            return null;
        }

        private bool HasSelectedEnchants()
        {
            EnchantView enchantView;
            foreach (UIElement uiElement in Enchants.Children)
            {
                if (!(uiElement is EnchantView)) continue;
                enchantView = uiElement as EnchantView;

                if (enchantView.Selected) return true;
            }
            return false;
        }

        private int SelectedEnchants {
            get {

                int selected = 0;
                EnchantView enchantView;

                foreach (UIElement uiElement in Enchants.Children)
                {
                    if (!(uiElement is EnchantView)) continue;
                    enchantView = uiElement as EnchantView;

                    if (enchantView.Selected) selected++;
                }

                return selected;
            }
        }

        private string GenerateItem()
        {
            StringBuilder command = new StringBuilder();
            command.Append(Type.Text.ToUpper() + " ");
            command.Append("data:" + Data.Text + " ");

            string customName = new TextRange(CustomName.Document.ContentStart, CustomName.Document.ContentEnd).Text.Replace(Environment.NewLine, string.Empty);
            string lore = new TextRange(Lore.Document.ContentStart, Lore.Document.ContentEnd).Text.Replace(Environment.NewLine, string.Empty);

            if (!string.IsNullOrEmpty(customName)) command.Append("name:" + customName + " ");
            if (!string.IsNullOrEmpty(lore)) command.Append("lore:" + lore + " ");

            if (HasSelectedEnchants())
            {
                int selectedEnchants = SelectedEnchants;
                int index = 1;

                command.Append("enchants:");

                for (int n = 0; n < Enchants.Children.Count; n++)
                {
                    UIElement control = Enchants.Children[n];
                    if (!(control is EnchantView)) continue;
                    EnchantView enchantView = control as EnchantView;

                    if (enchantView.Selected)
                    {
                        command.Append(enchantView.EnchantSet.ExportName + ":" + enchantView.SelectedLevel);
                        Console.WriteLine(index + " < " + selectedEnchants);
                        if (index < selectedEnchants)
                        {
                            command.Append(',');
                            index++;
                        }

                    }
                }
            }

            return command.ToString();
        }

        // -------- Events --------

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(ID.Text) || string.IsNullOrEmpty(Data.Text) || string.IsNullOrEmpty(Type.Text))
            {
                if (doneButtonVisible)
                {
                    doneButtonVisible = false;
                    Tools.Animations.FadeOut(DoneButton, .25d, null);
                    DoneButton.IsEnabled = false;
                }
            }
            else
            {
                if (!doneButtonVisible)
                {
                    doneButtonVisible = true;
                    Tools.Animations.FadeIn(DoneButton, .25d, null);
                    DoneButton.IsEnabled = true;
                }
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ID.Text))
            {
                EditorHub.HubInstance.Alert("Item's ID cannot be empty!", AlertType.Error);
                return;
            }

            if (editing)
            {
                if (Project.Quest.GetItem(ID.Text, new string[] { item.ID } ) != null)
                {
                    EditorHub.HubInstance.Alert("An item with such name already exists!", AlertType.Error);
                    return;
                }
                else
                {
                    Project.QuestUndoOperations.Push(new QuestDataImage(Project.Quest));

                    item.ID = ID.Text;
                    item.Command = GenerateItem();

                    Tools.PropertyListManagement.ReloadPropertyView(item, Properties);
                    CloseItemEditing();
                }
            }
            else
            {
                if (Project.Quest.GetItem(ID.Text) != null)
                {
                    EditorHub.HubInstance.Alert("An item with such name already exists!", AlertType.Error);
                    return;
                }
                else
                {
                    Project.QuestUndoOperations.Push(new QuestDataImage(Project.Quest));

                    Property item = new Property(ID.Text, GenerateItem());

                    Project.Quest.Items.Add(item);

                    ItemsTitle.Text = "Created items (" + Project.Quest.Items.Count + "):";

                    Tools.PropertyListManagement.AddToPropertiesList(item, Properties, PropertyView_Interact);
                    EditorHub.HubInstance.CallOffPriorityAlert();
                }
            }
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseItemEditing();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToHorizontalOffset(scv.HorizontalOffset + e.Delta);
            e.Handled = true;
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
                    Project.Quest.Items.Remove(property);
                    Tools.PropertyListManagement.RemoveFromPropertiesList(property, Properties, Project.Quest.Items, "No items created!");
                    CloseItemEditing();

                    ItemsTitle.Text = "Created items (" + Project.Quest.Items.Count + "):";
                }
                else
                {
                    OpenItemEditing(property);
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

    }
}
