using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.structure.items
{
    public class EnchantSet : Enchant
    {
        private byte maxLevel;

        // -------- Initializator --------

        public EnchantSet(string name, byte maxLevel) : base(name)
        {
            this.maxLevel = maxLevel;
        }

        // -------- Access --------

            
        public byte MaxLevel {
            get {
                return maxLevel;
            }
        }

    }
}
