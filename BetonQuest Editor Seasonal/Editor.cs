using BetonQuest_Editor_Seasonal.logic.gcreator.presentation;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal
{
    public class Editor
    {
        public static string ApplicationDirectory;

        // -------- Start --------
        
        public static void Initiate()
        {
            ApplicationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor";
        }

        // -------- Access --------
        // ---- Loading ----

        public static void InitiateProject(string directory) 
        {
            Console.WriteLine(directory);

            if (directory.Contains(ApplicationDirectory))
            {
                string id = directory.Substring(directory.LastIndexOf(@"\") + 1);
                Project.Quest = new Quest(id);

                Console.WriteLine("Editor Initiator: id: " + id);

                if (!File.Exists(ApplicationDirectory + @"\definitions\" + id + ".txt")) return;
                string name = File.ReadAllLines(ApplicationDirectory + @"\definitions\" + id + ".txt")[0];

                Console.WriteLine("Name: " + name);

                Project.Quest.Name = name;

                Console.WriteLine("Project loading: " + Project.Quest.ID + ", " + Project.Quest.Name);
            }
            else
            {
                string id = Tools.GenerateID();
                string name = directory.Substring(directory.LastIndexOf(@"\") + 1);

                Directory.CreateDirectory(Project.ApplicationDirectory + @"\projects\" + id);
                Tools.CopyDirectory(new DirectoryInfo(directory), new DirectoryInfo(ApplicationDirectory + @"\projects\" + id));

                File.Create(Project.ApplicationDirectory + @"\definitions\" + id + @".txt").Dispose();
                File.WriteAllLines(Project.ApplicationDirectory + @"\definitions\" + id + @".txt", new string[] { name });

                Project.Quest = new Quest(id) { Name = name };

                Console.WriteLine("Project importing: " + Project.Quest.ID + ", " + Project.Quest.Name);
            }

        }

        public static void LoadGCE(string directory)
        {
            if (Directory.Exists(ApplicationDirectory + @"\gce\" + Project.Quest.ID))
            {
                string gcePath = ApplicationDirectory + @"\gce\" + Project.Quest.ID;
                foreach (Conversation conversation in Project.Quest.Conversations)
                {
                    if (!File.Exists(gcePath + @"\" + conversation.NPCName + conversation.NPCID + @".gce")) continue;

                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(gcePath + @"\" + conversation.NPCName + conversation.NPCID + @".gce", FileMode.Open, FileAccess.Read, FileShare.Read);

                    conversation.GraphicalConversationEditor = new GraphicalConversationEditor(conversation, formatter.Deserialize(stream) as GCEPresentation);

                    Console.WriteLine("Loaded GCE for " + conversation.NPCName);
                }
            }
        }

    }
}
