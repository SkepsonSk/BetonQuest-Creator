﻿using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetonQuest_Editor_Seasonal.logic.structure
{
    public class Quest
    {
        public string ID;
        public string Name;

        public List<Conversation> Conversations;

        public List<Property> Events;
        public List<Property> Conditions;
        public List<Property> Objectives;
        public List<Property> JournalEntries;
        public List<Property> Items;

        public bool Online = false;

        // -------- Initializator --------

        public Quest(string id)
        {
            ID = id;

            Conversations = new List<Conversation>();

            Events = new List<Property>();
            Conditions = new List<Property>();
            Objectives = new List<Property>();
            JournalEntries = new List<Property>();
            Items = new List<Property>();
        }

        // -------- Getters and Setters --------

        public List<Property> ConversationsProperties {
            get {
                List<Property> properties = new List<Property>();
                foreach (Conversation conversation in Conversations) properties.Add((Property)conversation);
                return properties;
            }
        }

        // -------- Research Engine --------

        public Conversation GetConversation(string npcName, int npcID)
        {
            foreach (Conversation conversation in Conversations)
            {
                if (conversation.NPCName.Equals(npcName) && conversation.NPCID == npcID) return conversation;
            }
            return null;
        }

        public Conversation GetConversation(string npcName)
        {
            foreach (Conversation conversation in Conversations)
            {
                if (conversation.NPCName.Equals(npcName)) return conversation;
            }
            return null;
        }

        // ----

        [Obsolete("Function is OUTDATED! Use GetProperty instead.")]
        public Property GetEvent(string id, string[] ignored = null)
        {
            foreach (Property property in Events)
            {
                if (ignored == null)
                {
                    if (property.ID.Equals(id)) return property;
                }
                else
                {
                    if (!ignored.Contains(property.ID) && property.ID.Equals(id)) return property;
                }

            }

            return null;
        }

        [Obsolete("Function is OUTDATED! Use GetProperty instead.")]
        public Property GetCondition(string id, string[] ignored = null)
        {
            foreach (Property property in Conditions)
            {
                if (ignored == null)
                {
                    if (property.ID.Equals(id)) return property;
                }
                else
                {
                    if (!ignored.Contains(property.ID) && property.ID.Equals(id)) return property;
                }

            }
            return null;
        }

        public Property GetObjective(string id, string[] ignored = null)
        {
            foreach (Property property in Objectives)
            {
                if (ignored == null)
                {
                    if (property.ID.Equals(id)) return property;
                }
                else
                {
                    if (!ignored.Contains(property.ID) && property.ID.Equals(id)) return property;
                }

            }
            return null;
        }

        public Property GetJournalEntry(string id, string[] ignored = null)
        {
            foreach (Property property in JournalEntries)
            {
                if (ignored == null)
                {
                    if (property.ID.Equals(id)) return property;
                }
                else
                {
                    if (!ignored.Contains(property.ID) && property.ID.Equals(id)) return property;
                }

            }
            return null;
        }

        public Property GetItem(string id, string[] ignored = null)
        {
            foreach (Property property in Items)
            {
                if (ignored == null)
                {
                    if (property.ID.Equals(id)) return property;
                }
                else
                {
                    if (!ignored.Contains(property.ID) && property.ID.Equals(id)) return property;
                }

            }
            return null;
        }

        // new style
        public Property GetProperty(PropertyType type, string id)
        {  
            List<Property> properties;

            if (type == PropertyType.Event) properties = Events;
            else if (type == PropertyType.Condition) properties = Conditions;

            else properties = Items;

            foreach (Property property in properties)
            {
                if (property.ID == id) return property;
            }

            return null;
        }

        public void AddProperty(PropertyType type, Property property)
        { 
            List<Property> properties = null;

            if (type == PropertyType.Event) properties = Events;
            else if (type == PropertyType.Condition) properties = Conditions;

            properties.Add(property);
        }

        // new style
        public Statement GetStatement(Conversation conversation, StatementType type, string id)
        {
            List<Statement> statements;

            if (type == StatementType.NPC) statements = conversation.NPCStatements;
            else statements = conversation.PlayerStatements;

            foreach (Statement statement in statements)
            {
                if (statement.ID == id) return statement;
            }

            return null;
        }

    }
}
