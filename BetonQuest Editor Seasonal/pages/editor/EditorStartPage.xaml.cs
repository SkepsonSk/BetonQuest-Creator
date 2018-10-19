using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.yaml;
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
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Ionic.Zip;
using BetonQuest_Editor_Seasonal.logic.online;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BetonQuest_Editor_Seasonal.pages.editor
{
    public partial class EditorStartPage : Page
    {

        // -------- Initializator --------

        public EditorStartPage()
        {
            InitializeComponent();

            ProjectNameTextBox.Text = Project.Quest.Name;

            if (ServerSession.CurrentSession == null)
            {
                OnlineOptions.Visibility = Visibility.Collapsed;
            }
            else if (ServerSession.CurrentSession.AuthorizationKey != null)
            {
                PublicationStatus.Text = "Project is published!";
                PublishButton.Content = "Update";
                RemoveButton.Visibility = Visibility.Visible;  
            }
            else
            {
                OnlineOptions.Visibility = Visibility.Collapsed;
            }
        }

        // -------- Events --------

        private void ProjectNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Project.Quest.Name = ProjectNameTextBox.Text;
        }

        private void ProjectNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Project.Quest.Name = ProjectNameTextBox.Text;
        }

        // ----

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Project.LastEditedProjectID = Project.Quest.ID;
            Project.LastEditedProjectName = Project.Quest.Name;
            Project.LastEditedProjectPath = Project.ApplicationDirectory + @"\projects\" + Project.Quest.ID;

            Project.SaveLastEditedProject();

            new QuestDataSaver(Project.Quest, Project.ApplicationDirectory + @"\projects");
            EditorHub.HubInstance.Alert("Project has been saved!", AlertType.Success, 3);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            string exportPath;
            string message = "Projec has been EXPORTED!";

            if (!CommonFileDialog.IsPlatformSupported)
            {
                MessageBox.Show("Cannot open folder dialog! Project will be exported to the desktop.");
                exportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;

                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    exportPath = dialog.FileName;
                }
                else return;
            }

            if (Directory.Exists(exportPath + @"\" + Project.Quest.Name))
            {
                string previousDirectoryName = Project.Quest.Name + @"_" + Tools.GenerateID(5);

                DirectoryInfo previous = new DirectoryInfo(exportPath + @"\" + previousDirectoryName);
                if (!previous.Exists) previous.Create();

                Tools.CopyDirectory(new DirectoryInfo(exportPath + @"\" + Project.Quest.Name), previous);

                Tools.EmptyFolder(new DirectoryInfo(exportPath + @"\" + Project.Quest.Name));

                message += " (Previous file: " + previousDirectoryName + ")";
            }

            new QuestDataSaver(Project.Quest, exportPath, true);
            EditorHub.HubInstance.Alert(message, AlertType.Success, 3);

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WelcomePageButton_Click(object sender, RoutedEventArgs e)
        {
            Project.Quest = null;

            EventsPage.Instance = null;

            MainWindow.Instance.Navigate(new WelcomePage());
        }

        // ---- Online ----

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            //Tools.CompressProject(Project.Quest, Project.ApplicationDirectory + @"\temp\" + Project.Quest.Name + ".zip", true);
            new ProjectUpload(Project.Quest);
        }

        private async void PublishProjectButton_Click(object sender, RoutedEventArgs e)
        {
            PublishButton.Visibility = Visibility.Collapsed;
            RemoveButton.Visibility = Visibility.Collapsed;
            MarketButton.Visibility = Visibility.Collapsed;

            if (Project.Quest.Online) PublicationStatus.Text = "Project is being UPDATED...";
            else PublicationStatus.Text = "Project is being PUBLISHED...";

            string respond = await ServerSession.CurrentSession.UploadCurrentProject();

            if (respond == null) EditorHub.HubInstance.Alert("Failed to upload project!", AlertType.Error);
            else
            {
                if (respond == "published")
                {
                    EditorHub.HubInstance.Alert("Project has been published!", AlertType.Success);
                    PublicationStatus.Text = "Project has been published!";
                }
                else if (respond == "updated")
                {
                    EditorHub.HubInstance.Alert("Project has been updated!", AlertType.Success);
                    PublicationStatus.Text = "Project has been updated!";
                }

                Project.Quest.Online = true;
                PublishButton.Content = "Update";

                PublishButton.Visibility = Visibility.Visible;
                RemoveButton.Visibility = Visibility.Visible;
                MarketButton.Visibility = Visibility.Visible;

            } 

        }

    }
}
