using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonQuest_Editor_Seasonal.logic.alerting
{
    public class Alert
    {
        private AlertType alertType;

        private bool alertBarVisible;
        private bool alertBarPriorityAlert;

        // -------- Initializator --------

        public Alert()
        {
            alertBarVisible = false;
            alertBarPriorityAlert = false;
        }

    }
}
