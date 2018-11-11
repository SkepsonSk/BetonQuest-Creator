using BetonQuest_Editor_Seasonal.controls.gcreator;
using BetonQuest_Editor_Seasonal.logic.gcreator;
using BetonQuest_Editor_Seasonal.logic.gcreator.presentation;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

    public partial class GraphicalConversationEditor : Page
    {
        private Point point;

        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;

        private Control movedControl;
        private Point startPt;
        private Point lastLoc;

        // ---- Connections 

        private bool creatingConnection = false;
        private GStatement selected;

        public List<PanelConnection> Connections { get; }

        // ---- Data ----

        private Conversation conversation;

        // -------- Start --------

        public GraphicalConversationEditor(Conversation conversation = null)
        {
            InitializeComponent();

            if (conversation == null)
            {
                this.conversation = new Conversation("test", 1906);
                Project.Quest.Conversations.Add(conversation);
            }
            else this.conversation = conversation;

            ScrollViewer.ScrollToVerticalOffset( Workspace.Height / 2 );
            ScrollViewer.ScrollToHorizontalOffset( Workspace.Width / 2 );

            PanelConnection.SetWorkspace(Workspace);
            Connections = new List<PanelConnection>();
        }

        public GraphicalConversationEditor(Conversation conversation, GCEPresentation presentation)
        {
            InitializeComponent();

            this.conversation = conversation;
            Connections = new List<PanelConnection>();
            PanelConnection.SetWorkspace(Workspace);
            GCEPresentation.Workspace = Workspace;

            LoadPresentation(presentation);

            ScrollViewer.ScrollToVerticalOffset(Workspace.Height / 2);
            ScrollViewer.ScrollToHorizontalOffset(Workspace.Width / 2);

            WelcomeText.Visibility = Visibility.Collapsed;
        }

        // ----

        public void InvokeWorkspace() { PanelConnection.SetWorkspace(Workspace); }

        private void Workspace_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                point = e.GetPosition(Workspace);

                if (WelcomeText.Visibility == Visibility.Visible) Tools.Animations.FadeOut(WelcomeText, .5d, HideWelcomeText);
            }
        }

        // --------

        private void ShowInformation(string content, string buttonContent, RoutedEventHandler buttonClick)
        {
            InformationContent.Text = content;
            InformationButton.Content = buttonContent;
            InformationButton.Click += buttonClick;

            if (TopInformationBar.Height == 0) Tools.Animations.SlideDown(TopInformationBar, 30d, .25d, null);
        }

        private void HideTopInformationBar()
        {
            if (TopInformationBar.Height != 0) Tools.Animations.SlideUp(TopInformationBar, 30d, .25d, null);
        }

        // --------

        private void AddStatementPanel(Point point, StatementType statementType, bool editConversation = true, Statement statement = null)
        {
            GStatement gStatement = new GStatement(statementType, statement);
            gStatement.CreateConnectionItem.Click += CreateConnectionItem_Click;

            gStatement.StartItem.Click += StartStatementItem_Click;
            gStatement.DeleteItem.Click += DeleteItem_Click;

            if (editConversation)
            {
                if (statementType == StatementType.Player) conversation.PlayerStatements.Add(gStatement.GetBoundProperty() as Statement);
                else conversation.NPCStatements.Add(gStatement.GetBoundProperty() as Statement);
            }

            if (statementType == StatementType.NPC) {

                gStatement.StartPosition.LostFocus += StartPosition_LostFocus;

                if (statement != null && conversation.StartStatements.Contains(statement))
                {
                    gStatement.SetBorderBrush = Brushes.Orange;
                    gStatement.Border.BorderBrush = Brushes.Orange;

                    gStatement.StartItem.Header = "Remove from start statements";
                }
            }

            Panel.SetZIndex(gStatement, 10);

            gStatement.MouseDown += Control_MouseDown;
            gStatement.MouseDown += Control_Connection_MouseDown;

            gStatement.MouseMove += Control_MouseMove;
            gStatement.MouseUp += Control_MouseUp;

            Canvas.SetTop(gStatement, point.Y);
            Canvas.SetLeft(gStatement, point.X);

            Workspace.Children.Add(gStatement);
        }

        private void AddPropertyPanel(Point point, PropertyType type, Property property = null)
        {
            GProperty gProperty = new GProperty(type, property);

            Panel.SetZIndex(gProperty, 10);

            gProperty.DeleteItem.Click += DeleteItem_Click;

            gProperty.MouseDown += Control_MouseDown;
            gProperty.MouseDown += Control_Connection_MouseDown;

            gProperty.MouseMove += Control_MouseMove;
            gProperty.MouseUp += Control_MouseUp;

            Canvas.SetTop(gProperty, point.Y);
            Canvas.SetLeft(gProperty, point.X);

            Workspace.Children.Add(gProperty);
        }

        // -------- Scaling the workspace --------

        private void Workspace_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta < 0 )
            {
                ScalingUnit.ScaleX += 0.01d;
                ScalingUnit.ScaleY += 0.01d;

                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ContentVerticalOffset + 50);
                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.ContentHorizontalOffset + 50);
            }
            else
            {
                ScalingUnit.ScaleX -= 0.01d;
                ScalingUnit.ScaleY -= 0.01d;

                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ContentVerticalOffset - 50);
                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.ContentHorizontalOffset - 50);
            }

        }

        // -------- Moving --------

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Control)) return;

            movedControl = sender as Control;
            movedControl.Cursor = Cursors.SizeAll;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startPt = e.GetPosition(Workspace);
                lastLoc = new Point(Canvas.GetLeft(movedControl), Canvas.GetTop(movedControl));
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (movedControl == null) return;

            var newX = (startPt.X + (e.GetPosition(Workspace).X - startPt.X));
            var newY = (startPt.Y + (e.GetPosition(Workspace).Y - startPt.Y));

            Point offset = new Point((startPt.X - lastLoc.X), (startPt.Y - lastLoc.Y));

            double CanvasTop = newY - offset.Y;
            double CanvasLeft = newX - offset.X;

            foreach (PanelConnection connection in Connections)
            {
                if (connection.First.Equals(movedControl) || connection.Second.Equals(movedControl)) connection.Update();
            }

            movedControl.SetValue(Canvas.TopProperty, CanvasTop);
            movedControl.SetValue(Canvas.LeftProperty, CanvasLeft);
        }

        private void Control_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (movedControl == null) return;

            movedControl.Cursor = Cursors.Arrow;
            movedControl = null;
        }

        // --------

        private void CreatePlayerStatement_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddStatementPanel(point, StatementType.Player);
        }

        private void CreateNPCStatement_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddStatementPanel(point, StatementType.NPC);
        }

        private void CreateCondition_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddPropertyPanel(point, PropertyType.Condition);
        }

        private void CreateEvent_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddPropertyPanel(point, PropertyType.Event);
        }

        // -------- Navigation over Workspace --------

        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (UIElement element in Workspace.Children)
            {
                if (element.IsMouseOver) return;
            }

            scrollMousePoint = e.GetPosition(ScrollViewer);
            hOff = ScrollViewer.HorizontalOffset;
            vOff = ScrollViewer.VerticalOffset;
            ScrollViewer.CaptureMouse();
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ScrollViewer.IsMouseCaptured)
            {
                ScrollViewer.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(ScrollViewer).X));
                ScrollViewer.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(ScrollViewer).Y));
            }
        }

        private void ScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer.ReleaseMouseCapture();
        }

        // -------- Creating connections --------

        private void CreateConnectionItem_Click(object sender, RoutedEventArgs e)
        {
            if (creatingConnection) return;
            creatingConnection = true;

            Control control = (sender as MenuItem).Tag as Control;

            if (!(control is GStatement))
            {
                Console.WriteLine("Cannot create connection from object other than GStatement!");
                return;
            }

            selected = control as GStatement;
            ShowInformation("Creating connection with element: " + selected.GetBoundProperty().ID, "Cancel", CancelConnection_Button_Click);
        }

        private void Control_Connection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!creatingConnection || selected == null) return;

            Control connecting = sender as Control;

            IPanel selectedPanel = sender as IPanel;
            IPanel connectingPanel = connecting as IPanel;

            PanelConnection panelConnection = null;

            if (connecting is GStatement)
            {
                GStatement connectingGStatement = connecting as GStatement;
                panelConnection = new PanelConnection(selected, connecting, true);

                if (selected.StatementType == connectingGStatement.StatementType)
                {
                    Console.WriteLine("Cannot connect two statements with same type!");

                    creatingConnection = false;
                    selected = null;

                    return;
                }

                (selected.GetBoundProperty() as Statement).NextStatements.Add(connectingGStatement.GetBoundProperty() as Statement);

            } 
            else if (connecting is GProperty)
            {
                GProperty connectingGProperty = connecting as GProperty;

                if (connectingGProperty.GetBoundProperty() == null)
                {
                    Console.WriteLine("GProperty property is not defined!");

                    creatingConnection = false;
                    selected = null;

                    return;
                }

                panelConnection = new PanelConnection(selected, connecting, false);
                Statement statement = selected.GetBoundProperty() as Statement;
                Property property = connectingGProperty.GetBoundProperty() as Property;

                if (connectingGProperty.Type == PropertyType.Condition)
                {
                    statement.Conditions.Add(property);
                    if (connectingGProperty.Negated) statement.NegatedConditions.Add(property.ID);
                }
                else if (connectingGProperty.Type == PropertyType.Event) statement.Events.Add(property);

            }
            Connections.Add(panelConnection);

            creatingConnection = false;
            selected = null;

            HideTopInformationBar();
        }

        private void CancelConnection_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!creatingConnection) return;

            selected = null;
            creatingConnection = false;

            HideTopInformationBar();
        }

        // -------- Deleting and Breaking Connections --------

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            IPanel iPanel = (sender as MenuItem).Tag as IPanel;

            bool removedIsStatement = iPanel is GStatement;

            GProperty gProperty = null;
            if (!removedIsStatement)
            {
                gProperty = iPanel as GProperty;

                for (int n = Connections.Count - 1; n >= 0; n--)
                {
                    if (gProperty.Type == PropertyType.Condition)
                    {
                        ((Connections[n].First as GStatement).GetBoundProperty() as Statement).Conditions.Remove(gProperty.GetBoundProperty());
                        ((Connections[n].First as GStatement).GetBoundProperty() as Statement).NegatedConditions.Remove(gProperty.GetBoundProperty().ID);
                    }
                    else ((Connections[n].First as GStatement).GetBoundProperty() as Statement).Events.Remove(gProperty.GetBoundProperty());

                    if (Connections[n].First.Equals(iPanel) || Connections[n].Second.Equals(iPanel))
                    {
                        Connections[n].Delete();
                        Connections.RemoveAt(n);
                    }
                }
            }
            else
            {
                for (int n = Connections.Count - 1; n >= 0; n--)
                {
                    if (Connections[n].First.Equals(iPanel) || Connections[n].Second.Equals(iPanel))
                    {
                        Connections[n].Delete();
                        Connections.RemoveAt(n);
                    }
                }

                conversation.BreakConnectionsWithStatement((iPanel.GetBoundProperty() as Statement));
                conversation.RemoveStatement(iPanel.GetBoundProperty() as Statement);

                UpdateStartStatementsSequence();
            }

            Workspace.Children.Remove(iPanel as Control);
        }

        // -------- Start Statements --------

        private void StartStatementItem_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as MenuItem).Tag is GStatement)) return;
 
            GStatement gStatement = (sender as MenuItem).Tag as GStatement;  

            if (conversation.StartStatements.Contains(gStatement.GetBoundProperty()))
            {
                Console.WriteLine("Is start!");

                gStatement.StartItem.Header = "Add to start statements";
                gStatement.StartPosition.Visibility = Visibility.Collapsed;

                gStatement.SetBorderBrush = Brushes.Transparent;
                gStatement.Border.BorderBrush = Brushes.Transparent;

                conversation.StartStatements.Remove(gStatement.GetBoundProperty() as Statement);

                UpdateStartStatementsSequence();
            }
            else
            {
                Console.WriteLine("Is not!");

                gStatement.StartItem.Header = "Remove from start statements";
                gStatement.StartPosition.Visibility = Visibility.Visible;  

                gStatement.SetBorderBrush = Brushes.Orange;
                gStatement.Border.BorderBrush = Brushes.Orange;

                conversation.StartStatements.Add(gStatement.GetBoundProperty() as Statement);

                UpdateStartStatementsSequence();
            }
        }

        private void StartPosition_LostFocus(object sender, RoutedEventArgs e)
        {
            int newPosition = int.Parse((sender as TextBox).Text) - 1;

            if (newPosition + 1 > conversation.StartStatements.Count)
            {
                UpdateStartStatementsSequence();
                return;
            }

            Statement movedStatement = ((sender as TextBox).Tag as GStatement).GetBoundProperty() as Statement;

            int movedPosition = -1;

            for (int n = 0; n < conversation.StartStatements.Count; n++)
            {
                if (conversation.StartStatements[n].ID == movedStatement.ID) movedPosition = n;
            }

            Statement statementAtNewPosition = conversation.StartStatements[newPosition];

            conversation.StartStatements[newPosition] = movedStatement;
            conversation.StartStatements[movedPosition] = statementAtNewPosition;

            UpdateStartStatementsSequence();
        }

        public void UpdateStartStatementsSequence()
        {
            for (int n = 0; n < conversation.StartStatements.Count; n++) GetGStatement(conversation.StartStatements[n]).StartPosition.Text = (n + 1).ToString();
        }

        // ---- Test purposes only ----

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) MainWindow.Instance.Navigate(EditorHub.HubInstance);
        }

        // --------

        private void HideWelcomeText(object sender, EventArgs e)
        {
            WelcomeText.Visibility = Visibility.Collapsed;
        }

        // -------- 

        public GStatement GetGStatement(Statement statement)
        {
            foreach (UIElement control in Workspace.Children)
            {
                if (control is GStatement)
                {
                    GStatement gStatement = control as GStatement;

                    if (gStatement.GetBoundProperty() == statement) return gStatement;
                }
            }
            return null;
        }

        public void LoadPresentation(GCEPresentation presentation)
        {
            Workspace.Children.Clear();
            Connections.Clear();

            Statement statement;
            Property property;

            foreach (GSPresentation statementPresentation in presentation.Statements)
            {
                statement = Project.Quest.GetStatement(conversation, statementPresentation.Type, statementPresentation.StatementID);
                AddStatementPanel(statementPresentation.PointPresentation.ToPoint(), statementPresentation.Type, false, statement);
            }
            foreach (GPPresentation propertyPresentation in presentation.Properties)
            {
                property = Project.Quest.GetProperty(propertyPresentation.PropertyType, propertyPresentation.PropertyID);
                AddPropertyPanel(propertyPresentation.PointPresentation.ToPoint(), propertyPresentation.PropertyType, property);
            }

            foreach (PanelConnectionPresentation connection in presentation.PanelConnections)
            {
                GStatement first = presentation.GetGStatement(connection.First as GSPresentation);

                if (connection.Second is GSPresentation) Connections.Add(new PanelConnection(first, presentation.GetGStatement(connection.Second as GSPresentation), true));
                else Connections.Add(new PanelConnection(first, presentation.GetGProperty(connection.Second as GPPresentation)));
            }
        }

    }
}
