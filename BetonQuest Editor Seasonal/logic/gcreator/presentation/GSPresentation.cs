using BetonQuest_Editor_Seasonal.controls.gcreator;
using BetonQuest_Editor_Seasonal.logic.structure;
using BetonQuest_Editor_Seasonal.logic.structure.conversating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.gcreator.presentation
{
    [Serializable]
    public class GSPresentation : PanelPresentation
    {
        public GSPresentation(GStatement statement)
        {
            Statement = statement.GetBoundProperty() as Statement;
            Type = statement.StatementType;
        }

        // -------- Access --------

        public Statement Statement { get; }
        public StatementType Type { get; }
    }
}
