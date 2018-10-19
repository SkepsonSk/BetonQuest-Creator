﻿using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.online;
using BetonQuest_Editor_Seasonal.logic.yaml;
using BetonQuest_Editor_Seasonal.pages.editor;
using BetonQuest_Editor_Seasonal.pages.online;
using BetonQuest_Editor_Seasonal.pages.online.market;
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
        private string pathToLoad;

        // -------- Initializator --------

        public WelcomePage()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Project.LastEditedProjectName))
            {
                LastProject.Visibility = Visibility.Visible;
                LastProjectName.Text = Project.LastEditedProjectName;
            }

            if (ServerSession.CurrentSession != null && !string.IsNullOrEmpty(ServerSession.CurrentSession.AuthorizationKey) && !string.IsNullOrEmpty(ServerSession.CurrentSession.Username))
            {
                OnlineButton.Visibility = Visibility.Collapsed;

                LoggedInUser.Visibility = Visibility.Visible;
                LoggedInUser.Text = ServerSession.CurrentSession.Username;

                OfflineButton.Visibility = Visibility.Visible;

                MarketButton.Visibility = Visibility.Visible;
            }
            else
            {
                OnlineButton.Visibility = Visibility.Visible;
                OfflineButton.Visibility = Visibility.Collapsed;
                LoggedInUser.Visibility = Visibility.Collapsed;        
                MarketButton.Visibility = Visibility.Collapsed;
            }

            if (Project.UpdateManager == null) Project.UpdateManager = new UpdateManager(20d, OnUpdateFound);

            if (Project.UpdateManager.Downloading)
            {
                UpdateVersion.Text = "Downloading of version " + UpdateManager.NewestVersion + "...";

                DirectDownload.Visibility = Visibility.Collapsed;
                WebsiteDownload.Visibility = Visibility.Collapsed;
            }
            else if (UpdateManager.UpdateAvailable)
            {
                UpdateVersion.Text = "Version " + UpdateManager.NewestVersion + " is ready to download";
                UpdatePanel.Opacity = 1d;
            }

            Tools.Animations.FadeIn(Page, .4d, null);
        }

        // -------- Event Handling --------

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new NewProjectPage());
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select the 'main.yml' file";

            if (dialog.ShowDialog() == true)
            {
                pathToLoad = dialog.FileName;
                MainWindow.Instance.Navigate(new LoadingPage(pathToLoad));
            }
        }

        private void MarketButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.Animations.FadeOut(this, .25d, SwitchToMarketPage);
        }

        private void LastProjectName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pathToLoad = Project.LastEditedProjectPath + @"\main.yml";
            MainWindow.Instance.Navigate(new LoadingPage(pathToLoad));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new SettingsHub());
        }

        // ---------

        private void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.Animations.FadeOut(Page, .25d, SwitchToAuthorizationPage);
        }

        private void OfflineButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServerSession.CurrentSession == null) return;

            OnlineButton.Content = "Online";
            OfflineButton.Visibility = Visibility.Hidden;
            OfflineButton.IsEnabled = false;

            MarketButton.Visibility = Visibility.Collapsed;
        }

        // --------

        private void SwitchToMarketPage(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayFrame.Navigate(new MarketMainPage());
        }

        private void SwitchToAuthorizationPage(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayFrame.Navigate(new LoginPage());
        }

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

        // --------

        private void DirectUpdateDownload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!CommonFileDialog.IsPlatformSupported)
            {
                MessageBox.Show("Cannot open file dialog! Please use webpage download option.");
                return;
            }

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                Console.WriteLine(dialog.FileName);
                Project.UpdateManager.DownloadUpdate(dialog.FileName, OnUpdateDownloadStart, OnUpdateDownloaded);
            }
        }

        private void GoToWebsite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://avatarserv.pl/bqe?version=" + Application.Current.FindResource("Version").ToString());
        }
        
        // --------
        
        private void OnUpdateFound()
        {
            if (UpdatePanel.Opacity > 0) return;

            UpdateVersion.Text = "Version " + UpdateManager.NewestVersion + " is ready to download";

            Tools.Animations.FadeIn(UpdatePanel, .25d, null);
            UpdatePanel.IsEnabled = true;
        }

        private void OnUpdateDownloadStart()
        {
            if (UpdatePanel.Opacity < 1) Tools.Animations.FadeIn(UpdatePanel, .25d, null);

            UpdateVersion.Text = "Downloading version " + UpdateManager.NewestVersion + "...";

            DirectDownload.Visibility = Visibility.Collapsed;
            WebsiteDownload.Visibility = Visibility.Collapsed;
        }

        private void OnUpdateDownloaded()
        {
            if (UpdatePanel.Opacity < 1) Tools.Animations.FadeIn(UpdatePanel, .25d, null);

            UpdateVersion.Text = "Version " + UpdateManager.NewestVersion + " has been downloaded!";

            DirectDownload.Visibility = Visibility.Collapsed;
            WebsiteDownload.Visibility = Visibility.Collapsed;
        }

        //test
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.ShowColorBar();
        }
    }
}