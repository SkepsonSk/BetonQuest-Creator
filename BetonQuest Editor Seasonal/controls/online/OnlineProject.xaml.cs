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

namespace BetonQuest_Editor_Seasonal.controls.online
{
    public partial class OnlineProject : UserControl
    {

        // -------- Initialization --------

        public OnlineProject(string name, string description, string creator)
        {
            InitializeComponent();

            Name.Text = name;
            Description.Text = description;
            Creator.Text = "Created by " + creator;

            Height = 95;
        }

        // -------- Events --------

        private void Project_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Height == 130) return;

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = Height;
            animation.To = 130;
            animation.Duration = TimeSpan.FromSeconds(.25d);

            Tools.Animations.FadeIn(Actions, .75d, null);

            BeginAnimation(HeightProperty, animation);
        }

        private void Project_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Height == 95) return;

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = Height;
            animation.To = 95;
            animation.Duration = TimeSpan.FromSeconds(.25d);
            BeginAnimation(HeightProperty, animation);

            Tools.Animations.FadeOut(Actions, .1d, null);
        }

        // -------- Event-Based Operations ---------

        private void SlideAway(object sender, EventArgs e)
        {

        }

    }
}
