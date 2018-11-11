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

        public StatementType StatementType { get; }
        public Brush SetBorderBrush { get; set; }

        // -------- Start --------

        public GStatement(StatementType statementType, Statement statement = null, bool start = false)
        {
            InitializeComponent();

            StatementType = statementType;

            StartItem.Tag = this;
            CreateConnectionItem.Tag = this;
            DeleteItem.Tag = this;
            StartPosition.Tag = this;

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

            SetBorderBrush = Brushes.Transparent;

            if (statementType == StatementType.Player)
            {
                StartPosition.Visibility = Visibility.Collapsed;
                StartItem.Visibility = Visibility.Collapsed;
                Title.Text = "Player Statement";
            }
            else if (start)
            {
                StartItem.Header = "Remove from start statements";
                SetBorderBrush = Brushes.Orange;
            }

            
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


        // -------- Events --------

        private void Content_LostFocus(object sender, RoutedEventArgs e)
        {
            statement.Content = Content.Text;
        }

        private void ID_LostFocus(object sender, RoutedEventArgs e)
        {
            statement.ID = ID.Text;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Border.BorderBrush = Brushes.Gray;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Border.BorderBrush = SetBorderBrush;
        }
    }
}
