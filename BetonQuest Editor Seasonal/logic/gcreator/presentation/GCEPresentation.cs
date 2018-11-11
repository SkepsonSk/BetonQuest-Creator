using BetonQuest_Editor_Seasonal.controls.gcreator;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BetonQuest_Editor_Seasonal.logic.gcreator.presentation
{
    [Serializable]
    public class GCEPresentation
    {
        private static GraphicalConversationEditor graphicalConversationEditor;
        public static Canvas Workspace { get; set; }

        // ----

        public List<GSPresentation> Statements { get; }
        public List<GPPresentation> Properties { get; }

        public List<PanelConnectionPresentation> PanelConnections { get; }

        // -------- Start --------

        public GCEPresentation(GraphicalConversationEditor graphicalConversationEditor)
        {
            GCEPresentation.graphicalConversationEditor = graphicalConversationEditor;
            Workspace = graphicalConversationEditor.Workspace;

            Statements = new List<GSPresentation>();
            Properties = new List<GPPresentation>();

            PanelConnections = new List<PanelConnectionPresentation>();

            HookProperties();
            HookConnections();
        }

        // -------- Access --------

        private void HookProperties()
        {   
            foreach (UIElement element in Workspace.Children)
            {
                if (element is GStatement)
                {
                    GStatement gStatement = element as GStatement;

                    GSPresentation statement = new GSPresentation(gStatement);
                    statement.PointPresentation = new PointPresentation( Canvas.GetLeft(element as GStatement), Canvas.GetTop(element as GStatement) );

                    Statements.Add(statement);
                }
                else if (element is GProperty)
                {
                    GProperty gProperty = element as GProperty;
                    GPPresentation property = new GPPresentation(gProperty);

                    property.PointPresentation = new PointPresentation(Canvas.GetLeft(element as GProperty), Canvas.GetTop(element as GProperty));

                    Properties.Add(property);
                }
            }
        }

        private void HookConnections()
        {
            PanelConnections.Clear();

            foreach (PanelConnection connection in graphicalConversationEditor.Connections)
            {
                GSPresentation firstStatement = GetStatementPresentation(connection.First as GStatement);

                if (connection.Second is GStatement) PanelConnections.Add(new PanelConnectionPresentation(firstStatement, GetStatementPresentation(connection.Second as GStatement)));
                else PanelConnections.Add(new PanelConnectionPresentation(firstStatement, GetPropertyPresentation(connection.Second as GProperty)));

            }

            /*foreach (UIElement element in workspace.Children)
            {

                if (element is GStatement)
                {
                    GStatement gStatement = element as GStatement;
                    GSPresentation statementPresentation = GetStatementPresentation(gStatement);

                    foreach (PanelConnection panelConnection in gStatement.GetPanelConnections())
                    {
                        if (panelConnection.Second is GStatement) PanelConnections.Add(new PanelConnectionPresentation(statementPresentation, GetStatementPresentation(panelConnection.Second as GStatement)));
                        else if (panelConnection.Second is GProperty) PanelConnections.Add(new PanelConnectionPresentation(statementPresentation, GetPropertyPresentation(panelConnection.Second as GProperty)));
                    }

                }

            }*/
        }
        
        // ----

        public List<PanelConnectionPresentation> GetConnections(PanelPresentation panelPresentation)
        {

            List<PanelConnectionPresentation> connections = new List<PanelConnectionPresentation>();

            foreach (PanelConnectionPresentation connectionPresentation in PanelConnections)
            {
                if (connectionPresentation.First.Equals(panelPresentation)) connections.Add(connectionPresentation);
            }

            return connections;
        }

        public GSPresentation GetStatementPresentation(GStatement gStatement)
        {
            double gStatementX = Canvas.GetLeft(gStatement);
            double gStatementY = Canvas.GetTop(gStatement);

            foreach (GSPresentation presentation in Statements)
            {
                if (presentation.PointPresentation.X == gStatementX && presentation.PointPresentation.Y == gStatementY) return presentation;
            }

            return null;
        }

        public GPPresentation GetPropertyPresentation(GProperty gProperty)
        {
            double gPropertyX = Canvas.GetLeft(gProperty);
            double gPropertyY = Canvas.GetTop(gProperty);

            foreach (GPPresentation presentation in Properties)
            {
                if (presentation.PointPresentation.X == gPropertyX && presentation.PointPresentation.Y == gPropertyY) return presentation;
            }

            return null;
        }

        public GStatement GetGStatement(GSPresentation statementPresentation)
        {
            double statementPresentationX = statementPresentation.PointPresentation.X;
            double statementPresentationY = statementPresentation.PointPresentation.Y;

            foreach (UIElement element in Workspace.Children)
            {
                if (element is GStatement)
                {
                    if (Canvas.GetLeft(element) == statementPresentationX && Canvas.GetTop(element) == statementPresentationY) return element as GStatement;
                }
            }

            return null;
        }

        public GProperty GetGProperty(GPPresentation propertyPresentation)
        {
            double propertyPresentationX = propertyPresentation.PointPresentation.X;
            double propertyPresentationY = propertyPresentation.PointPresentation.Y;

            foreach (UIElement element in Workspace.Children)
            {
                if (element is GProperty)
                {
                    if (Canvas.GetLeft(element) == propertyPresentationX && Canvas.GetTop(element) == propertyPresentationY) return element as GProperty;
                }
            }

            return null;
        }

    }
}
