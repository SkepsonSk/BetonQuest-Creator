using BetonQuest_Editor_Seasonal.logic.structure.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.structure
{
    public class Item
    {
        private string type;
        private byte data;

        private string customName;
        private string lore;

        private List<SingleEnchant> enchants;

        // -------- Initializator --------

        public Item(Property property)
        {
            Console.WriteLine(property.Command);

            string item = property.Command;

            string[] splitted = item.Split(new char[] { ' '});
            string[] command;

            type = splitted[0];

            for (int n = 1; n < splitted.Length; n++)
            {
                command = splitted[n].Split(new char[] { ':' }, 2);

                if (command[0].Equals("data")) data = byte.Parse(command[1]);
                if (command[0].Equals("name")) customName = command[1];
                if (command[0].Equals("lore")) lore = command[1];
                if (command[0].Equals("enchants"))
                {
                    this.enchants = new List<SingleEnchant>();

                    string[] enchants = command[1].Split(new char[] { ',' }), pattern;
                    for (int m = 0; m < enchants.Length; m++)
                    {
                        pattern = enchants[m].Split(new char[] { ':' });
                        this.enchants.Add(new SingleEnchant(pattern[0], byte.Parse(pattern[1])));
                    }
                }

            }

        }

        // -------- Access --------

        public string Type {
            get {
                return type;
            }
        }

        public byte Data {
            get {
                return data;
            }
        }

        public string CustomName {
            get {
                return customName;
            }
        }

        public string Lore {
            get {
                return lore;
            }
        }

        public List<SingleEnchant> Enchants {
            get {
                return enchants;
            }
        }

    }
}
