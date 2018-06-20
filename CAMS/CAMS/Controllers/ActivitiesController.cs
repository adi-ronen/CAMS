using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAMS.Models;
using static CAMS.Constant;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CAMS.Controllers
{
    public class ActivitiesController : BaseController
    {
        private readonly object syncLock = new object();
        
        internal List<int> GetLabIds()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Labs.Select(e => e.LabId).ToList();
            }
        }

        internal List<Computer> GetLabComputers(int labId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(labId);
                return lab.Computers.ToList();
            }
        }
       

        internal string GetCompDomain(int computerId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Computer comp = db.Computers.Find(computerId);
                return comp.Lab.Department.Domain;
            }
        }
        

        internal string GetCurrentComputerUser(int computerId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<Activity> act = db.Activities.Where(e => e.ComputerId.Equals(computerId) && !e.Logout.HasValue).ToList();
                if (act.Count > 0)
                    return act.First().UserName;
                else
                    return "";

            }
        }
        
        internal void UpdateLabSchedule(int labId, string classes)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                Lab lab = db.Labs.Find(labId);
                lab.TodaysClasses = classes;
                db.Entry(lab).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        

        public void CreateNewActivity(int compId, ActivityType mode, string userName)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                // Activity act = new Activity();
                Computer comp = db.Computers.Find(compId);
                string uName = null;
                if (userName != null)
                    uName = userName;
                Activity act = new Activity
                {
                    UserName = uName,
                    Mode = mode,
                    Login = DateTime.Now,
                    Weekend = IsWeekend(DateTime.Now.DayOfWeek),
                    ComputerId = comp.ComputerId
                };
                db.Activities.Add(act);
                comp.Activities.Add(act);
                db.Entry(comp).State = EntityState.Modified; // is it the way to update? enother option:  db.Set<X>().AddOrUpdate(x);
                db.SaveChanges();
            }
        }

        public void CreateNewClassActivity(int labId, DateTime start, DateTime end)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                // Activity act = new Activity();
                Lab lab = db.Labs.Find(labId);
                
                foreach (var item in lab.Computers)
                {
                    Activity act = new Activity
                    {
                        Mode = ActivityType.Class,
                        Login = start,
                        Logout = end,
                        Weekend = false,
                        ComputerId = item.ComputerId
                    };
                    db.Activities.Add(act);
                 //   item.Activities.Add(act);
                 //   db.Entry(comp).State = EntityState.Modified; // is it the way to update? enother option:  db.Set<X>().AddOrUpdate(x);
                    db.SaveChanges();
                }
                
            }
        }

        public int FindLabID(string building,string room)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                try
                {
                    var labs = db.Labs.Where(e => e.Building.Contains(building) && e.RoomNumber.Remove('-').Contains(room)).ToList();
                    return labs.First().LabId;
                }
                catch
                {
                    return -1;
                }

            }
        }


        private bool IsWeekend(DayOfWeek dayOfWeek)
        {
            return (dayOfWeek.Equals(DayOfWeek.Friday) || dayOfWeek.Equals(DayOfWeek.Saturday));

        }

       
        public void CloseActivity(int compId)
        {
            
            ExecudeCommand("UPDATE Activities SET Logout = '" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "' WHERE ComputerId = '" + compId + "' AND Logout is null; ");

        }

        public void SplitActivity(int compId)
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                List<Activity> acts = db.Activities.Where(e => e.ComputerId.Equals(compId) && !e.Logout.HasValue).ToList();
                foreach (var act in acts)
                {
                    if(!act.Login.Date.Equals(DateTime.Now.Date))
                    {
                        DateTime end = act.Login.Date.AddDays(1).AddTicks(-1);
                        ExecudeCommand("UPDATE Activities SET Logout = '" + end.ToString("MM/dd/yyyy HH:mm:ss") + "' WHERE ComputerId = '" + compId + "' AND Logout is null; ");

                        Activity newAct = new Activity
                        {
                            Login = DateTime.Now.Date
                        };
                        newAct.Weekend = IsWeekend(newAct.Login.DayOfWeek);
                        newAct.ComputerId = act.ComputerId;
                        newAct.Mode = act.Mode;
                        if (act.UserName != null)
                            act.UserName = act.UserName;
                        db.Activities.Add(newAct);
                        db.SaveChanges();
                    }
                }
               
            }
        }

        internal List<Lab> GetAllLabs()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                return db.Labs.ToList();
            }
        }

        internal void ClearLabsSchedule()
        {
            using (var db = new CAMS_DatabaseEntities())
            {
                foreach (var labId in db.Labs.Select(e=>e.LabId).ToList())
                {
                    UpdateLabSchedule(labId, "");
                }
                
            }
        }
    }
}
