using CAMS.Controllers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            List<int> freeComp = new List<int>();
            if (IsLabOccupied(lab))
                return 0;
            List<Task> tasks = new List<Task>();
            List<int> ids = lab.Computers.Select(e => e.ComputerId).ToList();
            foreach (int id in ids)
            {
                 tasks.Add(Task.Run(() => IsComputerAvilable(id, freeComp)));
                //IsComputerAvilable(id, freeComp);


            }
            Task.WaitAll(tasks.ToArray());
            return freeComp.Count();
        }

        private void IsComputerAvilable(int id, List<int> freeComp)
        {
            Activity act = _lController.CurrentActivityDetails(id);
            if (act == null || act.Mode==ActivityType.Off)
            {
                lock (freeComp)
                {
                    freeComp.Add(id);
                }

            }
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
            ComputersList.Add("Class004pc08,");
            ComputersList.Add("Class004pc09,");
            ComputersList.Add("Class004pc10,");
            ComputersList.Add("Class004pc11,");
            ComputersList.Add("Class004pc12,");
            ComputersList.Add("Class004pc13,");
            ComputersList.Add("Class004pc14,");
            ComputersList.Add("Class004pc15,");
            ComputersList.Add("Class004pc16,");
        }


        public ActivityType GetComputerState(Computer comp)
        {
            Activity currentAct = _lController.CurrentActivityDetails(comp.ComputerId);
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