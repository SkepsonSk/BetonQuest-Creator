using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BetonQuest_Editor_Seasonal.logic.online
{
    public class UpdateManager
    {
        public static bool UpdateAvailable = false;
        public static string NewestVersion = string.Empty;

        public delegate void UpdateRequestEnd();

        private DispatcherTimer timer;

        private UpdateRequestEnd updateFound;
        private UpdateRequestEnd onUpdateDownloaded;

        private bool downloading = false;
        private bool downloaded = false;

        private int downloadProgress;

        // -------- Initialization --------

        public UpdateManager(double interval, UpdateRequestEnd updateFound)
        {
            this.updateFound = updateFound;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(interval);

            timer.Tick += Tick;

            timer.Start();
        }

        // -------- Access --------

        public void DownloadUpdate(string path, UpdateRequestEnd onUpdateDownloadStart, UpdateRequestEnd onUpdateDownloaded)
        {
            this.onUpdateDownloaded = onUpdateDownloaded;

            downloading = true;

            using (WebClient client = new WebClient())
            {
                Console.WriteLine("Downloading...");

                client.DownloadFileAsync(new Uri("http://www.avatarserv.pl/bqe/download.php?version=" + NewestVersion), path + @"\BetonQuest Editor " + NewestVersion + ".exe" );
                client.DownloadFileCompleted += Client_DownloadFileCompleted;

                timer.Stop();
                onUpdateDownloadStart.Invoke();
            }
        }

        // ----

        public bool Downloading {
            get {
                return downloading;
            }
        }

        public bool Downloaded {
            get {
                return downloaded;
            }
        }

        public int DownloadProgress {
            get {
                return downloadProgress;
            }
        }

        // -------- Events --------

        private async void Tick(object sender, EventArgs e)
        {
            string json;

            try
            {
                json = await Tools.Communication.Communicate(null, "http://avatarserv.pl/bqe/versioncheck.php");
            }
            catch (Exception)
            {
                Console.WriteLine("comm error");
                return;
            }

            Console.WriteLine(json);

            JObject result = JObject.Parse(json);

            NewestVersion = result["newest"].ToString();

            if (System.Windows.Application.Current.FindResource("Version").ToString() != NewestVersion)
            {
                UpdateAvailable = true;
                updateFound.Invoke();
            }
        }

        // --------

        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("done");
            onUpdateDownloaded.Invoke();
            downloading = false;
            downloaded = true;
        }

    }
}
