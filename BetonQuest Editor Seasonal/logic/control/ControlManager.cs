using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.control
{
    public class ControlManager
    {
        private static List<ControllablePage> registeredPages = new List<ControllablePage>();

        // -------- Access --------

        public static List<ControllablePage> RegisteredPages {
            get {
                return registeredPages;
            }
        }

        public static void ReloadAllPages()
        {
            foreach (ControllablePage page in registeredPages) page.ReloadProperties();
        }

    }
}
