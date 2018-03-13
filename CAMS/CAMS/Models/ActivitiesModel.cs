using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace CAMS.Models
{
    public class ActivitiesModel
    {
        CAMS_DatabaseEntities _db = new CAMS_DatabaseEntities();
        /// <summary>
        /// get current cumputer activity for a computer list using active directory. 
        /// </summary>
        /// <param name="ComputerList"></param>
        public static String GetComputersActivity(List<Computer> ComputerList)
        {
            //foreach (Computer comp in ComputerList)
            //{
            //powershell ask for "enable", "username" --> find how to execute ps command and get output
            //get his last activity from activity table that is not "class mode" 
            //compare and update as nedded
            /*
             * if 
             */

            //string path = @"\..\Scripts\powerShell\computerActivity.ps1";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Scripts\powerShell\computerActivity.ps1");

            return runScript(loadScript(path));
            //return script();



            //}
            //return "none";
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

        //private static String script()
        //{
        //    String ans = string.Empty;

        //    // Initialize PowerShell engine
        //    var shell = PowerShell.Create();

        //    // Add the script to the PowerShell object
        //    //@"C:\Users\Olga\Source\Repos\CAMS\CAMS\CAMS\Scripts\powerShell\computerActivity.ps1"
        //    shell.Commands.AddScript("$Env:USERNAME");

        //    // Execute the script
        //    var results = shell.Invoke();

        //    // display results, with BaseObject converted to string
        //    // Note : use |out-string for console-like output
        //    if (results.Count > 0)
        //    {
        //        // We use a string builder ton create our result text
        //        var builder = new StringBuilder();

        //        foreach (var psObject in results)
        //        {
        //            // Convert the Base Object to a string and append it to the string builder.
        //            // Add \r\n for line breaks
        //            builder.Append(psObject.BaseObject.ToString() + "\r\n");
        //        }

        //        // Encode the string in HTML (prevent security issue with 'dangerous' caracters like < >
        //        ans = builder.ToString();
        //    }
        //    return ans;
        //}




        private static string runScript(string scriptText)
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

        // helper method that takes your script path, loads up the script 
        // into a variable, and passes the variable to the RunScript method 
        // that will then execute the contents 
        private static string loadScript(string filename)
        {
            try
            {
                // Create an instance of StreamReader to read from our file. 
                // The using statement also closes the StreamReader. 
                using (StreamReader sr = new StreamReader(filename))
                {

                    // use a string builder to get all our lines from the file 
                    StringBuilder fileContents = new StringBuilder();

                    // string to hold the current line 
                    string curLine;

                    // loop through our file and read each line into our 
                    // stringbuilder as we go along 
                    while ((curLine = sr.ReadLine()) != null)
                    {
                        // read each line and MAKE SURE YOU ADD BACK THE 
                        // LINEFEED THAT IT THE ReadLine() METHOD STRIPS OFF 
                        fileContents.Append(curLine + "\n");
                    }

                    // call RunScript and pass in our file contents 
                    // converted to a string 
                    return fileContents.ToString();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong. 
                string errorText = "The file could not be read:";
                errorText += e.Message + "\n";
                return errorText;
            }

        }




    }
}