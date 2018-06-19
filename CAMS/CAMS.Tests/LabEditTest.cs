using System;
using CAMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CAMS.Tests
{
    [TestClass]
    public class LabEditTest
    {
        [TestMethod]
        public void removeComputerTest()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("D:\\CLASS004PC01.txt"))
                {
                    string line;
                    while (sr.Peek() >= 0)
                    {
                       line=sr.ReadLine();
                       string[] param = line.Split(' ');
                       string userName = param[1];
                       string time = param[2] + " " + param[3];
                       DateTime login = DateTime.Parse(time);
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
