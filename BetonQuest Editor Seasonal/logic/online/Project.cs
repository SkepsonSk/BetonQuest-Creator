using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.online
{
    public class Project
    {
        private string id;
        private string name;
        private string description;
        private string creatorID;
        private string creatorName;

        // -------- Initialization --------

        public Project(string id, string name, string description, string creatorID, string creatorName)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.creatorID = creatorID;
            this.creatorName = creatorName;
        }

        // -------- Access --------

        public string ID {
            get {
                return id;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public string Description {
            get {
                return description;
            }
        }

        public string CreatorID {
            get {
                return creatorID;
            }
        }

        public string CreatorName {
            get {
                return creatorName;
            }
        }

    }
}
