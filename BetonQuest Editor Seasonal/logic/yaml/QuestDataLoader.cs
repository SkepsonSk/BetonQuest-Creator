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

        private readonly string directory;
        private Dictionary<string, Conversation> conversationsFiles;

        // -------- Initializator --------

        public QuestDataLoader(string directory)
        {
            this.directory = directory;
            conversationsFiles = new Dictionary<string, Conversation>();

            LoadProperties("events.yml", Project.Quest.Events);
            LoadProperties("conditions.yml", Project.Quest.Conditions);
            LoadProperties("objectives.yml", Project.Quest.Objectives);
            LoadProperties("journal.yml", Project.Quest.JournalEntries);
            LoadProperties("items.yml", Project.Quest.Items);

            foreach (string conversationFile in Directory.GetFiles(directory + @"\conversations")) LoadConversation(conversationFile);
            ApplyConversationIDs();
        }

        // --------- Loaders --------

        private void ApplyConversationIDs()
        {
            string mainFile = directory + @"\main.yml";

            YamlStream mainStream;
            YamlMappingNode mainRoot;

            using (StringReader mainReader = new StringReader(File.ReadAllText(mainFile)))
            {
                mainStream = new YamlStream();
                mainStream.Load(mainReader);

                if (mainStream.Documents.Count != 0)
                {
                    mainRoot = (YamlMappingNode)mainStream.Documents[0].RootNode;

                    foreach (var mainOption in mainRoot.Children)
                    {

                        YamlScalarNode option = mainOption.Key as YamlScalarNode;
                        if (option.Value.Equals("npcs", StringComparison.InvariantCultureIgnoreCase)){

                            YamlMappingNode npcs = mainOption.Value as YamlMappingNode;
                            foreach (var npc in npcs.Children)
                            {
                                YamlScalarNode npcIDNode = npc.Key as YamlScalarNode;
                                YamlScalarNode npcFileName = npc.Value as YamlScalarNode;
                                int npcID = int.Parse(npcIDNode.Value);

                                Conversation conversation = conversationsFiles[npcFileName.Value];
                                conversation.NPCID = npcID;               
                            }

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

        public void LoadConversation(string conversationFile)
        {
            string conversationFileName = conversationFile.Substring(conversationFile.LastIndexOf('\\') + 1);
            conversationFileName = conversationFileName.Substring(0, conversationFileName.LastIndexOf('.'));

            YamlStream conversationStream;
            YamlMappingNode rootConversationNode;
            string key, value;

            Conversation conversation = null;
            Statement statement;
            bool isNpcStatement;

            using (StringReader conversationReader = new StringReader(File.ReadAllText(conversationFile)))
            {
                conversationStream = new YamlStream();
                conversationStream.Load(conversationReader);

                rootConversationNode = (YamlMappingNode)conversationStream.Documents[0].RootNode;
            }

            foreach (var keyValuePair in rootConversationNode.Children)
            {
                if (keyValuePair.Value.NodeType == YamlNodeType.Scalar)
                {
                    key = ((YamlScalarNode) keyValuePair.Key).Value;
                    value = ((YamlScalarNode) keyValuePair.Value).Value;

                    if (key.Equals("quester", StringComparison.InvariantCultureIgnoreCase))
                    {
                        conversation = new Conversation(value, 0);
                        conversationsFiles.Add(conversationFileName, conversation);
                        Project.Quest.Conversations.Add(conversation);
                    }
                    else if (key.Equals("first", StringComparison.InvariantCultureIgnoreCase)) conversation.StartStatementsList = value;
                }
                else if (keyValuePair.Value.NodeType == YamlNodeType.Mapping)
                {
                    isNpcStatement = ((YamlScalarNode)keyValuePair.Key).Value.Equals("NPC_options", StringComparison.InvariantCultureIgnoreCase);

                    YamlMappingNode statements = keyValuePair.Value as YamlMappingNode;
                    foreach (var conversationStatement in statements.Children)
                    {
                        YamlMappingNode readStatementProperties = conversationStatement.Value as YamlMappingNode;
                        statement = new Statement(((YamlScalarNode) conversationStatement.Key).Value);

                        foreach (var readProperty in readStatementProperties.Children)
                        {
                            key = ((YamlScalarNode)readProperty.Key).Value;
                            value = ((YamlScalarNode)readProperty.Value).Value;

                            if (key.Equals("text", StringComparison.InvariantCultureIgnoreCase)) statement.Content = value;
                            else if (key.Equals("events", StringComparison.InvariantCultureIgnoreCase) || key.Equals("event", StringComparison.InvariantCultureIgnoreCase)) statement.EventsList = value;
                            else if (key.Equals("conditions", StringComparison.InvariantCultureIgnoreCase) || key.Equals("condition", StringComparison.InvariantCultureIgnoreCase)) statement.ConditionsList = value;
                            else if (key.Equals("pointers", StringComparison.InvariantCultureIgnoreCase) || key.Equals("pointer", StringComparison.InvariantCultureIgnoreCase)) statement.NextStatementsList = value;
                        }

                        if (isNpcStatement) conversation.NPCStatements.Add(statement);
                        else conversation.PlayerStatements.Add(statement);

                    }

                }
            }
            conversation.ApplyPreStartStatements();
            conversation.CreateStatementsConnections();

        }

    }
}
