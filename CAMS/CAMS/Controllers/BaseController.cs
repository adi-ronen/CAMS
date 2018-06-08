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
        protected Object syncLock = new Object();

        protected CAMS_DatabaseEntities db = new CAMS_DatabaseEntities();

        internal List<string> GetBuildings()
        {
            lock (db)
            {
                return db.Labs.Select(x => x.Building).ToList();
            }
        }
        internal List<Lab> GetLabsOfDepartment(int departmentId)
        {
            lock (db)
            {
                return db.Labs.Where(lab => lab.DepartmentId == departmentId).ToList();
            }
        }

        internal List<Lab> GetLabs()
        {
            lock (db)
            {
                return db.Labs.ToList();
            }
        }

        internal List<Department> GetDepartments()
        {
            lock (db)
            {
                return db.Departments.ToList();
            }
        }

      
        public Lab GetLab(int id)
        {
            lock (db)
            {
                return db.Labs.Find(id);
            }
        }
     

        public Computer GetComputer(int id)
        {
            lock (db)
            {
                return db.Computers.Find(id);
            }
        }
        public Activity LastActivityDetails(int id)
        {
            Computer comp;
            lock (db)
            {
                comp = db.Computers.Find(id);
            }

            if (comp == null)
            {
                return null;
            }
            // return computer.Activities.Select(e => e).Where(e => e.Mode != ActivityMode.Class.ToString() && e.Logout.Equals(null)).Last();
            List<Activity> activities = comp.Activities.Select(e => e).Where(e => e.Logout.Equals(null)).ToList();
            if (activities.Count() == 0)
                return null;
            return activities.Last();



        }

        public List<User> GetEmailSubscribers(NotificationFrequency frequency)
        {
            lock (db)
            {
                return db.Users.Where(e => e.NotificationFrequency == frequency).ToList();
            }
        }

        protected Computer CreateComputer(string computerName, string domain)
        {
            Computer comp = new Computer();


            comp.ComputerName = computerName;
            lock (db)
            {
                comp.ComputerId = db.Computers.Max(e => e.ComputerId) + 1;
            }
            comp.LocationInLab = "0%,0%";
            //comp.MAC= findComputerMac(computerName, domain);
            lock (db)
            {
                db.Computers.Add(comp);
                db.SaveChanges();

            }
            return comp;
        }

        protected void DeleteLab(int id)
        {
            Lab lab;
            lock (db)
            {
                lab = db.Labs.Find(id);
            }
            while (lab.Computers.Count > 0)
            {
                RemoveComputerFromLab(lab.Computers.First().ComputerId, lab.LabId);
            }
            lock (db)
            {
                db.Labs.Remove(lab);
                db.SaveChanges();
            }
        }

        public void RemoveComputerFromLab(int compId, int labId)
        {
            Computer comp;
            lock (db)
            {
                comp = db.Computers.Find(compId);
            }
            //computer not in lab- nothing to update
            if (comp.CurrentLab != labId)
                return;
            var cList = comp.ComputerLabs.Select(e => e).Where(e => e.Exit.Equals(null)).Where(e => e.LabId.Equals(labId)).ToList();
            if (cList.Count > 0)
            {
                lock (db)
                {
                    ComputerLab cL = db.ComputerLabs.Find(labId, comp.ComputerId, cList.First().Entrance);
                    cL.Exit = DateTime.Now;
                    db.Entry(cL).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //db.ComputerLabs.Attach(cL);

            }

            comp.CurrentLab = null;
            // db.Computers.Attach(comp);
            lock (db)
            {
                db.Entry(comp).State = EntityState.Modified;
                db.SaveChanges();
            }


        }

        //-----------------------------------------------------------------
        //for tests only
        public void testAddLab(int id)
        {
            Lab lab = new Lab();
            lab.LabId = id;
            lock (db)
            {
                db.Labs.Add(lab);
                db.SaveChanges();
            }
        }
        public void testAddComp(int id)
        {
            Computer comp = new Computer();
            comp.ComputerId = id;
            lock (db)
            {
                db.Computers.Add(comp);
                db.SaveChanges();
            }
        }
        public void testAddCompLab(int compId,int labId,DateTime enter,DateTime? exit)
        {
            ComputerLab cL = new ComputerLab();
            cL.ComputerId = compId;
            cL.LabId = labId;
            cL.Entrance = enter;
            cL.Exit = exit;
            lock (db)
            {
                db.ComputerLabs.Add(cL);
                db.SaveChanges();
            }
        }
        public void testAddActivity(int compId,DateTime login,DateTime logout,ActivityType Mode)
        {
            Activity act = new Activity();
            act.ComputerId = compId;
            act.Login = login;
            act.Logout = logout;
            act.Mode = Mode;
            lock (db)
            {
                db.Activities.Add(act);
                db.SaveChanges();
            }
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