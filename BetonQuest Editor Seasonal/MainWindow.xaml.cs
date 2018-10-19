using BetonQuest_Editor_Seasonal.logic;
using BetonQuest_Editor_Seasonal.logic.control;
using BetonQuest_Editor_Seasonal.logic.settings;
using BetonQuest_Editor_Seasonal.logic.structure.items;
using BetonQuest_Editor_Seasonal.pages;
using BetonQuest_Editor_Seasonal.pages.setup;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BetonQuest_Editor_Seasonal
{
    public partial class MainWindow : Window
    {

        private static MainWindow instance;

        // ----

        private Page pageToLoad;
        private double animationTime;
        public delegate void OnNavigatorFree();
        public OnNavigatorFree NavigatorFree;

        private const double ACTIONBAR_HEIGHT = 36.28d;
        private const double COLOR_BAR_HEIGHT = 45d;

        public delegate void OnColorSelect(Brush color);
        public static OnColorSelect ColorSelect;

        // -------- Initialization --------

        public MainWindow()
        {
            InitializeComponent();

            instance = this;

            Project.VerifyDirectories();
            Project.VerifyEnchantDefaultFiles();

            EnchantPack.LoadEnchantPacks();

            DisplayFrame.Navigate(new WelcomePage());

            InitializeColorBar();

            ActionBar.Height = 0;
        }

        // -------- Navigator --------

        public void Navigate(Page pageToLoad, double animationTime = .25d, bool animated = true)
        {
            if (!animated) DisplayFrame.Navigate(pageToLoad);
            else
            {
                this.pageToLoad = pageToLoad;
                this.animationTime = animationTime;

                Tools.Animations.FadeOut(DisplayFrame, animationTime, SwitchPage);
            }
        }

        private void SwitchPage(object sender, EventArgs e)
        {
            DisplayFrame.Navigate(pageToLoad);

            if (NavigatorFree != null) Tools.Animations.FadeIn(DisplayFrame, animationTime, NavigationFinished);
            else Tools.Animations.FadeIn(DisplayFrame, animationTime, null);      
        }

        private void NavigationFinished(object sender, EventArgs e)
        {
            NavigatorFree();
            NavigatorFree = null;
        }

        // -------- Alerting System (AS) --------

        // -------- Message Communication System (SWA compatibility) --------

        public void ShowMessageFrame(string title, string message)
        {
            MessageFrame.Visibility = Visibility.Visible;
            MessageFrame.Opacity = 0d;

            Tools.Animations.FadeIn(MessageFrame, .25d, null);

            BlurEffect effect = new BlurEffect();
            effect.Radius = 0d;

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0d;
            animation.To = 10d;
            animation.Duration = TimeSpan.FromSeconds(0.5d);

            effect.BeginAnimation(BlurEffect.RadiusProperty, animation);

            DisplayFrame.Effect = effect;
            ActionBar.Effect = effect;
        }

        // ----

        public void HideMessageFrame()
        {
            Tools.Animations.FadeOut(MessageFrame, .25d, ChangeMessageFrameVisibility);

            BlurEffect effect = DisplayFrame.Effect as BlurEffect;

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 10d;
            animation.To = 0d;
            animation.Duration = TimeSpan.FromSeconds(0.5d);

            effect.BeginAnimation(BlurEffect.RadiusProperty, animation);

            DisplayFrame.Effect = effect;
            ActionBar.Effect = effect;
        }

        private void ChangeMessageFrameVisibility(object sender, EventArgs e)
        {
            MessageFrame.Visibility = Visibility.Collapsed;
        }

        // ----

        private void CloseMessageFrame_MouseDown(object sender, RoutedEventArgs e)
        {
            HideMessageFrame();
        }

        // -------- Color Bar --------

        private void InitializeColorBar()
        {
            foreach (UIElement element in Colors.Children)
            {
                element.MouseDown += Element_MouseDown;
            }
        }

        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle) ColorSelect((sender as Rectangle).Fill);
        }

        // --------

        public void ShowColorBar()
        {
            ColorBar.Visibility = Visibility.Visible;
            Tools.Animations.SlideDown(ColorBar, COLOR_BAR_HEIGHT, .25d, null);
        }

        public void HideColorBar()
        {
            Tools.Animations.SlideUp(ColorBar, COLOR_BAR_HEIGHT, .25d, CollapseColorBar);
        }

        private void CollapseColorBar(object sender, EventArgs e)
        {
            ColorBar.Visibility = Visibility.Collapsed;
        }

        // -------- Access --------

        public static MainWindow Instance {

            get {
                return instance;
            }

        }

    }
}
