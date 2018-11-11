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

namespace BetonQuest_Editor_Seasonal.pages.management
{
    public partial class ExploratorPage : Page
    {

        public ExploratorPage(string path)
        {
            InitializeComponent();

            if (!Directory.Exists(path)) return;

            TextBlock discDrive;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                discDrive = new TextBlock();
                discDrive.Text = drive.Name;
                discDrive.Style = FindResource("DefaultText73") as Style;
                discDrive.FontSize = 20d;

                Disks.Children.Add(discDrive);
            }

            Open(path);
        }

        public void Open(string path)
        {
            Files.Children.Clear();

            TextBlock name;

            foreach (string directory in Directory.GetDirectories(path))
            {
                name = new TextBlock();
                name.Text = directory.Substring(directory.LastIndexOf(@"\") + 1);
                name.Style = FindResource("DefaultText73") as Style;
                name.FontSize = 20d;
                name.Foreground = Brushes.Orange;

                Files.Children.Add(name);
            }

            foreach (string file in Directory.GetFiles(path))
            {
                name = new TextBlock();
                name.Text = file.Substring(file.LastIndexOf(@"\") + 1);
                name.Style = FindResource("DefaultText73") as Style;
                name.FontSize = 20d;

                Files.Children.Add(name);
            }
        }
    }
}
