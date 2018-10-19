using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.structure.items
{
    public class SingleEnchant : Enchant
    {
        private static List<SingleEnchant> availablesEnchantSets = new List<SingleEnchant>();

        // ----

        private byte level;

        // -------- Initializator --------

        public SingleEnchant(string name, byte level) : base(name)
        {
            this.level = level;

        }

        // -------- Access --------

        public static List<SingleEnchant> AvailableEnchantSets {
            get {
                return availablesEnchantSets;
            }
        }

        public byte Level {
            get {
                return level;
            }
        }

        // ----

        public static SingleEnchant GetSingleEnchant(string name)
        {
            foreach (SingleEnchant singleEnchant in availablesEnchantSets)
            {
                if (singleEnchant.Name.Equals(name)) return singleEnchant;
            }
            return null;
        }
    }
}
