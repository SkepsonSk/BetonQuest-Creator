using BetonQuest_Editor_Seasonal.logic.gcreator;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
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

namespace BetonQuest_Editor_Seasonal.controls.gcreator
{
    public partial class GStatement : UserControl, IPanel
    {
        private List<PanelConnection> panelConnections;
        private Statement statement;

        private StatementType statementType;

        // -------- Start --------

        public GStatement(StatementType statementType, Statement statement = null)
        {
            InitializeComponent();

            this.statementType = statementType;

            CreateConnectionItem.Tag = this;
            DeleteItem.Tag = this;

            panelConnections = new List<PanelConnection>();

            string id = Tools.GenerateID(8);

            Width = 250d;
            Height = 200d;

            if (statement == null) this.statement = new Statement(id);
            else
            {
                this.statement = statement;

                Content.Text = statement.Content;
            }

            if (statementType == StatementType.Player) Title.Text = "Player Statement";
            else Title.Text = "NPC Statement";

            ID.Text = this.statement.ID;
        }

        // -------- Access --------

        public List<PanelConnection> GetPanelConnections()
        {
            return panelConnections;
        }

        public Property GetBoundProperty()
        {
            return statement;
        }

        public void BindProperty(Property property)
        {
            statement = property as Statement;
        }

        // ----

        public StatementType StatementType {
            get {
                return statementType;
            }
        }

        // -------- Events --------

        private void Content_LostFocus(object sender, RoutedEventArgs e)
        {
            statement.Content = Content.Text;
        }

        private void ID_LostFocus(object sender, RoutedEventArgs e)
        {
            statement.ID = ID.Text;
        }
    }
}
