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

namespace BetonQuest_Editor_Seasonal.controls
{
    public partial class View : UserControl
    {
        public delegate bool SpecialButtonCanBeShown(object[] data);

        private object[] data;

        private bool acceptsLeftClick;
        private bool acceptsRightClick;

        // -------- Initializator --------

        public View(string head, string body, object[] data, bool acceptsLeftClick = true, bool acceptsRightClick = true, bool specialButtonVisible = false, string specialButtonText = "NULL", RoutedEventHandler specialButtonClick = null)
        {
            InitializeComponent();

            this.data = data;

            this.acceptsLeftClick = acceptsLeftClick;
            this.acceptsRightClick = acceptsRightClick;

            if (specialButtonVisible)
            {
                SpecialButton.Content = specialButtonText;
                if (specialButtonClick != null) SpecialButton.Click += specialButtonClick;
                SpecialButton.Visibility = Visibility.Visible;

                SpecialButton.Tag = this;
            }

            Head.Text = head;
            Body.Text = body;
        }

        public View(string head, object[] data, bool acceptsLeftClick = true, bool acceptsRightClick = true, bool specialButtonVisible = false, string specialButtonText = "NULL", RoutedEventHandler specialButtonClick = null)
        {
            InitializeComponent();

            this.data = data;

            this.acceptsLeftClick = acceptsLeftClick;
            this.acceptsRightClick = acceptsRightClick;

            if (specialButtonVisible)
            {
                SpecialButton.Content = specialButtonText;
                if (specialButtonClick != null) SpecialButton.Click += specialButtonClick;
                SpecialButton.Visibility = Visibility.Visible;

                SpecialButton.Tag = this;
            }

            Head.Text = head;
            Body.Visibility = Visibility.Collapsed;
        }

        // -------- Access --------

        public object[] Data {
            get {
                return data;
            }
        }

        public void SetHeadFontSize(double fontSize)
        {
            Head.FontSize = fontSize;
        }
    }
}
