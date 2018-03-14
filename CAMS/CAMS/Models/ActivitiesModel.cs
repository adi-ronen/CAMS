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
        CAMS_DatabaseEntities _db = new CAMS_DatabaseEntities();
        ComputersController _cController = new ComputersController();
        ActivitiesController _aController = new ActivitiesController();

        /// <summary>
        /// get current cumputer activity for a computer list using active directory. 
        /// </summary>
        /// <param name="ComputerList"></param>
        public String GetComputersActivity(List<Computer> ComputerList)
        {
            
            string ans = "";
            foreach (var comp in ComputerList)
            {
                //check for the last activity of the computer
                Activity lastAct=_cController.LastActivityDetails(comp.ComputerId);


                string logedOn = IsComputerLogedOn(comp.ComputerName);
                if (logedOn.Contains("T"))
                {
                    ans += " " + comp.ComputerName + ": ON ";
                    String userName = GetUserLogOn(comp.ComputerName);
                    if (!Regex.Replace(userName, @"\s+", "").Equals(""))
                    {
                        ////computer is taken by user 'userName'- compare with last activity and update if neseccery 
                        ans += "- user: " + userName;
                    }
                    else
                    {
                        //computer is avilable- compare with last activity and update if neseccery 
                        ans += "- user: none";
                    }
                }
                else
                {
                    //computer is disconnected- compare with last activity and update if neseccery 

                    if (lastAct == null)
                    {
                        //new off activity
                        Activity act = new Activity();
                        act.Mode = ActivityMode.Off.ToString();
                        act.Login = DateTime.Now;
                        act.ComputerId = comp.ComputerId;
                        act.Computer = comp;
                        _cController.AddActivity(act, comp);
                    }
                    else if(lastAct.Mode!= ActivityMode.Off.ToString())
                    {
                        
                    }
                    ans += " " + comp.ComputerName + ": Disconected";
                }
            }

            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Scripts\powerShell\computerActivity.ps1");
                return ans;
            }

       //TBD - איסוף שיבוץ חדרים
       public void GetClassesSchedule(List<Lab> LabList)
        {
            //open connection with classes pacment system databse
            foreach (Lab lab in LabList)
            {
                //get the classes that are plenned in this lab for today
            }
        }

        private string GetUserLogOn(string compNames)
        {
            String script = "(Get-WmiObject -Class win32_computersystem -ComputerName "+ compNames + ").UserName";
            String ans = runScript(script);
            return ans;
        }
        private string IsComputerLogedOn(String compName)
        {
            String script = "(Test-Connection -BufferSize 32 -Count 1 -ComputerName " + compName + " -Quiet)";
            return runScript(script);
        }



        private string runScript(string scriptText)
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

        //// helper method that takes your script path, loads up the script 
        //// into a variable, and passes the variable to the RunScript method 
        //// that will then execute the contents 
        //private string loadScript(string filename)
        //{
        //    try
        //    {
        //        // Create an instance of StreamReader to read from our file. 
        //        // The using statement also closes the StreamReader. 
        //        using (StreamReader sr = new StreamReader(filename))
        //        {

        //            // use a string builder to get all our lines from the file 
        //            StringBuilder fileContents = new StringBuilder();

        //            // string to hold the current line 
        //            string curLine;

        //            // loop through our file and read each line into our 
        //            // stringbuilder as we go along 
        //            while ((curLine = sr.ReadLine()) != null)
        //            {
        //                // read each line and MAKE SURE YOU ADD BACK THE 
        //                // LINEFEED THAT IT THE ReadLine() METHOD STRIPS OFF 
        //                fileContents.Append(curLine + "\n");
        //            }

        //            // call RunScript and pass in our file contents 
        //            // converted to a string 
        //            return fileContents.ToString();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        // Let the user know what went wrong. 
        //        string errorText = "The file could not be read:";
        //        errorText += e.Message + "\n";
        //        return errorText;
        //    }

        //}




    }
}