using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAMS.Models
{
    public class ActivitiesModel
    {
        CAMS_DatabaseEntities _db = new CAMS_DatabaseEntities();
        //TBD - איסוף מצב מחשבים נוכחי
        public void GetADComputers(List<Computer> ComputerList)
        {
            foreach (Computer comp in ComputerList)
            {
                //powershell ask for "enable", "username" --> find how to execute ps command and get output
                //get his last activity from activity table that is not "class mode" 
                //compare and update as nedded
                /*
                 * if 
                 */
            }
        }

       //TBD - איסוף שיבוץ חדרים
       public void GetClassesPlacement(List<Lab> LabList)
        {
            //open connection with classes pacment system databse
            foreach (Lab lab in LabList)
            {
                //get the classes that are plenned in this lab for today
            }
        }
    }
}