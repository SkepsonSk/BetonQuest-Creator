using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.online;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using BetonQuest_Editor_Seasonal.pages.editor;
using BetonQuest_Editor_Seasonal.pages.editor.properties;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors;
using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BetonQuest_Editor_Seasonal
{
    public class Project
    {
        public static Quest Quest;

        public static Stack<QuestDataImage> QuestUndoOperations = new Stack<QuestDataImage>();
        public static Stack<QuestDataImage> QuestRedoOperations = new Stack<QuestDataImage>();

        public static List<QuestDataImage> QuestChanges = new List<QuestDataImage>();
        public static int QuestChangesPosition = 0;

        public static string ApplicationDirectory;
        public static string ApplicationTempDirectory;

        public static string LastEditedProjectID;
        public static string LastEditedProjectName;
        public static string LastEditedProjectPath;

        public static UpdateManager UpdateManager;

        // -------- File System --------

        public static void VerifyDirectories()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor";

            ApplicationDirectory = path;
            ApplicationTempDirectory = path + @"\temp";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!Directory.Exists(path + @"\enchants")) Directory.CreateDirectory(path + @"\enchants");
            if (!Directory.Exists(path + @"\gce")) Directory.CreateDirectory(path + @"\gce");
            if (!Directory.Exists(path + @"\projects")) Directory.CreateDirectory(path + @"\projects");
            if (!Directory.Exists(path + @"\definitions")) Directory.CreateDirectory(path + @"\definitions");
            if (!Directory.Exists(path + @"\temp")) Directory.CreateDirectory(path + @"\temp");

            if (!File.Exists(path + @"\configuration.txt")) File.Create(path + @"\configuration.txt");

            try
            {
                using (StreamReader reader = new StreamReader(path + @"\configuration.txt"))
                {
                    int step = 0;
                    while (!reader.EndOfStream)
                    {
                        if (step == 0) LastEditedProjectName = reader.ReadLine();
                        else if (step == 1) LastEditedProjectPath = reader.ReadLine();

                        step++;
                    }      
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error");
            }

        }

        public static void VerifyEnchantDefaultFiles()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor\enchants\";
            string[] enchants;

            if (!File.Exists(path + @"ArmorDefault.txt"))
            {
                enchants = new string[12];
                enchants[0] = "true";
                enchants[1] = "Protection-4";
                enchants[2] = "Fire_Protection-4";
                enchants[3] = "Feather_Falling-4";
                enchants[4] = "Blast_Protection-4";
                enchants[5] = "Projectile_Protection-4";
                enchants[6] = "Respiration-3";
                enchants[7] = "Aqua_Affinity-1";
                enchants[8] = "Thorns-3";
                enchants[9] = "Depth_Strider-3";
                enchants[10] = "Frost_Walker-2";
                enchants[11] = "Curse_of_Binding-1";

                File.Create(path + @"ArmorDefault.txt").Close();
                File.WriteAllLines(path + @"ArmorDefault.txt", enchants);
            }
            if (!File.Exists(path + @"SwordDefault.txt"))
            {
                enchants = new string[8];
                enchants[0] = "true";
                enchants[1] = "Sharpness-5";
                enchants[2] = "Smite-5";
                enchants[3] = "Bane_of_Arthropods-5";
                enchants[4] = "Knockback-2";
                enchants[5] = "Fire_Aspect-2";
                enchants[6] = "Looting-3";
                enchants[7] = "Sweeping_Edge-3";

                File.Create(path + @"SwordDefault.txt").Close();
                File.WriteAllLines(path + @"SwordDefault.txt", enchants);
            }
            if (!File.Exists(path + @"BowDefault.txt"))
            {
                enchants = new string[5];
                enchants[0] = "true";
                enchants[1] = "Power-5";
                enchants[2] = "Punch-2";
                enchants[3] = "Flame-1";
                enchants[4] = "Infinity-1";

                File.Create(path + @"BowDefault.txt").Close();
                File.WriteAllLines(path + @"BowDefault.txt", enchants);
            }
            if (!File.Exists(path + @"FishingRodDefault.txt"))
            {
                enchants = new string[3];
                enchants[0] = "true";
                enchants[1] = "Luck_of_the_Sea-3";
                enchants[2] = "Lure-3";

                File.Create(path + @"FishingRodDefault.txt").Close();
                File.WriteAllLines(path + @"FishingRodDefault.txt", enchants);
            }
            if (!File.Exists(path + @"ToolDefault.txt"))
            {
                enchants = new string[4];
                enchants[0] = "true";
                enchants[1] = "Efficiency-5";
                enchants[2] = "Silk_Touch-1";
                enchants[3] = "Fortune-3";

                File.Create(path + @"ToolDefault.txt").Close();
                File.WriteAllLines(path + @"ToolDefault.txt", enchants);
            }
            if (!File.Exists(path + @"AnythingDefault.txt"))
            {
                enchants = new string[4];
                enchants[0] = "true";
                enchants[1] = "Unbreaking-3";
                enchants[2] = "Mending-1";
                enchants[3] = "Curse_of_Vanishing-1";

                File.Create(path + @"AnythingDefault.txt").Close();
                File.WriteAllLines(path + @"AnythingDefault.txt", enchants);
            }
        }

        public static void SaveLastEditedProject()
        {
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor\configuration.txt", LastEditedProjectName + Environment.NewLine + LastEditedProjectPath);
        }

        // -------- Creating New Quest --------

        public static void CreateNewQuest(string name)
        {
            string id = Tools.GenerateID(24);

            Quest = new Quest(id);
            Quest.Name = name;

            new ConversationsPage();
            new EventsPage();
            new ConditionsPage();
            new ObjectivesPage();
            new JournalPage();
            new ItemsPage();
            new StatementEditor();
        }

        // -------- Update Manager --------

    }
}
