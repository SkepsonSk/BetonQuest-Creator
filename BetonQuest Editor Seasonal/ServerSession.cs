using BetonQuest_Editor_Seasonal.logic.online;
using BetonQuest_Editor_Seasonal.logic.yaml;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace BetonQuest_Editor_Seasonal
{
    public class ServerSession
    {

        private static ServerSession currentSession;
        public static bool ServerUp = false;

        // --------

        private static DispatcherTimer timer = new DispatcherTimer();

        public string Username;
        public string UserID;
        public string AuthorizationKey;

        // --------

        public delegate void PostConnect();

        // -------- Initilization --------

        public ServerSession(TextBlock responseHandler = null)
        {
            currentSession = this;

            timer.Interval = TimeSpan.FromSeconds(1d);
            timer.Start();

            Console.WriteLine("Created new session!");
        }

        // -------- Access --------

        public static ServerSession CurrentSession {
            get {
                return currentSession;
            }
        }

        public static DispatcherTimer Timer {
            get {
                return timer;
            }
        }

        // -------- Testing --------

        // -------- Communication --------

        public async void Register(string username, string password, string repeatPassword, string mail, PostConnect registerOk, PostConnect registerFailed)
        {
            if (password != repeatPassword) return;

            Dictionary<string, string> request = new Dictionary<string, string>();
            request["user"] = username;
            request["pass"] = password;
            request["reppass"] = repeatPassword;
            request["mail"] = mail;

            string json = await Tools.Communication.Communicate(request, "http://localhost/bqe_online/register_service.php");
            Console.WriteLine(json);
            JObject result = JObject.Parse(json);

            if (result["userid"] != null && result["authkey"] != null)
            {
                Username = username;
                UserID = (string)result["userid"];
                AuthorizationKey = (string)result["authkey"];

                registerOk.Invoke();
            }
            else registerFailed.Invoke();
        }

        public async void Login(string username, string password, PostConnect loginOk, PostConnect loginFailed)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request["user"] = username;
            request["pass"] = password;

            string json = await Tools.Communication.Communicate(request, "http://localhost/bqe_online/auth_service.php");
            Console.WriteLine(json);
            JObject result = JObject.Parse(json);

            if (result["userid"] != null && result["authkey"] != null)
            {
                Username = username;
                UserID = (string) result["userid"];
                AuthorizationKey = (string) result["authkey"];

                Console.WriteLine(Username + ", " + UserID + ", " + AuthorizationKey);

                loginOk.Invoke();
            }
            else loginFailed.Invoke();
        }

        // ----

        public void Logout()
        {
            //TODO do it
        }

        public void LogoutNoRepsonse()
        {
            //TODO do it
        }

        // ----

        public async Task<List<logic.online.Project>> GetUserProjects(string projectsOwner = null)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request["userid"] = UserID;
            request["authkey"] = AuthorizationKey;

            if (projectsOwner != null) request["projectsowner"] = projectsOwner;

            string json = await Tools.Communication.Communicate(request, "http://localhost/bqe_online/project_services/listprojects_service.php");
            Console.WriteLine(json);
            JObject result = JObject.Parse(json);

            List<logic.online.Project> projects = new List<logic.online.Project>();

            foreach (JToken project in result["projects"])
            {
                projects.Add(new logic.online.Project((string)project["id"], (string)project["name"], (string)project["description"], (string)project["owner"], "skepsonsk"));
            }

            return projects;
        }

        public async Task<string> UploadCurrentProject()
        {
            new QuestDataSaver(Project.Quest, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor\temp", false);

            string projectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor\temp\" + Project.Quest.Name;
            string zipPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor\temp\" + Project.Quest.Name + ".zip";

            if (File.Exists(zipPath)) File.Delete(zipPath);

            ZipFile.CreateFromDirectory(projectPath, zipPath);

            Tools.EmptyFolder(new DirectoryInfo(projectPath));
            Console.WriteLine("Compression finished");

            string json = await Tools.Communication.UploadProject(zipPath, Project.Quest.Name, Project.Quest.ID);
            JObject respond = JObject.Parse(json);

            return (string)respond["status"];
        }

    }
}
