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
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

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
        {try
            {
                List<int> labs = GetLabsIds();
                //  List<Lab> labs = new List<Lab>();
                //  labs.Add(_aController.GetLab(53));
                List<Task> tasks = new List<Task>();
                try
                {
                    foreach (int labId in labs)
                    {
                        Task t = Task.Factory.StartNew(() =>
                        {
                            try
                            {
                               // Debug.WriteLine("start of data collection for lab in: " + lab.Building + ", " + lab.RoomNumber);
                                Debug.WriteLine("start of data collection for lab : " + labId);

                                Thread.CurrentThread.IsBackground = true;
                                GetComputerActivity(labId);
                                //Debug.WriteLine("end of data collection for lab in: " + lab.Building + ", " + lab.RoomNumber);
                                Debug.WriteLine("end of data collection for lab : " +labId);

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("faild on lab:" + labId  + ". error is:" + ex.Message);
                            }
                        });
                        tasks.Add(t);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("faild in lab foreach. error is:" + ex.Message);
                }
                Task.WaitAll(tasks.ToArray());
                Debug.WriteLine("done");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("faild in lab waitall. error is:" + ex.Message);
            }
        }

        private void GetComputerActivity(int labId )
        {
            try
            {
                List<Task> tasks = new List<Task>();
                List<Computer> compList = _aController.GetLabComputers(labId);
                try
                {
                    foreach (var comp in compList)
                    {
                        Task t = Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                //check for the last activity of the computer
                                Activity lastAct = _aController.CurrentActivityDetails(comp.ComputerId);

                                // if the last activity is user activity from the day before- split to two activities for each day
                                if (lastAct != null && lastAct.Mode.Equals(ActivityType.User) && !lastAct.Login.Date.Equals(DateTime.Now.Date))
                                    {
                                        lastAct = _aController.SplitActivity(lastAct);
                                    }

                                    string logedOn = IsComputerLogedOn(comp.ComputerName, _aController.GetCompDomain(comp.ComputerId));
                                // TBD- change the ugly T: isComputerLogedOn should return a boolean (in the func use trim)
                                if (logedOn.Contains("T"))
                                    {
                                        String userName = GetUserLogOn(comp.ComputerName, _aController.GetCompDomain(comp.ComputerId));
                                        if (!Regex.Replace(userName, @"\s+", "").Equals(""))
                                        {
                                        ////computer is taken by user 'userName'- compare with last activity and update if neseccery 
                                        if (lastAct == null)
                                            {
                                            //create user activity
                                            AddNewActivity(comp.ComputerId, ActivityType.User, userName);
                                            }
                                            else if (lastAct.UserName != userName)
                                            {
                                            //close current activity and create new user activity
                                            CloseActivity(lastAct);
                                                AddNewActivity(comp.ComputerId, ActivityType.User, userName);
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
                                        AddNewActivity(comp.ComputerId, ActivityType.Off, null);
                                        }
                                        else if (lastAct.Mode != ActivityType.Off)
                                        {
                                        //close current activity and create new off activity
                                        CloseActivity(lastAct);
                                            AddNewActivity(comp.ComputerId, ActivityType.Off, null);

                                        }
                                    // else- it already in off mode- dont change anything 
                                }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("faild on computer:" + comp.ComputerName + ". error is:" + ex.Message);
                                }

                            });
                        tasks.Add(t);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("faild in computer foreach. error is:" + ex.Message);
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("faild in computer waitall. error is:" + ex.Message);
            }
        }


        //TBD - איסוף שיבוץ חדרים
        public void GetClassesSchedule()
        {
            List<Lab> labList = GetLabs();
            string[] lines = System.IO.File.ReadAllLines(@"D:\olladi\free_class.txt");

            foreach (string line in lines)
            {
                try
                {
                    //96-003[0]    11/06/2014[1] 17:00[2] 20:00[3]      
                    char[] charSeparators = new char[] { ' ' };
                    string[] location_time = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    string building = location_time[0].Split('-')[0];
                    string room = location_time[0].Split('-')[1];

                    DateTime day = DateTime.Parse(location_time[1]);

                    DateTime activityStart = day.AddHours(int.Parse(location_time[2].Split(':')[0]));
                    DateTime activityEnd = day.AddHours(int.Parse(location_time[3].Split(':')[0]));
                    try
                    {
                        _aController.FindLabID(building, room);

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("couldn't find lab " + location_time[0]);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("couldn't pars row :" + line);
                }

            }

        }

        private List<Lab> GetLabs()
        {
            return _aController.GetAllLabs();
        }
        private List<int> GetLabsIds()
        {
            return _aController.GetAllLabs().Select(e => e.LabId).ToList(); ;
        }

        private string GetUserLogOn(string compNames, string domain)
        {
            String script = "(Get-WmiObject -Class win32_computersystem -ComputerName "+ compNames + ".).UserName";
            String ans = RunScript(script);
            return ans;
        }
        private string IsComputerLogedOn(String compName,string domain)
        {
            String script = "(Test-Connection -BufferSize 32 -Count 1 -ComputerName " + compName + " -Quiet)";
            return RunScript(script);
        }


        private void CloseActivity(Activity lastAct)
        {
            _aController.CloseActivity(lastAct);
        }

        private void AddNewActivity(int computerId, ActivityType mode, string userName)
        {

            _aController.CreateNewActivity(computerId, mode, userName);
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