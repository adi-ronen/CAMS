using CAMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CAMS.Constant;

namespace CAMS.Controllers
{
    public class BaseController : Controller
    {
        protected CAMS_DatabaseEntities db = new CAMS_DatabaseEntities();

        internal List<string> GetBuildings()
        {
            return db.Labs.Select(x => x.Building).ToList();
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

      
        public Lab GetLab(int id)
        {
            return db.Labs.Find(id);
        }
     

        public Computer GetComputer(int id)
        {
            return db.Computers.Find(id);
        }
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

        public List<User> GetEmailSubscribers(NotificationFrequency frequency)
        {
            return db.Users.Where(e => e.NotificationFrequency == frequency).ToList(); 
        }

        protected Computer CreateComputer(string computerName, string domain)
        {
            Computer comp = new Computer();
            using (db)
            {
                
                comp.ComputerName = computerName;
                comp.ComputerId = db.Computers.Max(e => e.ComputerId) + 1;
                comp.LocationInLab = "0%,0%";
                //comp.MAC= findComputerMac(computerName, domain);
                db.Computers.Add(comp);
                db.SaveChanges();
            }

            return comp;
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
        public void testAddActivity(int compId,DateTime login,DateTime logout,ActivityType Mode)
        {
            Activity act = new Activity();
            act.ComputerId = compId;
            act.Login = login;
            act.Logout = logout;
            act.Mode = Mode;

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