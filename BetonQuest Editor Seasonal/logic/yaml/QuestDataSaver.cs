using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace BetonQuest_Editor_Seasonal.logic.yaml
{
    public class QuestDataSaver
    {

        private Quest quest;
        private string directory;
        private string pathDirectory;

        // -------- Initializator --------

        public QuestDataSaver(Quest quest, string directory, bool useName = false)
        {
            this.quest = quest;
            this.directory = directory;

            if (useName) pathDirectory = directory + @"\" + quest.Name;
            else pathDirectory = directory + @"\" + quest.ID;

            if (!useName) GenerateDefinition();
            Prepare();
            SaveMain();

            if (Project.Quest.Conversations.Count > 0) SaveConversations();
            if (Project.Quest.Events.Count > 0) SaveProperties(quest.Events, pathDirectory + @"\events.yml");
            if (Project.Quest.Conditions.Count > 0) SaveProperties(quest.Conditions, pathDirectory + @"\conditions.yml");
            if (Project.Quest.Objectives.Count > 0) SaveProperties(quest.Objectives, pathDirectory + @"\objectives.yml");
            if (Project.Quest.JournalEntries.Count > 0) SaveProperties(quest.JournalEntries, pathDirectory + @"\journal.yml");
            if (Project.Quest.Items.Count > 0) SaveProperties(quest.Items, pathDirectory + @"\items.yml");
        }

        // -------- Saving Units --------

        public void GenerateDefinition()
        {
            if (File.Exists(Project.ApplicationDirectory + @"\definitions\" + quest.ID + @".txt")) File.Delete(Project.ApplicationDirectory + @"\definitions\" + quest.ID + @".txt");
            File.Create(Project.ApplicationDirectory + @"\definitions\" + quest.ID + @".txt").Dispose();
            File.WriteAllLines(Project.ApplicationDirectory + @"\definitions\" + quest.ID + @".txt", new string[] { quest.Name });
        }

        public void Prepare()
        {

            if (Directory.Exists(pathDirectory))
            {           
                if (Directory.Exists(pathDirectory + @"\conversations"))
                {
                    foreach (string file in Directory.GetFiles(pathDirectory + @"\conversations")) File.Delete(file);
                }
                foreach (string file in Directory.GetFiles(pathDirectory)) File.Delete(file);

                Tools.EmptyFolder(new DirectoryInfo(pathDirectory));
            }

            Directory.CreateDirectory(pathDirectory);

            if (Project.Quest.Conversations.Count != 0)
            {
                Directory.CreateDirectory(pathDirectory + @"\conversations");
                foreach (Conversation conversation in Project.Quest.Conversations) File.Create(pathDirectory + @"\conversations\" + conversation.NPCName + ".yml").Close();
            }

            File.Create(pathDirectory + @"\main.yml").Close();
            File.Create(pathDirectory + @"\events.yml").Close();
            File.Create(pathDirectory + @"\conditions.yml").Close();
            File.Create(pathDirectory + @"\objectives.yml").Close();
            File.Create(pathDirectory + @"\journal.yml").Close();
            File.Create(pathDirectory + @"\items.yml").Close();
        }

        public void SaveMain()
        {
            string root = "variables:";

            using (StringReader reader = new StringReader(root))
            {

                YamlStream stream = new YamlStream();
                stream.Load(reader);

                YamlMappingNode rootMappingNode = (YamlMappingNode)stream.Documents[0].RootNode;
                YamlMappingNode npcs = new YamlMappingNode();

                foreach (Conversation conversation in quest.Conversations)
                {
                    Console.WriteLine(conversation.NPCID.ToString() + " " + conversation.NPCName);
                    npcs.Add(conversation.NPCID.ToString(), conversation.NPCName);
                }

                rootMappingNode.Add("npcs", npcs);

                using (TextWriter writer = File.CreateText(pathDirectory + @"\main.yml"))
                {
                    stream.Save(writer, false);
                }

            }
        }

        public void SaveProperties(List<Property> properties, string directory)
        {
            if (properties.Count == 0) return;

            string root = properties[0].ID + ": " + properties[0].Command;

            using (StringReader reader = new StringReader(root))
            {
                YamlStream stream = new YamlStream();
                stream.Load(reader);
                
                YamlMappingNode rootNode = (YamlMappingNode) stream.Documents[0].RootNode;

                for (int n = 1; n < properties.Count; n++)
                {
                    rootNode.Add(properties[n].ID, properties[n].Command);
                }

                using (TextWriter writer = File.CreateText(directory))
                {
                    stream.Save(writer, false);
                }
            }

        }

        public void SaveConversations()
        {
            foreach (Conversation conversation in Project.Quest.Conversations)
            {
                string root = "quester: " + conversation.NPCName;

                using (StringReader reader = new StringReader(root))
                {
                    YamlStream stream = new YamlStream();
                    stream.Load(reader);

                    YamlMappingNode rootNode = (YamlMappingNode)stream.Documents[0].RootNode;

                    rootNode.Add("first", conversation.StartStatementsList);

                    YamlMappingNode npcStatements = new YamlMappingNode();
                    YamlMappingNode playerStatements = new YamlMappingNode();

                    foreach (Statement statement in conversation.NPCStatements)
                    {
                        YamlMappingNode node = new YamlMappingNode();

                        node.Add("text", statement.Content);
                        if (statement.Events.Count > 0) node.Add("events", statement.EventsList);
                        if (statement.Conditions.Count > 0) node.Add("conditions", statement.ConditionsList);
                        if (statement.NextStatements.Count > 0) node.Add("pointers", statement.NextStatementsList);

                        npcStatements.Add(statement.ID, node);
                    }

                    foreach (Statement statement in conversation.PlayerStatements)
                    {
                        YamlMappingNode node = new YamlMappingNode();

                        node.Add("text", statement.Content);
                        if (statement.Events.Count > 0) node.Add("events", statement.EventsList);
                        if (statement.Conditions.Count > 0) node.Add("conditions", statement.ConditionsList);
                        if (statement.NextStatements.Count > 0) node.Add("pointers", statement.NextStatementsList);

                        playerStatements.Add(statement.ID, node);
                    }

                    rootNode.Add("NPC_options", npcStatements);
                    rootNode.Add("player_options", playerStatements);

                    using (TextWriter writer = File.CreateText(pathDirectory + @"\conversations\" + conversation.NPCName + ".yml"))
                    {
                        stream.Save(writer, false);
                    }
                }

            }
        }

    }
}
