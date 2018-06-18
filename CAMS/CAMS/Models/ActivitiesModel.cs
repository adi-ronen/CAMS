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
                    foreach (int lab in labs)
                    {
                     //   Task t = Task.Factory.StartNew(() =>
                      //  {
                            try
                            {
                                Debug.WriteLine("start of data collection for lab : " + lab);

                                Thread.CurrentThread.IsBackground = true;
                                GetComputerActivity(lab);
                                Debug.WriteLine("end of data collection for lab : " + lab);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("faild on lab:" + lab+ ". error is:" + ex.Message);
                            }
                    //    });
                    //    tasks.Add(t);
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

        private List<int> GetLabsIds()
        {
            return _aController.GetLabIds();
        }

        private void GetComputerActivity(int labId)
        {
            try
            {
                List<Computer> compList = _aController.GetLabComputers(labId);
                List<Task> tasks = new List<Task>();
                try
                {
                    foreach (var comp in compList)
                    {
                      //  Task t = Task.Factory.StartNew(() =>
                      //      {
                                try
                                {
                                    //check for the last activity of the computer
                                    ActivityType lastAct = _aController.CurrentActivityDetails(comp.ComputerId);

                                    // if the last activity is user activity from the day before- split to two activities for each day
                                    if (!lastAct.Equals(ActivityType.On))
                                    {
                                       _aController.SplitActivity(comp.ComputerId);
                                    }

                                    string logedOn = IsComputerLogedOn(comp.ComputerName, _aController.GetCompDomain(comp.ComputerId));
                                    // TBD- change the ugly T: isComputerLogedOn should return a boolean (in the func use trim)
                                    if (logedOn.Contains("T"))
                                    {
                                        String userName = "";
                                        try
                                        {
                                            userName = GetUserLogOn(comp.ComputerName, _aController.GetCompDomain(comp.ComputerId));
                                        }
                                        catch { }
                                        if (!Regex.Replace(userName, @"\s+", "").Equals(""))
                                        {
                                            ////computer is taken by user 'userName'- compare with last activity and update if neseccery 
                                            if (lastAct == ActivityType.On)
                                            {
                                                //create user activity
                                                AddNewActivity(comp.ComputerId, ActivityType.User, userName);
                                            }
                                            else if (_aController.GetCurrentComputerUser(comp.ComputerId) != userName)
                                            {
                                                //close current activity and create new user activity
                                                CloseActivity(comp.ComputerId);
                                                AddNewActivity(comp.ComputerId, ActivityType.User, userName);
                                            }
                                        }
                                        else
                                        {
                                            //computer is avilable- compare with last activity and update if neseccery 
                                            //if last activity is null- no need to update. if not close the last activity 
                                            if (lastAct != ActivityType.On)
                                            {
                                                CloseActivity(comp.ComputerId);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //computer is disconnected- compare with last activity and update if neseccery 
                                        if (lastAct == ActivityType.On)
                                        {
                                            //create off activity
                                            AddNewActivity(comp.ComputerId, ActivityType.Off, null);
                                        }
                                        else if (lastAct != ActivityType.Off)
                                        {
                                            //close current activity and create new off activity
                                            CloseActivity(comp.ComputerId);
                                            AddNewActivity(comp.ComputerId, ActivityType.Off, null);

                                        }
                                        // else- it already in off mode- dont change anything 
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("faild on computer:" + comp.ComputerName + ". error is:" + ex.Message);
                                }

                          //  });
                      //  tasks.Add(t);
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
            Dictionary<string, string> classes = new Dictionary<string, string>();

            foreach (string line in lines)
            {
                try
                {
                    //format: 96-003    11/06/2014 17:00 20:00    
                    char[] charSeparators = new char[] { ' ' };
                    string[] location_time = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    string lab = location_time[0];
                    string building = lab.Split('-')[0];
                    string room = lab.Split('-')[1];
                    string date = location_time[1];
                    string startTime = location_time[2];
                    string endTime = location_time[3];

                    DateTime day = DateTime.Parse(date);
                    DateTime activityStart = day.AddHours(int.Parse(startTime.Split(':')[0]));
                    DateTime activityEnd = day.AddHours(int.Parse(endTime.Split(':')[0]));
                    try
                    {
                        int labid=_aController.FindLabID(building, room);
                        _aController.CreateNewClassActivity(labid, activityStart, activityEnd);

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("couldn't find lab " + location_time[0]);
                    }



                    if (classes.ContainsKey(lab))
                    {
                        classes[lab] += "," + startTime + "-" + endTime;
                    }
                    else
                    {
                        classes[lab] = startTime + "-" + endTime;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("couldn't pars row :" + line);
                }

            }
            //clear old schedule
            _aController.ClearLabsSchedule();
            //add daily class Schedule for each lab
            foreach (var item in classes)
            {
                try
                {
                    string[] building_room = item.Key.Split('-');
                    int labid=_aController.FindLabID(building_room[0], building_room[1]);
                    _aController.UpdateLabSchedule(labid, item.Value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("couldn't find lab " + item.Key);
                }
            }
            
        }

        private List<Lab> GetLabs()
        {
            return _aController.GetAllLabs();
        }

        private string GetUserLogOn(string compNames, string domain)
        {
            String script = "(Get-WmiObject -Class win32_computersystem -ComputerName "+ compNames + "."+domain+ ".ad.bgu.ac.il).UserName";
            String ans = RunScript(script);
            return ans;
        }
        private string IsComputerLogedOn(String compName, string domain)
        {
            String script = "(Test-Connection -BufferSize 32 -Count 1 -ComputerName " + compName +"." + domain + ".ad.bgu.ac.il -Quiet)";
            return RunScript(script);
        }


        private void CloseActivity(int compid)
        {
            _aController.CloseActivity(compid);
        }

        private void AddNewActivity(int comp, ActivityType mode, string userName)
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