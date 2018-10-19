using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.structure.items
{
    public abstract class Enchant
    {
        private string name;

        // -------- Initializator --------

        public Enchant(string name)
        {
            this.name = name;
        }

        // -------- Access --------

        public string Name {
            get {
                return name;
            }
        }

        public string ExportName {
            get {
                return name.ToUpper().Replace(" ", "_");
            }
        }

        public string SaveName {
            get {
                return name.Replace(" ", "_");
            }
        }

    }
}
