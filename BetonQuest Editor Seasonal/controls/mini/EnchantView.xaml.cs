using BetonQuest_Editor_Seasonal.logic.structure.items;
using System;
using System.Collections.Generic;
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

namespace BetonQuest_Editor_Seasonal.controls.mini
{
    public partial class EnchantView : UserControl
    {

        private EnchantSet enchantSet;

        private bool selected;
        private int selectedLevel;

        private Button[] levelButtons;

        // -------- Initializator --------

        public EnchantView(EnchantSet enchantSet, bool selected = false, int selectedLevel = 1)
        {
            InitializeComponent();

            this.enchantSet = enchantSet;

            this.selected = selected;
            this.selectedLevel = selectedLevel;

            Name.Text = enchantSet.Name;
            levelButtons = new Button[enchantSet.MaxLevel];

            Button button;
            for (int n = 1; n < enchantSet.MaxLevel + 1; n++)
            {
                button = new Button();
                button.Click += LevelSelect;
                button.Content = n.ToString();

                levelButtons[n - 1] = button;

                Level.Children.Add(button);
            }

            Refresh();
        }

        public void Refresh()
        {
            if (selected)
            {
                Background = new SolidColorBrush(Colors.ForestGreen);
                Name.Foreground = new SolidColorBrush(Colors.White);

                Separator.Background = new SolidColorBrush(Colors.White);

                for (int n = 0; n < levelButtons.Length; n++)
                {
                    levelButtons[n].Foreground = new SolidColorBrush(Colors.White);
                }

                if (selectedLevel > 0)
                {
                    levelButtons[selectedLevel - 1].Foreground = new SolidColorBrush(Color.FromRgb(255, 153, 51));
                }

            }
            else
            {
                Background = new SolidColorBrush(Color.FromRgb(166, 166, 166));
                Name.Foreground = new SolidColorBrush(Color.FromRgb(115, 115, 115));

                Separator.Background = new SolidColorBrush(Color.FromRgb(115, 115, 115));

                for (int n = 0; n < levelButtons.Length; n++)
                {
                    levelButtons[n].Foreground = new SolidColorBrush(Color.FromRgb(115, 115, 115));
                }
            }
        }

        // --------

        public EnchantSet EnchantSet {
            get {
                return enchantSet;
            }
        }

        public bool Selected {
            get {
                return selected;
            }
            set {
                selected = value;
            }
        }

        public int SelectedLevel {
            get {
                return selectedLevel;
            }
            set {
                selectedLevel = value;
            }
        }

        // -------- Events --------

        private void Select(object sender, MouseButtonEventArgs e)
        {
            selected = !selected;
            Refresh();
        }

        private void LevelSelect(object sender, RoutedEventArgs e)
        {
            if (!this.selected) return;

            Button selected = sender as Button;
            selectedLevel = int.Parse(selected.Content as string);

            Refresh();
        }

        // --------


    }
}
