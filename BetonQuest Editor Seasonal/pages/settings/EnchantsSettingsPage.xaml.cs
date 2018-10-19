using BetonQuest_Editor_Seasonal.controls;
using BetonQuest_Editor_Seasonal.controls.enchants;
using BetonQuest_Editor_Seasonal.logic.settings;
using BetonQuest_Editor_Seasonal.logic.structure.items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetonQuest_Editor_Seasonal.pages.settings
{

    public partial class EnchantsSettingsPage : Page
    {
        private EnchantPack editedEnchantPack;
        private EnchantSet editedEnchantSet;

        // -------- Initializator --------

        public EnchantsSettingsPage()
        {
            InitializeComponent();

            RefreshEnchantPacks();
            RefreshEnchantPackEnchantSets();
        }

        public void RefreshEnchantPacks()
        {
            EnchantPacks.Children.Clear();
            if (EnchantPack.LoadedEnchantPacks.Count == 0) EnchantPacks.Children.Add(new NoPropertyView("No loaded enchant packs!"));
            else
            {
                EnchantPackView enchantPackView;

                foreach (EnchantPack enchantPack in EnchantPack.LoadedEnchantPacks)
                {
                    enchantPackView = new EnchantPackView(enchantPack);
                    enchantPackView.MouseDown += EnchantPackView_MouseDown;

                    EnchantPacks.Children.Add(enchantPackView);
                }
                if (editedEnchantPack != null) RefreshEnchantPackEnchantSets();
            }
        }

        public void RefreshEnchantPackEnchantSets()
        {
            Enchants.Children.Clear();

            if (editedEnchantPack == null) Enchants.Children.Add(new NoPropertyView("Select a pack"));
            else
            {
                if (editedEnchantPack.Enchants.Count == 0) Enchants.Children.Add(new NoPropertyView("No enchants in the pack"));
                foreach (EnchantSet enchantSet in editedEnchantPack.Enchants) Enchants.Children.Add(new EnchantEditingView(enchantSet));
            }
        }

        public void RefreshEnchantSetEditor()
        {

        }

        // -------- Events --------

        private void AddEnchantPackButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ID.Text)) return;

            editedEnchantPack = new EnchantPack(ID.Text);
            EnchantPack.LoadedEnchantPacks.Add(editedEnchantPack); // may cause errors!

            RefreshEnchantPacks();
        }
        
        private void AddEnchantToPackButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EnchantName.Text) || string.IsNullOrEmpty(EnchantMaxLevel.Text)) return;
            if (editedEnchantPack == null) return;

            string name = EnchantName.Text;
            byte maxLevel = byte.Parse(EnchantMaxLevel.Text);

            editedEnchantPack.Enchants.Add(new EnchantSet(name, maxLevel));

            RefreshEnchantPackEnchantSets();

        }

        private void SavePackButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BetonQuest Editor";
            if (File.Exists(path + @"\enchants\" + editedEnchantPack.ID + ".txt")) File.Delete(path + @"\enchants\" + editedEnchantPack.ID + ".txt");

            string[] enchants = new string[editedEnchantPack.Enchants.Count + 1];
            enchants[0] = editedEnchantPack.Enabled.ToString();

            for (int n = 1; n < enchants.Length + 1; n++)
            {
                enchants[n] = editedEnchantPack.Enchants[n].SaveName + "-" + editedEnchantPack.Enchants[n].MaxLevel;
            }

            File.Create(path + @"\enchants\" + editedEnchantPack.ID + ".txt").Close();
            File.WriteAllLines(path + @"\enchants\" + editedEnchantPack.ID + ".txt", enchants);
        }

        // ----
    
        private void EnchantPackView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EnchantPackView enchantPackView = sender as EnchantPackView;
            editedEnchantPack = enchantPackView.EnchantPack;

            EnchantPackID.Text = editedEnchantPack.ID;

            RefreshEnchantPackEnchantSets();
        }

        private void EnchantEditingView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

    }
}
