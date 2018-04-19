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

        internal List<Lab> GetLabsOfDepartment(int departmentId)
        {
            return db.Labs.Where(lab => lab.DepartmentId == departmentId).ToList();
        }

        internal List<Lab> GetLabs()
        {
            return db.Labs.ToList();
        }

        internal List<Department> GetDepartments()
        {
            return db.Departments.ToList();
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
        public Lab GetLab(int id)
        {
            return db.Labs.Find(id);
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

        public Computer GetComputer(int id)
        {
            return db.Computers.Find(id);
        }


        //-----------------------------------------------------------------
        //for tests only
        public void testAddLab(int id)
        {
            Lab lab = new Lab();
            lab.LabId = id;

            db.Labs.Add(lab);
            db.SaveChanges();
        }
        public void testAddComp(int id)
        {
            Computer comp = new Computer();
            comp.ComputerId = id;

            db.Computers.Add(comp);
            db.SaveChanges();
        }
        public void testAddCompLab(int compId,int labId,DateTime enter,DateTime? exit)
        {
            ComputerLab cL = new ComputerLab();
            cL.ComputerId = compId;
            cL.LabId = labId;
            cL.Entrance = enter;
            cL.Exit = exit;

            db.ComputerLabs.Add(cL);
            db.SaveChanges();
        }
        public void testAddActivity(int compId,DateTime login,DateTime logout,ActivityMode Mode)
        {
            Activity act = new Activity();
            act.ComputerId = compId;
            act.Login = login;
            act.Logout = logout;
            act.Mode = Mode.ToString();

            db.Activities.Add(act);
            db.SaveChanges();
        }
        public void testDeleteLab(int id)
        {
            Lab lab = db.Labs.Find(id);
            while( lab.ComputerLabs.Count>0)
            {
                db.ComputerLabs.Remove(lab.ComputerLabs.First());
            }
            db.Labs.Remove(lab);
            db.SaveChanges();
        }
        public void testDeleteComp(int id)
        {
            Computer comp = db.Computers.Find(id);
            while (comp.ComputerLabs.Count > 0)
            {
                db.ComputerLabs.Remove(comp.ComputerLabs.First());
            }
            while (comp.Activities.Count > 0)
            {
                db.Activities.Remove(comp.Activities.First());
            }
            db.Computers.Remove(comp);
            db.SaveChanges();
        }




    }
}