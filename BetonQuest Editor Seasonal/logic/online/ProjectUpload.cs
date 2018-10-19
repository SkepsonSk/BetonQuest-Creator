using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.yaml;
using BetonQuest_Editor_Seasonal.pages.editor;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetonQuest_Editor_Seasonal.logic.online
{
    public class ProjectUpload
    {
        private BackgroundWorker worker;
        private Quest quest;

        private string result;

        // -------- Initialization --------

        public ProjectUpload(Quest quest)
        {
            this.quest = quest;

            worker = new BackgroundWorker();

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        // -------- Delegates --------

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            new QuestDataSaver(quest, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor\temp\" + quest.Name, true);

            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncoding = Encoding.UTF8;
                zip.AddDirectory(BetonQuest_Editor_Seasonal.Project.ApplicationDirectory + @"\temp\" + quest.Name);
                zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                zip.Save(BetonQuest_Editor_Seasonal.Project.ApplicationDirectory + @"\temp\" + quest.Name + ".zip");
            }

            Process process = new Process();

            string jarPath = BetonQuest_Editor_Seasonal.Project.ApplicationDirectory + @"\BetonQuestUploader.jar";

            Console.WriteLine(jarPath);

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "java";
            process.StartInfo.RedirectStandardOutput = true;

            process.StartInfo.Arguments = "-jar \"" + BetonQuest_Editor_Seasonal.Project.ApplicationDirectory + @"\BetonQuestUploader.jar" + "\"" + " http://avatarserv.pl:8125/ skepson beduin12 \"" + BetonQuest_Editor_Seasonal.Project.ApplicationDirectory + @"\temp\" + quest.Name + ".zip\"";
            process.Start();

            result = process.StandardOutput.ReadToEnd();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (result == "ACCEPT") EditorHub.HubInstance.Alert("Project has been UPLOADED!", AlertType.Success, 3);
            else EditorHub.HubInstance.Alert("An error occured while UPLOADING!", AlertType.Error, 3);
        }

    }
}
