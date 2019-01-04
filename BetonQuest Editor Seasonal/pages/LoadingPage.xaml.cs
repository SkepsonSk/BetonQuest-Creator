using BetonQuest_Editor_Seasonal.logic.gcreator.presentation;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
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
        }

        // -------- Event-Based Operations --------

        private void BeginLoadingProject()
        {
            Editor.InitiateProject(path);
            new QuestDataLoader(path);
            Editor.LoadGCE(path);

            ConversationsPreparator.Navigate(new ConversationsPage());
            EventsPreparator.Navigate(new EventsPage());
            ConditionsPreparator.Navigate(new ConditionsPage());
            ObjectivesPreparator.Navigate(new ObjectivesPage());
            JournalPreparator.Navigate(new JournalPage());
            ItemsPreparator.Navigate(new ItemsPage());
            StatementsPreparator.Navigate(new StatementEditor());

            MainWindow.Instance.Navigate(new EditorHub());
        }

    }
}
