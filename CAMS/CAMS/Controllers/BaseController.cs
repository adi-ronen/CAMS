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


        internal List<string> GetBuildings()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Labs.Select(x => x.Building).ToList();
            }
        }
        internal List<Lab> GetLabsOfDepartment(int departmentId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Labs.Where(lab => lab.DepartmentId == departmentId).ToList();
            }
        }

        

        internal List<Department> GetDepartments()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Departments.ToList();
            }
        }

      
        public Lab GetLab(int id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Labs.Find(id);
            }
        }
     
        
        public Activity CurrentActivityDetails(int id)
        {
            Computer comp;
            using (var db = new CAMS_DatabaseEntities())
            {
                comp = db.Computers.Find(id);


                if (comp == null)
                {
                    return null;
                }
                List<Activity> activities;
                //  lock (db)
                {
                    activities = comp.Activities.Select(e => e).Where(e => e.Logout.Equals(null)).ToList();
                }
                if (activities.Count() == 0)
                    return null;
                return activities.Last();

                // return computer.Activities.Select(e => e).Where(e => e.Mode != ActivityMode.Class.ToString() && e.Logout.Equals(null)).Last();
            }



        }

        public List<User> GetEmailSubscribers(NotificationFrequency frequency)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Users.Where(e => e.NotificationFrequency == frequency).ToList();
            }
        }

        protected Computer CreateComputer(string computerName, string domain)
        {
            
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer comp = new Computer();
                comp.ComputerName = computerName;
                comp.ComputerId = db.Computers.Max(e => e.ComputerId) + 1;
                comp.LocationInLab = "0%,0%";
                //comp.MAC= findComputerMac(computerName, domain);
                lock (db)
                {
                    db.Computers.Add(comp);
                    db.SaveChanges();

                }
                return comp;
            }
            
        }

        protected void DeleteLab(int id)
        {
            Lab lab;
            using (var db = new CAMS_DatabaseEntities())
            {
                lab = db.Labs.Find(id);
                while (lab.Computers.Count > 0)
                {
                    RemoveComputerFromLab(lab.Computers.First().ComputerId, lab.LabId);
                }
                db.Labs.Remove(lab);
                db.SaveChanges();
            }
            
        }

        public void RemoveComputerFromLab(int compId, int labId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer comp = db.Computers.Find(compId);
                //computer not in lab- nothing to update
                if (comp.CurrentLab != labId)
                    return;
                var cList = comp.ComputerLabs.Select(e => e).Where(e => e.Exit.Equals(null)).Where(e => e.LabId.Equals(labId)).ToList();
                if (cList.Count > 0)
                {

                    ComputerLab cL = db.ComputerLabs.Find(labId, comp.ComputerId, cList.First().Entrance);
                    cL.Exit = DateTime.Now;
                    db.Entry(cL).State = EntityState.Modified;
                    db.SaveChanges();

                    //db.ComputerLabs.Attach(cL);
                    db.Entry(comp).State = EntityState.Modified;
                    db.SaveChanges();
                }

                comp.CurrentLab = null;
            }
            
        }

      


    }
}