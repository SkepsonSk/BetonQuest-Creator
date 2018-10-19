using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BetonQuest_Editor_Seasonal;
using YamlDotNet.RepresentationModel;
using BetonQuest_Editor_Seasonal.pages.editor;

namespace BetonQuest_Editor_Seasonal.logic.yaml
{
    public class QuestDataLoader
    {
        private string directory;

        // -------- Initializator --------

        public QuestDataLoader(string main)
        {
            directory = main.Substring(0, main.LastIndexOf(@"\"));

            Console.WriteLine(directory);

            if (directory.Contains(Project.ApplicationDirectory))
            {
                string id = directory.Substring(directory.LastIndexOf(@"\") + 1);
                Project.Quest = new Quest(id);

                if (!File.Exists(Project.ApplicationDirectory + @"\definitions\" + id + ".txt")) return;
                string name = File.ReadAllLines(Project.ApplicationDirectory + @"\definitions\" + id + ".txt")[0];

                Project.Quest.Name = name;

                Console.WriteLine("Project loading: " + Project.Quest.ID + ", " + Project.Quest.Name);
            }
            else
            {
                if (!Tools.ProjectImportPathFine(directory))
                {
                    Console.WriteLine("invalid directory"); return;
                }

                string id = Tools.GenerateID();
                string name = directory.Substring(directory.LastIndexOf(@"\") + 1);

                Directory.CreateDirectory(Project.ApplicationDirectory + @"\projects\" + id);
                Tools.CopyDirectory(new DirectoryInfo(directory), new DirectoryInfo(Project.ApplicationDirectory + @"\projects\" + id));

                File.Create(Project.ApplicationDirectory + @"\definitions\" + id + @".txt").Dispose();
                File.WriteAllLines(Project.ApplicationDirectory + @"\definitions\" + id + @".txt", new string[] { name });

                Project.Quest = new Quest(id);
                Project.Quest.Name = name;

                Console.WriteLine("Project (imported) loading: " + Project.Quest.ID + ", " + Project.Quest.Name);
            }

            LoadMain();
            LoadProperties("events.yml", Project.Quest.Events);
            LoadProperties("conditions.yml", Project.Quest.Conditions);
            LoadProperties("objectives.yml", Project.Quest.Objectives);
            LoadProperties("journal.yml", Project.Quest.JournalEntries);
            LoadProperties("items.yml", Project.Quest.Items);
            LoadConversations();

            //Project.ApplyQuestChanges();
            //Project.StartSaveGuard();
        }

        // --------- Loaders --------

        private void LoadMain()
        {
            string file = directory + @"\main.yml";

            if (!File.Exists(file)) return;

            using (StringReader reader = new StringReader(File.ReadAllText(file)))
            {
                YamlStream stream = new YamlStream();
                stream.Load(reader);

                if (stream.Documents.Count == 0) return;

                YamlMappingNode root = (YamlMappingNode)stream.Documents[0].RootNode;

                int npcID;
                string npcName;

                foreach (var node in root.Children)
                {
                    YamlScalarNode scalar = node.Key as YamlScalarNode;
                    if (scalar.Value.Equals("npcs", StringComparison.InvariantCultureIgnoreCase))
                    {
                        YamlMappingNode npcSet = node.Value as YamlMappingNode;

                        foreach (var npc in npcSet.Children)
                        {
                            YamlScalarNode npcIDNode = npc.Key as YamlScalarNode;
                            YamlScalarNode npcNameNode = npc.Value as YamlScalarNode;

                            npcID = int.Parse(npcIDNode.Value);
                            npcName = npcNameNode.Value;

                            Project.Quest.Conversations.Add(new Conversation(npcName, npcID));
                            Console.WriteLine(npcID + " " + npcName);
                        }
                    }
                }
            }
        }

        private void LoadProperties(string file, List<Property> properties)
        {
            file = directory + @"\" + file;

            if (!File.Exists(file)) return;

            using (StringReader reader = new StringReader(File.ReadAllText(file)))
            {
                YamlStream stream = new YamlStream();
                stream.Load(reader);

                if (stream.Documents.Count == 0) return;

                YamlMappingNode root = (YamlMappingNode)stream.Documents[0].RootNode;

                foreach (var node in root.Children)
                {
                    properties.Add(new Property(((YamlScalarNode)node.Key).Value, ((YamlScalarNode)node.Value).Value));
                }
            }
        }

        private void LoadConversations()
        {
            foreach (string file in Directory.GetFiles(directory + @"\conversations"))
            {
                using (StringReader reader = new StringReader(File.ReadAllText(file)))
                {
                    YamlStream stream = new YamlStream();
                    stream.Load(reader);

                    YamlMappingNode root = (YamlMappingNode)stream.Documents[0].RootNode;

                    Conversation conversation = null;
                    Statement statement;
                    bool npc;

                    foreach (var readType in root.Children)
                    {
                        if (readType.Value.NodeType == YamlNodeType.Scalar)
                        {
                            string key = ((YamlScalarNode)readType.Key).Value;
                            string value = ((YamlScalarNode)readType.Value).Value;

                            if (key.Equals("quester", StringComparison.InvariantCultureIgnoreCase)) conversation = Project.Quest.GetConversation(value);
                            if (key.Equals("first", StringComparison.InvariantCultureIgnoreCase)) conversation.StartStatementsList = value;
                        }
                        else if (readType.Value.NodeType == YamlNodeType.Mapping)
                        {

                            npc = ((YamlScalarNode)readType.Key).Value.Equals("NPC_options", StringComparison.InvariantCultureIgnoreCase);

                            YamlMappingNode type = readType.Value as YamlMappingNode;
                            foreach (var readStatement in type.Children)
                            {
                                YamlMappingNode readStatementProperties = readStatement.Value as YamlMappingNode;
                                statement = new Statement(((YamlScalarNode)readStatement.Key).Value);

                                foreach (var readProperty in readStatementProperties.Children)
                                {
                                    string property = ((YamlScalarNode)readProperty.Key).Value;
                                    string value = ((YamlScalarNode)readProperty.Value).Value;

                                    if (property.Equals("text", StringComparison.InvariantCultureIgnoreCase)) statement.Content = value;
                                    else if (property.Equals("events", StringComparison.InvariantCultureIgnoreCase) || property.Equals("event", StringComparison.InvariantCultureIgnoreCase)) statement.EventsList = value;
                                    else if (property.Equals("conditions", StringComparison.InvariantCultureIgnoreCase) || property.Equals("condition", StringComparison.InvariantCultureIgnoreCase)) statement.ConditionsList = value;
                                    else if (property.Equals("pointers", StringComparison.InvariantCultureIgnoreCase) || property.Equals("pointer", StringComparison.InvariantCultureIgnoreCase)) statement.NextStatementsList = value;
                                }

                                if (npc) conversation.NPCStatements.Add(statement);
                                else conversation.PlayerStatements.Add(statement);

                            }
                        }
                    }
                    conversation.ApplyPreStartStatements();
                    conversation.CreateStatementsConnections();
                }
            }
        }
    }
}
