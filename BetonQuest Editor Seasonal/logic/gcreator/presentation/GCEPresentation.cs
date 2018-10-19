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
    public class GCEPresentation
    {
        private static GraphicalConversationEditor graphicalConversationEditor;

        // ----

        private List<GPPresentation> properties;

        // -------- Start --------

        public GCEPresentation(GraphicalConversationEditor graphicalConversationEditor)
        {
            GCEPresentation.graphicalConversationEditor = graphicalConversationEditor;

            Statements = new List<GSPresentation>();
            properties = new List<GPPresentation>();

            HookStatements();
        }

        // -------- Access --------

        public List<GSPresentation> Statements { get;  }

        // ----

        private void HookStatements()
        {
            Canvas workspace = graphicalConversationEditor.Workspace;

            foreach (UIElement element in workspace.Children)
            {

                if (element is GStatement)
                {
                    GSPresentation statement = new GSPresentation(element as GStatement);
                    statement.PointPresentation = new PointPresentation( Canvas.GetLeft(element as GStatement), Canvas.GetTop(element as GStatement) );

                    Statements.Add(statement);
                }

            }
        }
    }
}
