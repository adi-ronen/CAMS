using CAMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
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

        internal int GetConnectedUser()
        {
            if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int userId))
            {
                return userId;
            }
            return -1;
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
                Lab lab = db.Labs.Find(id);
                db.Entry(lab).Collection(e => e.ComputerLabs).Load();
                db.Entry(lab).Collection(e => e.Computers).Load();
                db.Entry(lab).Reference(e => e.Department).Load();
                return lab;
            }
        }
     
        
        public ActivityType CurrentActivityDetails(int id)
        {
            Computer comp;
            using (var db = new CAMS_DatabaseEntities())
            {
                comp = db.Computers.Find(id);


                if (comp == null)
                {
                    return ActivityType.On;
                }
                List<Activity> activities;
                //  lock (db)
                {
                    activities = comp.Activities.Select(e => e).Where(e => e.Logout.Equals(null)).ToList();
                }
                if (activities.Count() == 0)
                    return ActivityType.On;
                return activities.Last().Mode;

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

        internal bool IsFullAccess(int depId)
        {
            Dictionary<int, AccessType> acDic = (Dictionary<int, AccessType>)Session["Accesses"];
            if (acDic != null && acDic.ContainsKey(depId) && acDic[depId] == AccessType.Full)
                return true;
            return false;

        }

        internal bool IsFullAccessUser()
        {
            Dictionary<int, AccessType> acDic = (Dictionary<int, AccessType>)Session["Accesses"];
            if (acDic!=null && acDic.ContainsValue(AccessType.Full))
                return true;
            return false;
        }
        internal bool IsViewAccessUser()
        {
            Dictionary<int, AccessType> acDic = (Dictionary<int, AccessType>)Session["Accesses"];
            if (acDic != null && acDic.Keys.Count>0)
                return true;
            return false;
        }

        internal bool IsLimitedAccess(int depId)
        {
            Dictionary<int, AccessType> acDic = (Dictionary<int, AccessType>)Session["Accesses"];
            if (acDic!=null && acDic.ContainsKey(depId) && (acDic[depId] == AccessType.Limited || acDic[depId] == AccessType.Full))
                return true;
            return false;

        }
        internal bool IsViewAccess(int depId)
        {
            Dictionary<int, AccessType> acDic = (Dictionary<int, AccessType>)Session["Accesses"];
            if (acDic != null && acDic.ContainsKey(depId))
                return true;
            return false;

        }
        internal bool IsSuperUser()
        {
            return Session["SupperUser"]!=null && (bool)Session["SupperUser"];
        }
        protected ActionResult RedirectAcordingToLogin()
        {
            if (GetConnectedUser() == -1)
                return RedirectToAction("Login", "Account");
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }


        protected Computer CreateComputer(string computerName, string domain)
        {
            
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer comp = new Computer();
                comp.ComputerName = computerName;
                //comp.ComputerId = db.Computers.Max(e => e.ComputerId) + 1;
                comp.LocationInLab = "0%,0%";
                //comp.MAC= findComputerMac(computerName, domain);
              //  lock (db)
                {
                    db.Computers.Add(comp);
                    db.SaveChanges();

                }
                return comp;
            }
            
        }

        protected void DeleteLab(int id)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(id);
                //delete only if user have full access to lab
                if (IsFullAccess(lab.DepartmentId))
                {
                    foreach (var compId in lab.Computers.Select(e => e.ComputerId).ToList())
                    {
                        RemoveComputerFromLab(compId, lab.LabId);
                    }
                    db.Labs.Remove(lab);
                    db.SaveChanges();
                }
            }
            
        }

        public void RemoveComputerFromLab(int compId, int labId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                ExecudeCommand("UPDATE ComputerLabs SET [Exit] = '" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "' WHERE ComputerId = '" + compId + "' AND [Exit] is null; ");

                Computer comp = db.Computers.Find(compId);
                comp = db.Computers.Find(compId);
                comp.CurrentLab = null;
                db.SaveChanges();
            }
        }
        

        protected void ExecudeCommand(string query)
        {
            string connectionString = "data source=132.72.223.244;initial catalog=CAMS_Database;user id=CAMS_Admin;password=9O8qAft1;MultipleActiveResultSets=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;


                reader = cmd.ExecuteReader();
            }
        }
    }
}