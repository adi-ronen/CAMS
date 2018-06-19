using System;
using CAMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CAMS.Tests
{
    [TestClass]
    public class LabEditTest
    {

        CAMS.Controllers.BaseController bC = new Controllers.BaseController();

        [TestMethod]
        public void removeComputerTest()
        {
            

        }

        public void func()
        {
            for (int i = 0; i <= 9; i++)
            {
                LoadComputersActivities("CLASS007PC0" + i);

            }
            for (int i = 10; i <= 21; i++)
            {
                LoadComputersActivities("CLASS007PC" + i);

            }
        }


        private bool IsWeekend(DayOfWeek dayOfWeek)
        {
            return (dayOfWeek.Equals(DayOfWeek.Friday) || dayOfWeek.Equals(DayOfWeek.Saturday));

        }
        private void LoadComputersActivities(string compName)
        {

            try
            {   // Open the text file using a stream reader.
                CAMS.Controllers.BaseController bC = new Controllers.BaseController();

                ComputerLab cl = null;

                int labId = 17;
                int compId = bC.GetComputerId(compName);
                using (StreamReader sr = new StreamReader(@"D:\olladi\computers\\" + compName + ".txt"))
                {
                    string line;
                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine();
                        string[] param = line.Split(' ');
                        if (param[0].ToLower() != "login")
                            continue;
                        string userName = param[1];
                        string time = param[2] + " " + param[3];
                        DateTime login = DateTime.Parse(time);
                        line = sr.ReadLine();
                        param = line.Split(' ');
                        if (param[0].ToLower() != "logout")
                        {
                            line = sr.ReadLine();
                            param = line.Split(' ');
                            if (param[0].ToLower() != "logout")
                            {
                                continue;
                            }

                        }
                        time = param[2] + " " + param[3];
                        DateTime logout = DateTime.Parse(time);
                        if (logout.Date != login.Date)
                        {
                            continue;
                        }
                        if (cl == null)
                        {
                            cl = new ComputerLab
                            {
                                LabId = labId,
                                ComputerId = compId,
                                Entrance = login.Date
                            };
                            bC.AddComputerLab(cl);
                        }
                        Activity act = new Activity
                        {
                            Login = login,
                            Logout = logout,
                            UserName = userName,
                            ComputerId = compId,
                            Mode = CAMS.Models.ActivityType.User,
                            Weekend = IsWeekend(login.DayOfWeek)
                        };
                        bC.AddActivity(act);
                    }

                }


            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
