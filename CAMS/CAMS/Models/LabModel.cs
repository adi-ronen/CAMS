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

        ComputersController _cController = new ComputersController();
        public Lab Lab;

        public LabModel(Lab lab)
        {
            this.Lab = lab;
        }


        public ActivityMode GetComputerState(Computer comp)
        {
            Activity currentAct = _cController.LastActivityDetails(comp.ComputerId);
            if (currentAct == null)
                return ActivityMode.On;
            else if (currentAct.Mode == ActivityMode.Off.ToString())
                return ActivityMode.Off;
            else
                return ActivityMode.User; 

        }
    }
}