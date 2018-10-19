using BetonQuest_Editor_Seasonal.controls.online;
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

namespace BetonQuest_Editor_Seasonal.pages.online.market.subpages
{
    public partial class QuestsPage : Page
    {

        public QuestsPage()
        {
            InitializeComponent();
            RefreshProjects();
        }

        public async void RefreshProjects()
        {
            Projects.Children.Clear();
            List<logic.online.Project> projects = await ServerSession.CurrentSession.GetUserProjects();

            foreach (logic.online.Project project in projects)
            {
                Projects.Children.Add(new OnlineProject(project.Name, project.Description, project.CreatorName));
            }
        }

    }
}
