using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BetonQuest_Editor_Seasonal.logic.gcreator
{
    public class PanelConnection
    {
        private static Canvas workspace;

        private Control first;
        private Control second;

        private bool direction;

        private Shape arrowLine;
        private Line line;

        // --------

        public PanelConnection(Control first, Control second, bool direction = false)
        {
            this.first = first;
            this.second = second;

            this.direction = direction;

            if (direction) CreateArrow();
            else CreateLine();
        }

        // -------- Creating connection --------

        public void CreateArrow()
        {
            Point firstPoint = new Point(Canvas.GetLeft(first), Canvas.GetTop(first));
            Point secondPoint = new Point(Canvas.GetLeft(second), Canvas.GetTop(second));

            Console.WriteLine(firstPoint.X + " " + firstPoint.Y + " / " + secondPoint.X + " " + secondPoint.Y);

            firstPoint.X += first.Width / 2;
            firstPoint.Y += first.Height / 2;

            secondPoint.X += second.Width / 2;
            secondPoint.Y += second.Height / 2;

            arrowLine = ArrowGenerator.DrawLinkArrow(firstPoint, secondPoint);
            arrowLine.ContextMenu = CreateArrowContextMenu();

            Panel.SetZIndex(arrowLine, 0);

            workspace.Children.Add(arrowLine);
        }

        public void CreateLine()
        {
            line = new Line();

            line.Stroke = Brushes.Gray;
            line.StrokeThickness = 2d;

            Point firstPoint = new Point(Canvas.GetLeft(first), Canvas.GetTop(first));
            Point secondPoint = new Point(Canvas.GetLeft(second), Canvas.GetTop(second));

            line.X1 = firstPoint.X + first.Width / 2;
            line.X2 = secondPoint.X + second.Width / 2;
            line.Y1 = firstPoint.Y + first.Height / 2;
            line.Y2 = secondPoint.Y + second.Height / 2;

            Panel.SetZIndex(line, 0);

            workspace.Children.Add(line);
        }

        // ---- Updating the connection ----

        public void Update()
        {
            if (!direction)
            {
                Point firstPoint = first.TranslatePoint(new Point(0, 0), workspace);
                Point secondPoint = second.TranslatePoint(new Point(0, 0), workspace);

                line.X1 = firstPoint.X + first.Width / 2;
                line.X2 = secondPoint.X + second.Width / 2;
                line.Y1 = firstPoint.Y + first.Height / 2;
                line.Y2 = secondPoint.Y + second.Height / 2;
            }
            else
            {
                workspace.Children.Remove(arrowLine);
                CreateArrow();
            }
        }

        // -------- Deleting --------

        public void Delete()
        {
            if (arrowLine != null) workspace.Children.Remove(arrowLine);
            else if (line != null) workspace.Children.Remove(line);
        }

        // -------- Access --------

        public Control First {
            get {
                return first;
            }
        }

        public Control Second {
            get {
                return second;
            }
        }

        // --------

        public static void SetWorkspace(Canvas workspace)
        {
            PanelConnection.workspace = workspace;
        }

        public ContextMenu CreateArrowContextMenu()
        {
            ContextMenu arrowLineContextMenu = new ContextMenu();
            arrowLineContextMenu.Items.Add(new MenuItem().Header = "Break connection");

            return arrowLineContextMenu;
        }

    }
}
