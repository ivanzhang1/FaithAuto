using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Xml;
using System.IO;

namespace QAUtil.Controllers
{
    public class TestsController : Controller
    {
        //
        // GET: /Tests/

        public ActionResult Index()
        {
            string rootDir = @"C:\builds\";
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(@"\\hudson\e$\hudson\jobs\Portal-FunctionalTest\builds\2012-04-05_09-48-13\junitResult.xml");

            var dirName = new DirectoryInfo("C:\\builds").GetDirectories().OrderByDescending(d => d.LastWriteTime).First();
            xmlDoc.Load(string.Format("{0}{1}\\junitResult.xml", rootDir, dirName));

            XmlNodeList suites = xmlDoc.SelectNodes("//suite");
            List<Models.TestSuite> testSuites = new List<Models.TestSuite>();
            foreach (XmlNode suite in suites) {
                testSuites.Add(new Models.TestSuite { File = suite.FirstChild.InnerText, Name = suite.FirstChild.NextSibling.InnerText, Duration = suite.FirstChild.NextSibling.NextSibling.InnerText, TestCases = suite.FirstChild.NextSibling.NextSibling.NextSibling.ChildNodes.Count });
                //suiteNames.Add(suite.FirstChild.NextSibling.InnerText);
            }
    
            return View(testSuites);
        }

    }
}
