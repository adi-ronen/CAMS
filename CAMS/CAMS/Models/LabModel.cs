using CAMS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CAMS.Constant;

namespace CAMS.Models
{
    public class LabModel
    {

        public Lab Lab;
        private LabsController _lController;

        public LabModel(Lab lab,LabsController labsController)
        {
            this.Lab = lab;
            this._lController = labsController;
        }


        public ActivityMode GetComputerState(Computer comp)
        {
            Activity currentAct = _lController.LastActivityDetails(comp.ComputerId);
            if (currentAct == null)
                return ActivityMode.On;
            else if (currentAct.Mode.Trim(' ').Equals(ActivityMode.Off.ToString()))
                return ActivityMode.Off;
            else
                return ActivityMode.User; 

        }

        public bool IsLabOccupied()
        {
            if (Lab.TodaysClasses == null)
                return false;
            string[] classes=Lab.TodaysClasses.Trim(' ').Split(',');
            DateTime now = DateTime.Now;
            //format of todayClasses: XX:XX-XX:XX,XX:XX,XX:XX,XX:XX
            foreach (string c in classes)
            {
                string start = c.Split('-')[0];
                string end = c.Split('-')[1];
                var dateTimeStart = DateTime.ParseExact(start, "H:mm", null, System.Globalization.DateTimeStyles.None);
                var dateTimeEnd = DateTime.ParseExact(end, "H:mm", null, System.Globalization.DateTimeStyles.None);

                if (now > dateTimeStart && now < dateTimeEnd)
                    return true;
            }

            return false;
        }
    }
}