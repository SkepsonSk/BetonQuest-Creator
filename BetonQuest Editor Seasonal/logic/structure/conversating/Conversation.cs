using BetonQuest_Editor_Seasonal.pages.editor.properties.subeditors.conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.structure.conversating
{
    [Serializable]
    public class Conversation : Property
    {
        private List<Statement> npcStatements;
        private List<Statement> playerStatements;

        private List<Statement> startStatements;
        private List<string> preStartStatements;

        private GraphicalConversationEditor graphicalConversationEditor;

        // -------- Initializator --------

        public Conversation(string npcName, int npcID) : base(npcName, npcID.ToString())
        {
            npcStatements = new List<Statement>();
            playerStatements = new List<Statement>();

            startStatements = new List<Statement>();
            preStartStatements = new List<string>();
        }

        public Conversation(string npcName, int npcID, List<Statement> npcStatements, List<Statement> playerStatements, List<Statement> startStatements) : base(npcName, npcID.ToString())
        {
            this.npcStatements = npcStatements;
            this.playerStatements = playerStatements;

            this.startStatements = startStatements;
        }

        // -------- Access --------

        public string NPCName {
            get {
                return ID;
            }
            set {
                ID = value;
            }
        }

        public int NPCID {
            get {
                return int.Parse(Command);
            }
            set {
                Command = value.ToString();
            }
        }

        public List<Statement> NPCStatements {
            get {
                return npcStatements;
            }
        }

        public List<Statement> PlayerStatements {
            get {
                return playerStatements;
            }
        }

        public List<Statement> StartStatements {
            get {
                return startStatements;
            }
        }

        public GraphicalConversationEditor GraphicalConversationEditor {
            get {
                return graphicalConversationEditor;
            }
            set {
                graphicalConversationEditor = value;
            }
        }

        // ----

        public string StartStatementsList {

            get {
                StringBuilder startStatements = new StringBuilder();

                for (int n = 0; n < this.startStatements.Count; n++)
                {
                    if (n == this.startStatements.Count - 1) startStatements.Append(this.startStatements[n].ID);
                    else startStatements.Append(this.startStatements[n].ID + ",");
                }

                return startStatements.ToString();
            }
            set {

                this.preStartStatements.Clear();
                StringBuilder preStartStatements = new StringBuilder();

                for (int n = 0; n < value.Length; n++)
                {
                    if (value[n] == ',')
                    {
                        Console.WriteLine(preStartStatements.ToString());
                        this.preStartStatements.Add(preStartStatements.ToString());
                        preStartStatements.Clear();
                    }
                    else if (n == value.Length - 1)
                    {
                        preStartStatements.Append(value[n]);
                        Console.WriteLine(preStartStatements.ToString());
                        this.preStartStatements.Add(preStartStatements.ToString());
                        preStartStatements.Clear();
                    }
                    else preStartStatements.Append(value[n]);
                }

            }
        }

        // ----

        public void ApplyPreStartStatements()
        {
            if (preStartStatements.Count == 0) return;

            startStatements.Clear();
            foreach (string startStatement in preStartStatements)
            {
                Statement statement = GetNPCStatement(startStatement);
                if (statement == null) continue;
                startStatements.Add(statement);
            }
        }

        public void CreateStatementsConnections()
        {
            foreach (Statement statement in npcStatements)
            {
                foreach (string readPlayerStatement in statement.PreNextStatements)
                {
                    Statement playerStatement = GetPlayerStatement(readPlayerStatement);
                    if (playerStatement == null) continue;
                    statement.NextStatements.Add(playerStatement);
                }
            }
            foreach (Statement statement in playerStatements)
            {
                foreach (string readNPCStatement in statement.PreNextStatements)
                {
                    Statement npcStatement = GetNPCStatement(readNPCStatement);
                    if (npcStatement == null) continue;
                    statement.NextStatements.Add(npcStatement);
                }
            }

        }

        // -------- Specific Statement Getter --------

        public Statement GetStartStatement(string id)
        {
            foreach (Statement statement in startStatements)
            {
                if (statement.ID.Equals(id)) return statement;
            }
            return null;
        }

        public Statement GetPlayerStatement(string id, string[] ignored = null)
        {
            foreach (Statement statement in playerStatements)
            {
                if (ignored != null && ignored.Contains(statement.ID)) continue;
                if (statement.ID.Equals(id)) return statement;
            }
            return null;
        }

        public Statement GetNPCStatement(string id, string[] ignored = null)
        {
            foreach (Statement statement in npcStatements)
            {
                if (ignored != null && ignored.Contains(statement.ID)) continue;
                if (statement.ID.Equals(id)) return statement;
            }
            return null;
        }

        public void RemoveStatement(Statement statement)
        {
            for (int n = NPCStatements.Count - 1; n >= 0; n--)
            {
                if (NPCStatements[n].Equals(statement)) NPCStatements.RemoveAt(n);
            }
            for (int n = PlayerStatements.Count - 1; n >= 0; n--)
            {
                if (PlayerStatements[n].Equals(statement)) PlayerStatements.RemoveAt(n);
            }
        }

        public void ResetStatement(Statement statement)
        {
            statement.Conditions.Clear();
            statement.NegatedConditions.Clear();
            statement.Events.Clear();
        }

        public void BreakConnectionsWithStatement(Statement statement)
        {
            if (NPCStatements.Contains(statement))
            {
                foreach (Statement npcStatement in NPCStatements)
                {
                    if (npcStatement.NextStatements.Contains(statement)) npcStatement.NextStatements.Remove(statement);
                }
            }
            else
            {
                foreach (Statement playerStatement in PlayerStatements)
                {
                    if (playerStatement.NextStatements.Contains(statement)) playerStatement.NextStatements.Remove(statement);
                }
            }

            statement.NextStatements.Clear();
        }

    }
}
