﻿using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.structure
{
    public class QuestDataImage
    {
        private string name;

        private List<Conversation> conversations;

        private List<Property> events;
        private List<Property> conditions;
        private List<Property> objectives;
        private List<Property> journalEntries;
        private List<Property> items;

        // -------- Initializator --------

        public QuestDataImage(Quest quest)
        {
            name = quest.Name.Clone() as string;        }

        // -------- Access --------

        public void Apply(Quest quest)
        {
            quest.Name = name;

            quest.Conversations = conversations;

            quest.Events = events;
            quest.Conditions = conditions;
            quest.Objectives = objectives;
            quest.JournalEntries = journalEntries;
            quest.Items = items;
        }

    }
}
