using CAMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CAMS.Constant;

namespace CAMS.Controllers
{
    public class BaseController : Controller
    {
        protected CAMS_DatabaseEntities db = new CAMS_DatabaseEntities();

        public Activity LastActivityDetails(int id)
        {
            Computer computer = db.Computers.Find(id);
            return LastActivityDetails(computer);
        }

        public Activity LastActivityDetails(Computer comp)
        {
            if (comp == null)
            {
                return null;
            }
            try
            {
                // return computer.Activities.Select(e => e).Where(e => e.Mode != ActivityMode.Class.ToString() && e.Logout.Equals(null)).Last();
                return comp.Activities.Select(e => e).Where(e => e.Logout.Equals(null)).Last();

            }
            catch (InvalidOperationException)
            {
                //if no element was found that means the cumputer is currently in On state with no user loged in
                return null;
            }
        }
        public void CreateNewActivity(Computer comp, ActivityMode mode, string userName)
        {
            Activity act = new Activity();
            if (userName != null)
                act.UserName = userName;
            act.Mode = mode.ToString();
            act.Login = DateTime.Now;
            act.ComputerId = comp.ComputerId;
            act.Computer = comp;
            db.Activities.Add(act);
            comp.Activities.Add(act);
            db.Entry(comp).State = EntityState.Modified; // is it the way to update? enother option:  db.Set<X>().AddOrUpdate(x);
            db.SaveChanges();
        }

        public void CloseActivity(Activity act)
        {
            act.Logout = DateTime.Now;
            db.Entry(act).State = EntityState.Modified; //same as above
            db.SaveChanges();

        }

    }
}