using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.online;
using BetonQuest_Editor_Seasonal.logic.yaml;
using BetonQuest_Editor_Seasonal.pages.editor;
using BetonQuest_Editor_Seasonal.pages.management;
using BetonQuest_Editor_Seasonal.pages.settings;
using BetonQuest_Editor_Seasonal.pages.setup;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetonQuest_Editor_Seasonal.pages
{
    public partial class WelcomePage : Page
    {
        private double ProjectsSidepanelWidth = 400d;

        // -------- Initializator --------

        public WelcomePage()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Project.LastEditedProjectName))
            {
                LastProject.Visibility = Visibility.Visible;
                LastProjectName.Text = Project.LastEditedProjectName;
            }

            UpdateProjects();
            ProjectsSidepanel.Width = 0d;

            Tools.Animations.FadeIn(Page, .4d, null);
        }

        private void UpdateProjects()
        {
            string[] files = Directory.GetFiles(Project.ApplicationDirectory + @"\definitions");

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    string id = file.Substring(file.LastIndexOf(@"\") + 1);
                    id = id.Substring(0, id.LastIndexOf('.'));

                    string name = File.ReadAllText(file);
                    name = name.Substring(0, name.Length - 2);

                    View view = new View(name, new object[] { id }, true, false, false, "", null) { /* view.SetHeadFontSize(21d); */ Cursor = Cursors.Hand, Margin = new Thickness(0d, 0d, 0d, 10d) };
                    view.MouseDown += Project_MouseDown;
                    view.Head.Padding = new Thickness(10d);

                    Projects.Children.Add(view);
                }
            }
            else Tools.PropertyListManagement.AddEmptyView(Projects, "No projects!");
        }

        // -------- Event Handling --------

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new NewProjectPage());
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.Animations.SlideRight(ProjectsSidepanel, ProjectsSidepanelWidth, 0.25d, null);
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Title = "Select the 'main.yml' file." };

            if (dialog.ShowDialog() == true)
            {
                string importedPath = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf('\\'));
                MainWindow.Instance.Navigate(new LoadingPage(importedPath));
            }
        }

        private void LastProjectName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.Navigate(new LoadingPage(Project.LastEditedProjectPath));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new SettingsHub());
        }

        // ---- Projects Slidepanel ----

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tools.Animations.SlideLeft(ProjectsSidepanel, ProjectsSidepanelWidth, 0.25d, null);
        }

        private void Project_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string id = (sender as View).Data[0] as string;
            MainWindow.Instance.Navigate(new LoadingPage(Project.ApplicationDirectory + @"\projects\" + id));
        }

        // --------

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // --------

        private void Discord_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://discord.gg/pVFq2gN");
        }

        private void Facebook_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.facebook.com/avatarserv/");
        }

        private void GoToWebsite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://avatarserv.pl/bqe?version=" + Application.Current.FindResource("Version").ToString());
        }
       
        //testing purposes only
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.LoadToFloatingFrame(new ExploratorPage(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
        }

    }
}
