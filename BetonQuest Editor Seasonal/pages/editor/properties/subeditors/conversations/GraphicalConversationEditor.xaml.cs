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

        // ---- Data ----

        private Conversation conversation;

        // -------- Start --------

        public GraphicalConversationEditor(Conversation conversation = null)
        {
            InitializeComponent();

            if (conversation == null)
            {
                this.conversation = new Conversation("test", 1806);
                Project.Quest.Conversations.Add(conversation);
            }
            else this.conversation = conversation;

            ScrollViewer.ScrollToVerticalOffset( Workspace.Height / 2 );
            ScrollViewer.ScrollToHorizontalOffset( Workspace.Width / 2 );

            PanelConnection.SetWorkspace(Workspace);
        }

        private void Workspace_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                point = e.GetPosition(Workspace);

                if (WelcomeText.Visibility == Visibility.Visible) Tools.Animations.FadeOut(WelcomeText, .5d, HideWelcomeText);
            }
        }

        // --------

        private void AddStatementPanel(Point point, StatementType statementType, bool editConversation = true, Statement statement = null)
        {
            GStatement gStatement = new GStatement(statementType, statement);
            gStatement.CreateConnectionItem.Click += CreateConnectionItem_Click;

            if (editConversation)
            {
                if (statementType == StatementType.Player) conversation.PlayerStatements.Add(gStatement.GetBoundProperty() as Statement);
                else conversation.NPCStatements.Add(gStatement.GetBoundProperty() as Statement);
            }

            Panel.SetZIndex(gStatement, 10);

            gStatement.MouseDown += Control_MouseDown;
            gStatement.MouseDown += Control_Connection_MouseDown;

            gStatement.MouseMove += Control_MouseMove;
            gStatement.MouseUp += Control_MouseUp;

            gStatement.Width = 250d;
            gStatement.Height = 200d;

            Canvas.SetTop(gStatement, point.Y);
            Canvas.SetLeft(gStatement, point.X);

            Workspace.Children.Add(gStatement);
        }

        private void AddPropertyPanel(Point point, PropertyType type, Property property = null)
        {
            GProperty gProperty = new GProperty(type, property);

            Panel.SetZIndex(gProperty, 10);

            gProperty.MouseDown += Control_MouseDown;
            gProperty.MouseDown += Control_Connection_MouseDown;

            gProperty.MouseMove += Control_MouseMove;
            gProperty.MouseUp += Control_MouseUp;

            Canvas.SetTop(gProperty, point.Y);
            Canvas.SetLeft(gProperty, point.X);

            Workspace.Children.Add(gProperty);
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

            if (movedControl is IPanel)
            {
                foreach (PanelConnection panelConnection in (movedControl as IPanel).GetPanelConnections()) panelConnection.Update();
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

            if (!(((MenuItem)sender).Tag is GStatement))
            {
                Console.WriteLine("Cannot create connection from object other than GStatement!");
                return;
            }

            selected = ((MenuItem)sender).Tag as GStatement;

            Console.WriteLine("Creating connection...");
        }

        private void Control_Connection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!creatingConnection || selected == null) return;
            Console.WriteLine("Connection created!");

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

            (selected as IPanel).GetPanelConnections().Add(panelConnection);
            (connecting as IPanel).GetPanelConnections().Add(panelConnection);

            creatingConnection = false;
            selected = null;
        }

        // ---- Test purposes only ----

        private GCEPresentation presentation;

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == Key.Escape) MainWindow.Instance.DisplayFrame.Navigate(ConversationsPage.Instance);
            else if (e.Key == Key.S) presentation = new GCEPresentation(this);
            else if (e.Key == Key.L) LoadPresentation(presentation);
        }

        // --------

        private void HideWelcomeText(object sender, EventArgs e)
        {
            WelcomeText.Visibility = Visibility.Collapsed;
        }

        // -------- 

        public void LoadPresentation(GCEPresentation presentation)
        {
            Workspace.Children.Clear();

            foreach (GSPresentation statement in presentation.Statements) AddStatementPanel(statement.PointPresentation.ToPoint(), statement.Type, false, statement.Statement);
            foreach (GPPresentation property in presentation.Properties) AddPropertyPanel(property.PointPresentation.ToPoint(), property.PropertyType, property.Property);

            foreach (GSPresentation statementPresentation in presentation.Statements)
            {
                List<PanelConnectionPresentation> connectionPresentations = presentation.GetConnections(statementPresentation);

                if (connectionPresentations.Count == 0) continue;

                GStatement gStatement = presentation.GetGStatement(statementPresentation);

                foreach (PanelConnectionPresentation connectionPresentation in connectionPresentations)
                {
                    Control connected = null;
                    PanelConnection connection = null;

                    if (connectionPresentation.Second is GSPresentation)
                    {
                        connected = presentation.GetGStatement(connectionPresentation.Second as GSPresentation);
                        connection = new PanelConnection(gStatement, connected, true);
                    }
                    else if (connectionPresentation.Second is GPPresentation)
                    {
                        connected = presentation.GetGProperty(connectionPresentation.Second as GPPresentation);
                        connection = new PanelConnection(gStatement, connected, false);
                    }
     
                    gStatement.GetPanelConnections().Add(connection);
                    (connected as IPanel).GetPanelConnections().Add(connection);
                }

            }

        }

    }
}
