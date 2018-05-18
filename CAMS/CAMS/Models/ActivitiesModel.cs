using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;
using CAMS.Controllers;
using static CAMS.Constant;

namespace CAMS.Models
{
    public class ActivitiesModel
    {
        ActivitiesController _aController;
        //TBD- add arguments to Repoet details + properties so it can be bind from Views/Activities/Create (Create Report)
        public ActivitiesModel(ActivitiesController activitiesController)
        {
            _aController = activitiesController;
        }

        /// <summary>
        /// get current cumputer activity. 
        /// </summary>
        public void GetComputersActivity()
        {
            //List<Lab> labs = GetLabs();
            List<Lab> labs = new List<Lab>();
            foreach (Lab lab in labs)
            {
                GetComputerActivity(lab.Computers);
            }
        }

        private void GetComputerActivity(ICollection<Computer> compList)
        {

            //TBD- make it asyncronic!
            foreach (var comp in compList)
            {
                //check for the last activity of the computer
                Activity lastAct = _aController.LastActivityDetails(comp.ComputerId);
                
                // if the last activity is user activity from the day before- split to two activities for each day
                if(lastAct!=null && lastAct.Mode.Equals(ActivityType.User) && !lastAct.Login.Date.Equals(DateTime.Now.Date))
                {
                    lastAct = _aController.SplitActivity(lastAct);
                }

                string logedOn = IsComputerLogedOn(comp.ComputerName);
                // TBD- change the ugly T: isComputerLogedOn should return a boolean (in the func use trim)
                if (logedOn.Contains("T"))
                {
                    String userName = GetUserLogOn(comp.ComputerName);
                    if (!Regex.Replace(userName, @"\s+", "").Equals(""))
                    {
                        ////computer is taken by user 'userName'- compare with last activity and update if neseccery 
                        if (lastAct == null)
                        {
                            //create user activity
                            AddNewActivity(comp, ActivityType.User, userName);
                        }
                        else if (lastAct.UserName != userName)
                        {
                            //close current activity and create new user activity
                            CloseActivity(lastAct);
                            AddNewActivity(comp, ActivityType.User, userName);
                        }
                    }
                    else
                    {
                        //computer is avilable- compare with last activity and update if neseccery 
                        //if last activity is null- no need to update. if not close the last activity 
                        if (lastAct != null)
                        {
                            CloseActivity(lastAct);
                        }
                    }
                }
                else
                {
                    //computer is disconnected- compare with last activity and update if neseccery 
                    if (lastAct == null)
                    {
                        //create off activity
                        AddNewActivity(comp, ActivityType.Off, null);
                    }
                    else if (lastAct.Mode !=ActivityType.Off)
                    {
                        //close current activity and create new off activity
                        CloseActivity(lastAct);
                        AddNewActivity(comp, ActivityType.Off, null);

                    }
                    // else- it already in off mode- dont change anything 
                }
            }
            
        }


        //TBD - איסוף שיבוץ חדרים
        public void GetClassesSchedule()
        {
            List<Lab> labList = GetLabs();
            //open connection with classes pacment system databse
            foreach (Lab lab in labList)
            {
                //get the classes that are plenned in this lab for today

                //string classes = "";

                //_aController.UpdateLabSchedule(lab, classes);




            }
        }

        private List<Lab> GetLabs()
        {
            return _aController.GetAllLabs();
        }

        private string GetUserLogOn(string compNames)
        {
            String script = "(Get-WmiObject -Class win32_computersystem -ComputerName "+ compNames + ").UserName";
            String ans = RunScript(script);
            return ans;
        }
        private string IsComputerLogedOn(String compName)
        {
            String script = "(Test-Connection -BufferSize 32 -Count 1 -ComputerName " + compName + " -Quiet)";
            return RunScript(script);
        }


        private void CloseActivity(Activity lastAct)
        {
            _aController.CloseActivity(lastAct);
        }

        private void AddNewActivity(Computer comp, ActivityType mode, string userName)
        {

            _aController.CreateNewActivity(comp, mode, userName);
        }




        private string RunScript(string scriptText)
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

            // convert the script result into a single string 
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            // return the results of the script that has 
            // now been converted to text 
            return stringBuilder.ToString();
        }
        



    }
}