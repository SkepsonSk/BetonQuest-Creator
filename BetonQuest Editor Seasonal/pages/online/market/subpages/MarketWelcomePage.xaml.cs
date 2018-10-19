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

namespace BetonQuest_Editor_Seasonal.pages.online.market.subpages
{
    public partial class MarketWelcomePage : Page
    {
        public MarketWelcomePage()
        {
            InitializeComponent();
            AddNews("The adventure begins...", "Editor has finally been opened to the public, which means a lot of great events for the Community! Have fun!", "skepsonsk");
            AddNews("Live happilly ever after", "One of the developers is getting married! Let's show him our applause!", "skepsonsk");
        }

        public void AddNews(string title, string text, string author)
        {
            StackPanel panel = new StackPanel();

            TextBlock titleBlock = new TextBlock();
            TextBlock textBlock = new TextBlock();
            TextBlock authorBlock = new TextBlock();
            Separator separator = new Separator();

            titleBlock.Style = App.Current.Resources["DefaultText73"] as Style;
            textBlock.Style = App.Current.Resources["DefaultText8c"] as Style;
            authorBlock.Style = App.Current.Resources["DefaultText73"] as Style;

            titleBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.TextWrapping = TextWrapping.Wrap;
            authorBlock.TextWrapping = TextWrapping.Wrap;

            titleBlock.Text = title;
            textBlock.Text = text;
            authorBlock.Text = author;

            titleBlock.FontSize = 20;
            textBlock.FontSize = 18;
            authorBlock.FontSize = 16;

            authorBlock.Margin = new Thickness(0d, 5d, 0d, 10d);
            separator.Margin = new Thickness(0d, 0d, 0d, 20d);

            panel.Children.Add(titleBlock);
            panel.Children.Add(textBlock);
            panel.Children.Add(authorBlock);
            panel.Children.Add(separator);

            NewsPanel.Children.Add(panel);
        }

    }
}
