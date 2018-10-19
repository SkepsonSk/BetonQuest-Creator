using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BetonQuest_Editor_Seasonal.logic.colors
{
    public class ColoredText
    {

        private char[] letters;
        private Brush[] colors;
        
        // -------- Initializator --------

        public ColoredText(string text)
        {
            letters = text.ToCharArray();
            colors = new Brush[letters.Length];
        }

        // -------- Access --------

        public void SetText(string text)
        {
            letters = text.ToCharArray();
        }

        public void Color(int start, int end, Brush color)
        {
            for (int n = start; n < end; n++) colors[n] = color;
        }

        // ----

        public override string ToString()
        {
            StringBuilder coloredText = new StringBuilder();

            for (int n = 0; n < letters.Length; n++)
            {
                if (colors[n] == null) coloredText.Append(letters[n]);
                else coloredText.Append(ColorConverter.Convert(colors[n])).Append(letters[n]);
            }

            return coloredText.ToString();
        }

    }
}
