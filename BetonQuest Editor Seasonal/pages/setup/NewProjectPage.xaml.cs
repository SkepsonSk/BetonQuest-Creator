using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.pages.editor;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetonQuest_Editor_Seasonal.pages.setup
{
    public partial class NewProjectPage : Page
    {

        private bool continueButtonVisible = false;

        // -------- Initializator --------

        public NewProjectPage()
        {
            InitializeComponent();

            ContinueButton.Opacity = 0d;
            ContinueButton.IsEnabled = false;

            Tools.Animations.FadeIn(Page, .25d, null);
        }

        // -------- Events --------

        private void ReturnTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tools.Animations.FadeOut(Page, 0.25d, SwitchToMainPage);
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)     // Also initializes a new quest project.
        {
            string name = ProjectNameTextBox.Text;

            if (string.IsNullOrEmpty(name)) return;

            Project.CreateNewQuest(name);

            Tools.Animations.FadeOut(Page, 0.25d, SwitchToEditorHub);
        }

        private void ProjectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ProjectNameTextBox.Text;

            if (string.IsNullOrEmpty(text) && continueButtonVisible)
            {
                ContinueButton.IsEnabled = false;
                continueButtonVisible = false;

                Tools.Animations.FadeOut(ContinueButton, 0.25d, null);
            }
            else if (!string.IsNullOrEmpty(text) && !continueButtonVisible)
            {
                ContinueButton.IsEnabled = true;
                continueButtonVisible = true;

                Tools.Animations.FadeIn(ContinueButton, 0.25d, null);
            }
        }

        // ---- Switch Page Operations ----

        private void SwitchToMainPage(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayFrame.Navigate(new WelcomePage());
        }

        private void SwitchToEditorHub(object sender, EventArgs e)
        {
            MainWindow.Instance.DisplayFrame.Navigate(new EditorHub());
        }

    }
}
