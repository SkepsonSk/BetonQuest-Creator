using BetonQuest_Editor_Seasonal.controls.gcreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.gcreator.presentation
{
    [Serializable]
    public class PanelConnectionPresentation
    {
        public PanelPresentation First { get; }
        public PanelPresentation Second { get; }

        // -------- Access --------

        public PanelConnectionPresentation(PanelPresentation first, PanelPresentation second)
        {
            First = first;
            Second = second;
        }

    }
}
