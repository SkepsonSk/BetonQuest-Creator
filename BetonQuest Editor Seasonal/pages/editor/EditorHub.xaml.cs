using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.control;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.yaml;
using BetonQuest_Editor_Seasonal.pages.editor.properties;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors;
using BetonQuest_Editor_Seasonal.pages.setup;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace BetonQuest_Editor_Seasonal.pages.editor
{
    public partial class EditorHub : Page
    {
        private static EditorHub hubInstance;

        private Button lastClickedButton;

        private AlertType alertType;
        private bool alertBarVisible = false;
        private bool alertBarPriorityAlert = false;
        public int AlertDuration = 0;

        private DispatcherTimer timer;

        // -------- Initializator --------

        public EditorHub()
        {
            InitializeComponent();
            hubInstance = this;

            Toolbar.Height = 0;

            NavigatorFrame.Content = new EditorStartPage();

            AlertBar.Height = 0d;

            CheckButton(Start);

            // Will be moved to MainWindow!
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1d);
            timer.Tick += Timer_Tick;
        }

        // -------- Outer Communication --------

        public static EditorHub HubInstance {
            get {
                return hubInstance;
            }
        }

        // ---- Alert System ----
        // ---- Preparing to move to MainWindow soon

        public void Alert(string text, AlertType type, int duration = 0)
        {  
            if (type == AlertType.Success && !alertBarPriorityAlert)
            {
                alertType = type;

                AlertText.Text = text;
                ActionButton.Content = "OK";
                AlertDuration = 1;
                AlertBar.Background = new LinearGradientBrush(Color.FromRgb(34, 139, 34), Color.FromRgb(30, 123, 30), new Point(0, 0), new Point(1, 0));

                timer.Start();
            }

            else if (type == AlertType.Error)
            {
                alertType = type;

                AlertText.Text = text;
                ActionButton.Content = "OK";
                AlertBar.Background = new LinearGradientBrush(Color.FromRgb(255, 51, 51), Color.FromRgb(255, 0, 0), new Point(0, 0), new Point(1, 0));

                AlertDuration = duration;

                alertBarPriorityAlert = true;

                timer.Start();
            }

            SlideUpAlert();
        }

        public void SlideUpAlert()
        {
            if (alertBarVisible) return;

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0d;
            animation.To = 30d;
            animation.Duration = TimeSpan.FromSeconds(.5d);

            alertBarVisible = true;

            AlertBar.BeginAnimation(HeightProperty, animation);
        }

        public void SlideDownAlert()
        {
            if (!alertBarVisible) return;

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 30d;
            animation.To = 0d;
            animation.Duration = TimeSpan.FromSeconds(.5d);

            alertBarVisible = false;
            alertBarPriorityAlert = false;

            AlertBar.BeginAnimation(HeightProperty, animation);
        }

        public void CallOffPriorityAlert()
        {
            if (!alertBarPriorityAlert) return;

            alertBarPriorityAlert = false;
            SlideDownAlert();
        }

        public void CallOffNonPriorityAlert()
        {
            if (!alertBarPriorityAlert) SlideDownAlert();
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (alertType == AlertType.Success) CallOffNonPriorityAlert();
            else if (alertType == AlertType.Error) CallOffPriorityAlert();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (AlertDuration == 0)
            {
                CallOffNonPriorityAlert();
                timer.Stop();
            }
            else AlertDuration--;
        }

        // -------- Menu Effects --------

        private void CheckButton(Button button)
        {
            SolidColorBrush brush = new SolidColorBrush();

            ColorAnimation animation = new ColorAnimation();
            animation.From = Color.FromRgb(191, 191, 191);
            animation.To = Color.FromRgb(255, 153, 51);
            animation.Duration = TimeSpan.FromSeconds(0.25d);

            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            button.Foreground = Brushes.White;
            button.Background = brush;
            lastClickedButton = button;
        }

        private void UncheckButton(Button button)
        {
            SolidColorBrush brush = new SolidColorBrush();

            ColorAnimation animation = new ColorAnimation();
            animation.From = Color.FromRgb(255, 153, 51);
            animation.To = Color.FromRgb(191, 191, 191);
            animation.Duration = TimeSpan.FromSeconds(0.25d);

            button.Foreground = Brushes.Gray;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            button.Background = brush;
        }

        public void HideNavigatorPanel()
        {
            Tools.Animations.SlideUp(NavigatorPanel, 30d, .25d, null);
        }

        public void ShowNavigatorPanel()
        {
            Tools.Animations.SlideDown(NavigatorPanel, 30d, .25d, null);
        }

        // -------- Events --------

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastClickedButton.Equals(sender)) return;

            Button button = sender as Button;

            if (lastClickedButton == null) CheckButton(button);
            else
            {
                UncheckButton(lastClickedButton);
                CheckButton(button);
            }

            Page page = null;
            string name =  button.Name;
            CallOffPriorityAlert();

            if (name.Equals("Start"))
            {
                page = new EditorStartPage();
            }
            else if (name.Equals("Conversations"))
            {
                page = ConversationsPage.Instance;
            }
            else if (name.Equals("Events"))
            {
                page = EventsPage.Instance;
            }
            else if (name.Equals("Conditions"))
            {
                 page = ConditionsPage.Instance;
            }
            else if (name.Equals("Objectives"))
            {
                page = ObjectivesPage.Instance;
            }
            else if (name.Equals("Journal"))
            {
                page = JournalPage.Instance;
            }
            else if (name.Equals("Items"))
            {
                page = ItemsPage.Instance;
            }

            NavigatorFrame.Navigate(page);
        }

        // -------- Toolbar --------
        // ---- Will be moved to MainWindow soon

        private void SwitchToolbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Toolbar.Height == 0d)
            {
                Tools.Animations.SlideDown(Toolbar, 40d, .25d, null);
                ToolbarPanel.Visibility = Visibility.Collapsed;
            }
            else Tools.Animations.SlideUp(Toolbar, 40d, .25d, ShowToolbarPanel);
        }

        private void ShowToolbarPanel(object sender, EventArgs e)
        {
            ToolbarPanel.Opacity = 0d;
            ToolbarPanel.Visibility = Visibility.Visible;
            Tools.Animations.FadeIn(ToolbarPanel, .25d, null);
        }

        // ---- Needs upgrade
        // ---- Tests only!

        private void UndoButton_Click(object sender, EventArgs e)
        {
            if (Project.QuestUndoOperations.Count == 0)
            {
                Alert("No more UNDO operations!", AlertType.Error, 1);
                return;
            }

            Project.QuestRedoOperations.Push(new QuestDataImage(Project.Quest));
            Project.QuestUndoOperations.Pop().Apply(Project.Quest);

            ControlManager.ReloadAllPages();

        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            if (Project.QuestRedoOperations.Count == 0)
            {
                Alert("No more REDO operations!", AlertType.Error, 1);
                return;
            }

            Project.QuestUndoOperations.Push(new QuestDataImage(Project.Quest));
            Project.QuestRedoOperations.Pop().Apply(Project.Quest);

            ControlManager.ReloadAllPages();

        }

    }
}
