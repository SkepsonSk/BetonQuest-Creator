using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.structure
{
    [Serializable]
    public class Property : ICloneable
    {
        private string id;
        private string command;

        // -------- Initializator --------

        public Property(string id, string command)
        {
            this.id = id;
            this.command = command;
        }

        // -------- Getters -------

        public string ID {
            get {
                return id;
            }
            set {
                id = value;
            }
        }

        public string Command {
            get {
                return command;
            }
            set {
                command = value;
            }
        }

        // -------- Comparing and Cloning --------

        public override bool Equals(object o)
        {
            if (!(o is Property))
            {
                return false;
            }

            Property property = (Property) o ;
            
            if (!id.Equals(property.id) || !command.Equals(property.command))
            {
                return false;
            }

            return true;
        }

        public object Clone()
        {
            return new Property(String.Copy(id), String.Copy(command));
        }

    }
}
