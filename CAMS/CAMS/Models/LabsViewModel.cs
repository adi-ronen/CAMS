using CAMS.Controllers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
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
        public List<string> ComputersList;

        public LabDetailsViewModel(Lab lab, LabsController labsController)
        {
            this.Lab = lab;
            this._lController = labsController;
            ComputersList = GetComputerList(lab.Department.Domain);
        }

        private List<string> GetComputerList(string domain)
        {
            string[] SearchBaseDomain = domain.Split('.');
            for (int i = 0; i < SearchBaseDomain.Length; i++)
            {
                SearchBaseDomain[i] = "DC=" + SearchBaseDomain[i];
            }
            string SearchBase = string.Join(",",SearchBaseDomain);
            return RunScript("Get-ADComputer -Filter * -SearchBase \"DC=campus,DC =ad,DC=bgu,DC=ac,DC=il\" | select-object -expandproperty name");
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
        private List<string> RunScript(string scriptText)
        {
            // create Powershell runspace 
            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it 
            runspace.Open();

            // create a pipeline and feed it the script text 
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptText);

            // add an extra command to transform the script output objects into nicely formatted strings 
            // remove this line to get the actual objects that the script returns. For example, the script 
            // "Get-Process" returns a collection of System.Diagnostics.Process instances. 
            pipeline.Commands.Add("Out-String");

            // execute the script 
            Collection<PSObject> results = pipeline.Invoke();

            // close the runspace 
            runspace.Close();

            //convert to list
            string[] stringSeparators = new string[] { "\r\n" };
            string[] computerNames = results[0].ToString().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            List<string> ComputerList = computerNames.ToList();
            // return the results of the script that has 
            // now been converted to text 
            return ComputerList;
        }

    }
}