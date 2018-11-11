using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using BetonQuest_Editor_Seasonal.logic.yaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BetonQuest_Editor_Seasonal
{
    public class Tools
    {
        public static class PropertyListManagement
        {

            public delegate void IteratingAction(object[] data);

            private static bool ListIsEmpty(StackPanel panel)
            {
                if (panel.Children.Count == 1 && (panel.Children[0] as View).Data == null) return true;
                return false;
            }
            
            public static void AddEmptyView(StackPanel panel, string message)
            {
                View view = new View(message, null, false, false);
                view.Background = new SolidColorBrush(Colors.Transparent);
                view.Head.HorizontalAlignment = HorizontalAlignment.Center;
                view.Head.FontSize = 20;

                view.Dispatcher.Invoke(new Action(() => { panel.Children.Add(view); }));
            }

            public static void AddToPropertiesList(Property property, StackPanel panel, MouseButtonEventHandler handler, bool specialButtonVisible = false, string specialButtonText = "NONE", RoutedEventHandler specialButtonClick = null, IteratingAction iteratingAction = null)
            {
                if (ListIsEmpty(panel)) panel.Children.Clear();

                View view = new View(property.ID, property.Command, new object[] { property, false }, true, true, specialButtonVisible, specialButtonText, specialButtonClick);
                view.Margin = new Thickness(0d, 0d, 0d, 2.5d);
                view.Cursor = Cursors.Hand;

                if (iteratingAction != null) new IteratingAction(iteratingAction).Invoke(new object[] { view, property });
                if (handler != null) view.MouseDown += handler;

                panel.Children.Add(view);
            }

            public static void LoadPropertiesToList(StackPanel panel, List<Property> properties, MouseButtonEventHandler handler, bool specialButtonVisible = false, string specialButtonText = "NONE", RoutedEventHandler specialButtonClick = null, IteratingAction iteratingAction = null)
            {
                foreach (Property property in properties)
                {
                    AddToPropertiesList(property, panel, handler, specialButtonVisible, specialButtonText, specialButtonClick, iteratingAction);
                }
            }

            public static void RemoveFromPropertiesList(Property property, StackPanel panel, List<Property> properties, string message)
            {
                int index = -1;
                for (int n = 0; n < panel.Children.Count; n++)
                {
                    View view = panel.Children[n] as View;
                    Property viewProperty = view.Data[0] as Property;

                    if (viewProperty.Equals(property)) index = n;
                }

                panel.Children.RemoveAt(index);

                if (properties.Count == 0) AddEmptyView(panel, message);
            }

            public static void ReloadPropertyView(Property property, StackPanel panel)
            {
                for (int n = 0; n < panel.Children.Count; n++)
                {
                    View view = panel.Children[n] as View;
                    
                    if (view.Data[0].Equals(property))
                    {
                        view.Head.Text = property.ID;
                        view.Body.Text = property.Command;

                        return;
                    }
                    
                }
            }
        }

        public static class Animations
        {

            public static void FadeIn(UIElement control, double duration, EventHandler completed)
            {
                control.Opacity = 0d;

                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0d;
                animation.To = 1d;

                animation.Duration = TimeSpan.FromSeconds(duration);

                if (completed != null) animation.Completed += completed;

                control.BeginAnimation(UIElement.OpacityProperty, animation);
            }

            public static void FadeOut(UIElement control, double duration, EventHandler completed)
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 1d;
                animation.To = 0d;

                animation.Duration = TimeSpan.FromSeconds(duration);

                if (completed != null) animation.Completed += completed;

                control.BeginAnimation(UIElement.OpacityProperty, animation);
            }

            // -------- Height --------

            public static void SlideDown(FrameworkElement control, double height, double duration, EventHandler completed)
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0d;
                animation.To = height;
                animation.Duration = TimeSpan.FromSeconds(duration);

                if (completed != null) animation.Completed += completed;

                control.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }

            public static void SlideUp(FrameworkElement control, double height, double duration, EventHandler completed)
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = height;
                animation.To = 0d;
                animation.Duration = TimeSpan.FromSeconds(duration);

                if (completed != null) animation.Completed += completed;

                control.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }

            public static void Slide(FrameworkElement element, double fromHeigt, double toHeight, double duration, EventHandler completed)
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = fromHeigt;
                animation.To = toHeight;
                animation.Duration = TimeSpan.FromSeconds(duration);

                if (completed != null) animation.Completed += completed;

                element.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }

            // -------- Width --------

            public static void SlideRight(FrameworkElement control, double width, double duration, EventHandler completed)
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0d;
                animation.To = width;
                animation.Duration = TimeSpan.FromSeconds(duration);

                if (completed != null) animation.Completed += completed;

                control.BeginAnimation(FrameworkElement.WidthProperty, animation);
            }

            public static void SlideLeft(FrameworkElement control, double width, double duration, EventHandler completed)
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = width;
                animation.To = 0d;
                animation.Duration = TimeSpan.FromSeconds(duration);

                if (completed != null) animation.Completed += completed;

                control.BeginAnimation(FrameworkElement.WidthProperty, animation);
            }

            public static void BackgroundColorAnimation(Panel control, Color to, double duration, bool autoReverse)
            {
                SolidColorBrush brush = new SolidColorBrush();

                ColorAnimation animation = new ColorAnimation();
                animation.From = ((SolidColorBrush)control.Background).Color;
                animation.To = to;
                animation.Duration = TimeSpan.FromSeconds(duration);
                animation.AutoReverse = autoReverse;

                brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                control.Background = brush;
            }

        }

        public static class Communication
        {

            private static HttpClient client = new HttpClient();

            public static async Task<string> Communicate(Dictionary<string, string> values, string url)
            {
                MultipartFormDataContent form = new MultipartFormDataContent();

                if (values != null)
                {
                    foreach (KeyValuePair<string, string> entry in values) form.Add(new StringContent(entry.Value), entry.Key);
                }
              
                HttpResponseMessage response = await client.PostAsync(url, form);

                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }

            public static async Task<string> UploadProjectToFTP(Quest quest, string user, string password)
            {

                MultipartFormDataContent form = new MultipartFormDataContent();

                HttpResponseMessage response = await client.PostAsync("http://avatarserv.pl:8125", form);

                response.EnsureSuccessStatusCode();

                form.Dispose();

                return await response.Content.ReadAsStringAsync();
            }

            public static async Task<string> UploadProject(string path, string name, string id, bool delete = true)
            {
                if (ServerSession.CurrentSession == null) return null;
                if (string.IsNullOrEmpty(ServerSession.CurrentSession.Username) || string.IsNullOrEmpty(ServerSession.CurrentSession.AuthorizationKey)) return null;

                MultipartFormDataContent form = new MultipartFormDataContent();

                byte[] file = File.ReadAllBytes(path);

                form.Add(new StringContent(ServerSession.CurrentSession.UserID), "userid");
                form.Add(new StringContent(ServerSession.CurrentSession.AuthorizationKey), "authkey");
                form.Add(new StringContent(id), "projectid");
                form.Add(new StringContent(name), "projectname");
                form.Add(new ByteArrayContent(file), "project", "project");

                HttpResponseMessage response = await client.PostAsync("http://localhost/bqe_online/project_services/uploadproject_service.php", form);

                response.EnsureSuccessStatusCode();

                form.Dispose();

                if (delete) File.Delete(path);

                return response.Content.ReadAsStringAsync().Result;
            }

        }

        public static void CompressProject(Quest quest, string zipPath, bool directoryInside = false)
        {
            new QuestDataSaver(Project.Quest, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor\temp\" + quest.Name, true);

            string projectPath = Project.ApplicationDirectory + @"\temp\" + quest.Name;

            if (File.Exists(zipPath)) File.Delete(zipPath);

            ZipFile.CreateFromDirectory(projectPath, zipPath);
            
        }

        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories()) CopyDirectory(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles()) file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

        public static void EmptyFolder(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                if (subfolder.GetFiles().Length == 0) subfolder.Delete();
                else EmptyFolder(subfolder);
            }

            if (directoryInfo.GetFiles().Length == 0) directoryInfo.Delete();
        }

        public static bool ProjectImportPathFine(string directory)
        {
            if (Directory.GetFiles(directory).Length < 8 && Directory.GetDirectories(directory).Length == 1) return true;
            else return false;
        }

        public static string ListToString(List<Property> list)
        {
            StringBuilder newString = new StringBuilder();

            for (int n = 0; n < list.Count; n++)
            {
                if (n == list.Count - 1) newString.Append(list[n].ID + ",");
                else newString.Append(list[n].ID);
            }

            return newString.ToString();
        }

        public static string ListToString(List<Statement> list)
        {
            StringBuilder newString = new StringBuilder();

            for (int n = 0; n < list.Count; n++)
            {
                if (n == list.Count - 1) newString.Append(list[n].ID + ",");
                else newString.Append(list[n].ID);
            }

            return newString.ToString();
        }

        public static string GenerateID(int length = 24)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder();

            for (int n = 0; n < length; n++)
            {
                if (random.Next(0, 2) == 0)
                {
                    builder.Append((char)('a' + random.Next(0, 26)));
                }
                else
                {
                    builder.Append(random.Next(0, 9));
                }
            }

            return builder.ToString();
        }

    }
}
