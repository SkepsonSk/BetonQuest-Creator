using BetonQuest_Editor_Seasonal.controls.gcreator;
using BetonQuest_Editor_Seasonal.logic.structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.gcreator.presentation
{
    [Serializable]
    public class GPPresentation : PanelPresentation
    {

        public string PropertyID { get; }

        public Property Property { get; }
        public PropertyType PropertyType { get; }

        // -------- Start --------

        public GPPresentation(GProperty property)
        {
            PropertyID = property.GetBoundProperty().ID;

            Property = property.GetBoundProperty() as Property;
            PropertyType = property.Type;
        }

    }
}
