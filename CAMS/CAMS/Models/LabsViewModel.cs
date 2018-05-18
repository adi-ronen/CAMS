using CAMS.Controllers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CAMS.Constant;

namespace CAMS.Models
{
    public class LabsViewModel
    {
        public IPagedList<Lab> Labs;
        private BaseController _lController;


        public LabsViewModel(IPagedList<Lab> pagedList, BaseController controller)
        {
            this.Labs = pagedList;
            this._lController = controller;
        }
        public int NumberOfAvilableComputers(Lab lab)
        {
            int ans = 0;
            if (IsLabOccupied(lab))
                return ans;
            foreach (Computer comp in lab.Computers)
            {
                if (_lController.LastActivityDetails(comp) == null)
                {
                    ans++;
                }
            }

            return ans;
        }

        public bool IsLabOccupied(Lab lab)
        {
            if (lab.TodaysClasses == null)
                return false;
            string[] classes = lab.TodaysClasses.Trim(' ').Split(',');
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
    public class LabDetailsViewModel
    {

        public Lab Lab;
        private LabsController _lController;
        public List<string> ComputersList = new List<string>();

        public LabDetailsViewModel(Lab lab, LabsController labsController)
        {
            this.Lab = lab;
            this._lController = labsController;
            ComputersList.Add("Class004pc05,");
            ComputersList.Add("Class004pc06,");
            ComputersList.Add("Class004pc07,");
        }


        public ActivityType GetComputerState(Computer comp)
        {
            Activity currentAct = _lController.LastActivityDetails(comp.ComputerId);
            if (currentAct == null)
                return ActivityType.On;
            else
                return currentAct.Mode;

        }

        public bool IsLabOccupied()
        {
            if (Lab.TodaysClasses == null)
                return false;
            string[] classes = Lab.TodaysClasses.Trim(' ').Split(',');
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