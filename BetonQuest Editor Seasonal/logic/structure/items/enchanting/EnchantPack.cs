using BetonQuest_Editor_Seasonal.logic.structure.items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.settings
{
    public class EnchantPack
    {
        private static List<EnchantPack> loadedEnchantsPacks = new List<EnchantPack>();
        private static List<EnchantSet> availableEnchantSets = new List<EnchantSet>();

        // ----

        private string id;
        private bool enabled;

        private List<EnchantSet> enchants;

        // -------- Initializator --------

        public EnchantPack(string id, bool enabled = false)
        {
            this.id = id;
            this.enabled = enabled;

            enchants = new List<EnchantSet>();
        }

        // -------- Access --------

        public static List<EnchantPack> LoadedEnchantPacks {
            get {
                return loadedEnchantsPacks;
            }
        }

        public static List<EnchantSet> EnchantSets {
            get {
                return availableEnchantSets;
            }
        }

        public string ID {
            get {
                return id;
            }
            set {
                id = value;
            }
        }
        
        public bool Enabled {
            get {
                return enabled;
            }
            set {
                enabled = value;
            }
        }

        public List<EnchantSet> Enchants {
            get {
                return enchants;
            }
        }

        // ----

        public static EnchantSet GetEnchant(string name)
        {
            foreach (EnchantSet enchantSet in EnchantSets)
            {
                if (enchantSet.Name.ToLower().Equals(name.ToLower())) return enchantSet;
            }
            return null;
        }

        // ----

        public static void LoadEnchantPacks()
        {
            loadedEnchantsPacks.Clear();
            availableEnchantSets.Clear();

            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor\enchants\";

            EnchantPack enchantPack;
            EnchantSet enchantSet;

            string[] file, enchantData;

            string fileName, id;
            bool enabled;

            foreach (string name in Directory.GetFiles(directory))
            {
                file = File.ReadAllLines(name);

                fileName = name.Substring(name.LastIndexOf('\\') + 1);
                id = fileName.Substring(0, fileName.Length - 4);

                enabled = bool.Parse(file[0]);

                enchantPack = new EnchantPack(id, enabled);
                LoadedEnchantPacks.Add(enchantPack);

                if (!enabled) continue;

                for (int n = 1; n < file.Length; n++)
                {
                    enchantData = file[n].Split(new char[] { '-' });

                    enchantSet = new EnchantSet(enchantData[0].Replace('_', ' '), byte.Parse(enchantData[1]));

                    enchantPack.Enchants.Add(enchantSet);
                    availableEnchantSets.Add(enchantSet);
                }
            }

        }
    }
}
