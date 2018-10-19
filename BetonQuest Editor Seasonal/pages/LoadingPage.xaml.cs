using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.yaml;
using BetonQuest_Editor_Seasonal.pages.editor;
using BetonQuest_Editor_Seasonal.pages.editor.properties;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace BetonQuest_Editor_Seasonal.pages
{
    public partial class LoadingPage : Page
    {
        private string path;

        // -------- Initializator --------

        public LoadingPage(string path)
        {
            InitializeComponent();
            this.path = path;

            MainWindow.Instance.NavigatorFree += BeginLoadingProject;

            BeginLoadingProject();
        }

        // -------- Event-Based Operations --------

        private void BeginLoadingProject()
        {
            new QuestDataLoader(path);

            ConversationsPreparator.Navigate(new ConversationsPage());
            EventsPreparator.Navigate(new EventsPage());
            ConditionsPreparator.Navigate(new ConditionsPage());
            ObjectivesPreparator.Navigate(new ObjectivesPage());
            JournalPreparator.Navigate(new JournalPage());
            ItemsPreparator.Navigate(new ItemsPage());
            StatementsPreparator.Navigate(new StatementEditor());

            if (ServerSession.CurrentSession != null && !string.IsNullOrEmpty(ServerSession.CurrentSession.AuthorizationKey)) CheckOnline();
            else
            {
                Console.WriteLine("loading");
                MainWindow.Instance.Navigate(new EditorHub());
            }
        }

        private async void CheckOnline()
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request["userid"] = ServerSession.CurrentSession.UserID;
            request["authkey"] = ServerSession.CurrentSession.AuthorizationKey;
            request["projectid"] = Project.Quest.ID;

            string json = await Tools.Communication.Communicate(request, "http://localhost/bqe_online/project_services/locateproject_service.php");
            Console.WriteLine(json);
            Project.Quest.Online = bool.Parse((string) JObject.Parse(json)["message"]);

            MainWindow.Instance.Navigate(new EditorHub());
        }

    }
}
