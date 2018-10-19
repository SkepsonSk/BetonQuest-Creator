using BetonQuest_Editor_Seasonal.logic.structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.gcreator
{
    public interface IPanel
    {
        List<PanelConnection> GetPanelConnections();

        Property GetBoundProperty();
        void BindProperty(Property property);
    }
}
