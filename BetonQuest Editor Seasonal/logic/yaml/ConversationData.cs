using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.yaml
{
    public class ConversationData
    {

        public string NPCName { get; }
        public int NPCID { get; }

        // -------- Start --------

        public ConversationData(string npcName, int npcID)
        {
            NPCName = npcName;
            NPCID = npcID;
        }


    }
}
