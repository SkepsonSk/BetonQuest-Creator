using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetonQuest_Editor_Seasonal.logic.gcreator.presentation
{
    [Serializable]
    public class PointPresentation
    {
        public PointPresentation(double x, double y)
        {
            X = x;
            Y = y;
        }

        // -------- Access --------

        public double X { get; }

        public double Y { get; }

        public Point ToPoint() { return new Point(X, Y); }

    }
}
