using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Diagnostics;

namespace QAUtil.Controllers
{
    public class CommandLineController : Controller
    {
        //
        // GET: /CommandLine/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MultiMechanize() {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = @"C:\Dev\tests\LoadTests";
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = @"C:\Python26\python.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = @"multi-mechanize.py InFellowship";
            startInfo.Domain = "active";
            startInfo.UserName = "msneeden";

            string initString = "MacBookPro17";
      // Instantiate the secure string.
      System.Security.SecureString testString = new System.Security.SecureString();
      // Use the AppendChar method to add each char value to the secure string.
      foreach (char ch in initString)
         testString.AppendChar(ch);

      startInfo.Password = testString;

            ////ProcessStartInfo startInfo = new ProcessStartInfo();
            ////startInfo.WorkingDirectory = @"C:\Windows\System32";
            ////startInfo.CreateNoWindow = false;
            ////startInfo.UseShellExecute = true;
            ////startInfo.FileName = @"notepad.exe";
            ////startInfo.WindowStyle = ProcessWindowStyle.Normal;
            ////startInfo.Arguments = @"C:\Dev\tests\LoadTests\multi-mechanize.py InFellowship";

            Process.Start(startInfo);

            //try {
            //    // Start the process with the info we specified.
            //    // Call WaitForExit and then the using statement will close.
            //    using (Process exeProcess = Process.Start(startInfo)) {
            //        exeProcess.WaitForExit();
            //    }
            //}
            //catch {
            //    // Log error.
            //}

            //Process.Start(@"C:\Windows\System32\notepad.exe");

            //Process python = Process.Start(@"C:\Python26\python.exe", @"C:\Dev\tests\LoadTests\multi-mechanize.py C:\Dev\tests\LoadTests\InFellowship");
            //python.Star


            return View("Index");
        }
    }
}
