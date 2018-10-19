using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.structure.conversating
{
    [Serializable]
    public class Statement : Property
    {
        private List<Property> conditions;
        private List<Property> events;

        private List<string> negatedConditions;

        private List<Statement> nextStatements;
        private List<string> preNextStatements;

        // -------- Initializator --------

        public Statement(string id) : base(id, string.Empty)
        {
            conditions = new List<Property>();
            events = new List<Property>();

            negatedConditions = new List<string>();

            nextStatements = new List<Statement>();
            preNextStatements = new List<string>();
        }

        public Statement(string id, string content) : base(id, content)
        {
            conditions = new List<Property>();
            events = new List<Property>();

            negatedConditions = new List<string>();

            nextStatements = new List<Statement>();
        }

        public Statement(string id, string content, List<Property> conditions, List<Property> events, List<string> negatedConditions, List<Statement> nextStatements) : base(id, content)
        {
            this.conditions = conditions;
            this.events = events;

            this.negatedConditions = negatedConditions;

            this.nextStatements = nextStatements;
        }

        // --------- Access --------

        public string Content {
            get {
                return Command;
            }
            set {
                Command = value;
            }
        }

        public List<Statement> NextStatements {
            get {
                return nextStatements;
            }
        }

        public List<Property> Conditions {
            get {
                return conditions;
            }
        }

        public List<string> NegatedConditions {
            get {
                return negatedConditions;
            }
        }

        public List<Property> Events {
            get {
                return events;
            }
        }

        public List<string> PreNextStatements {
            get {
                return preNextStatements;
            }
        }

        // -------- 

        public string EventsList {

            get {

                StringBuilder events = new StringBuilder();

                for (int n = 0; n < this.events.Count; n++)
                {
                    if (n == this.events.Count - 1) events.Append(this.events[n].ID);
                    else events.Append(this.events[n].ID + ",");
                }

                return events.ToString();
            }
            set {
                this.events.Clear();
                StringBuilder events = new StringBuilder();

                for (int n = 0; n < value.Length; n++)
                {
                    if (value[n] == ',')
                    {
                        this.events.Add(Project.Quest.GetEvent(events.ToString()));
                        events.Clear();
                    }
                    else if (n == value.Length - 1)
                    {
                        events.Append(value[n]);
                        this.events.Add(Project.Quest.GetEvent(events.ToString()));
                        events.Clear();
                    }
                    else events.Append(value[n]);
                }
            }
        }

        public string ConditionsList {

            get {

                StringBuilder conditions = new StringBuilder();

                for (int n = 0; n < this.conditions.Count; n++)
                {
                    if (n == this.conditions.Count - 1)
                    {
                        if (negatedConditions.Contains(this.conditions[n].ID)) conditions.Append("!" + this.conditions[n].ID);
                        else conditions.Append(this.conditions[n].ID);
                    }
                    else
                    {
                        if (negatedConditions.Contains(this.conditions[n].ID)) conditions.Append("!" + this.conditions[n].ID + ",");
                        else conditions.Append(this.conditions[n].ID + ",");
                    }
                }

                return conditions.ToString();
            }
            set {

                this.conditions.Clear();
                StringBuilder conditions = new StringBuilder();

                for (int n = 0; n < value.Length; n++)
                {
                    if (value[n] == ',')
                    {
                        if (conditions.ToString().StartsWith("!"))
                        {
                            this.conditions.Add(Project.Quest.GetCondition(conditions.ToString().Substring(1)));
                            negatedConditions.Add(conditions.ToString().Substring(1));
                        }
                        else this.conditions.Add(Project.Quest.GetCondition(conditions.ToString()));
                        conditions.Clear();
                    }
                    else if (n == value.Length - 1)
                    {
                        conditions.Append(value[n]);
                        if (conditions.ToString().StartsWith("!"))
                        {
                            this.conditions.Add(Project.Quest.GetCondition(conditions.ToString().Substring(1)));
                            negatedConditions.Add(conditions.ToString().Substring(1));
                        }
                        else this.conditions.Add(Project.Quest.GetCondition(conditions.ToString()));
                        conditions.Clear();
                    }

                    else conditions.Append(value[n]);
                }
            }

        }

        public string NextStatementsList {

            get {

                StringBuilder statements = new StringBuilder();

                for (int n = 0; n < nextStatements.Count; n++)
                {
                    if (n == nextStatements.Count - 1) statements.Append(nextStatements[n].ID);
                    else statements.Append(nextStatements[n].ID + ",");
                }

                return statements.ToString();
            }
            set {
                this.preNextStatements.Clear();
                StringBuilder preNextStatements = new StringBuilder();

                for (int n = 0; n < value.Length; n++)
                {
                    if (value[n] == ',')
                    {
                        Console.WriteLine(preNextStatements.ToString());
                        this.preNextStatements.Add(preNextStatements.ToString());
                        preNextStatements.Clear();
                    }
                    else if (n == value.Length - 1)
                    {
                        preNextStatements.Append(value[n]);
                        Console.WriteLine(preNextStatements.ToString());
                        this.preNextStatements.Add(preNextStatements.ToString());
                        preNextStatements.Clear();
                    }
                    else preNextStatements.Append(value[n]);
                }
            }

        }

    }
}
