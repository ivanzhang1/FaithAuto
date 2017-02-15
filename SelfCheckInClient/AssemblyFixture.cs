using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using log4net;
using log4net.Config;

namespace FTTests {


    [AssemblyFixture]
    public class AssemblyFixture : Setup {

        Boolean local = false;
        private Process _processSelenium = null;
        private Process _processSeleniumNode = null;
        private Process _processSeleniumHub = null;

        //FGJ log4net
        //private static readonly ILog log = LogManager.GetLogger(typeof(AssemblyFixture));
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        [FixtureSetUp]
        public void SetUp() {

            //TODO
            //FGJ            
            //Getting log4net:ERROR Could not create Appender [AutomationPatternFileAppender] of type [log4net.Appender.RollingPatternFileAppender]. Reported error follows
            //Investigating more later.
            //String log4NetFileName = @"..\..\..\Common\bin\Debug\Common.dll.config";
            //XmlConfigurator.Configure(new System.IO.FileInfo(log4NetFileName));

            //Adding log4net config setup in here
            log4net.Config.BasicConfigurator.Configure();
            //log4net.Config.XmlConfigurator.Configure();

            log.Debug("*** We are logging!!! ***");
            log.Debug("Enter Setup");

            /* HOW TO's
             Starting up the hub
             java -jar selenium-server-standalone-2.25.0.jar -role hub -browserTimeout 300 -timeout 300

            Starting up the node
            java -jar selenium-server-standalone-2.25.0.jar -role node -hub http://localhost:4444/grid/register 
             */

            // Start a local instance of the selenium server if tests are to be run locally
            if(base.ServerHost == "localhost")
            {

                local = true;
                //string argumentsSeleniumHub = @"-jar ..\..\..\selenium-server-standalone-2.31.0.jar -role hub";
                string argumentsSeleniumHub = @"..\..\..\startHub.bat";

                //string argumentsSeleniumNode = @"-jar ..\..\..\selenium-server-standalone-2.25.0.jar -role node -hub http://localhost:4444/grid/register -browser ""browserName=internet  explorer,version=9,maxInstances=1,platform=WINDOWS"" -browser ""browserName=firefox,maxInstances=5,platform=WINDOWS"" ";
                //string argumentsSeleniumNode = @"-jar ..\..\..\selenium-server-standalone-2.31.0.jar -role node -hub http://localhost:4444/grid/register -hubPort 4444 -browser ""browserName=firefox,maxInstances=5,platform=WINDOWS"" ";
                //string argumentsSeleniumNode = @"-jar ..\..\..\selenium-server-standalone-2.25.0.jar -role node -hub http://localhost:4444/grid/register -hubPort 4444 -url http://localhost:4444 -remoteHost http://localhost:4444";

                //string argumentsSeleniumNode = @"..\..\..\startNode.bat";
                string argumentsSeleniumNode = @"..\..\..\startNodeJSON.bat";

                //log.Debug("Local Host - Launch Selenium Server Standalone Hub: " + argumentsSeleniumHub);
                //log.DebugFormat(string.Format("{0}java", base.Configuration.AppSettings.Settings["FTTests.JavaPath"].Value), argumentsSeleniumHub);

                //log.Debug("Starting Hub ... " + argumentsSeleniumHub);
                //log.DebugFormat(string.Format("{0}java", base.Configuration.AppSettings.Settings["FTTests.JavaPath"].Value), argumentsSeleniumHub);
                //_processSeleniumHub = Process.Start(string.Format("{0}java", base.Configuration.AppSettings.Settings["FTTests.JavaPath"].Value), argumentsSeleniumHub);

                log.Info("Start Hub");
                _processSeleniumHub = ExecuteCommand(argumentsSeleniumHub);
                Thread.Sleep(5000);

                //string browser = @"set BROWSER=""browserName=firefox,maxInstances=15,platform=WINDOWS""";
                //java -jar C:\Selenium\selenium-server-standalone-2.25.0.jar -role node -hub http://localhost:4444/grid/register -browser %BROWSER% -maxSession 15

                //log.Debug("Starting Node ... " + argumentsSeleniumNode);
                //log.DebugFormat(string.Format("{0}java", base.Configuration.AppSettings.Settings["FTTests.JavaPath"].Value), argumentsSeleniumNode);
                //string arguments = String.Format("-jar \"{0}\" -firefoxProfileTemplate \"{1}\"", SeleniumAppSettings.ServerJar, SeleniumAppSettings.FirefoxProfile);
                //_processSelenium = Process.Start(string.Format("{0} {1}java", browser, base.Configuration.AppSettings.Settings["FTTests.JavaPath"].Value), argumentsSeleniumNode);

                log.Info("Start Node");
                _processSeleniumNode = ExecuteCommand(argumentsSeleniumNode);
                log.Info("Wait for Node & Hub to sync");
                //TODO Should just send an HTTP request for now let's just wait a few seconds
                Thread.Sleep(5000);

            }
            //else if ((base.ServerHost == "localhost"))
            //{
            //    local = true;
            //    string argumentsSelenium = @"-jar ..\..\..\selenium-server-standalone-2.31.0.jar -port 5444";
            //    log.Debug("Starting  ... " + argumentsSelenium);
            //    _processSelenium = Process.Start(string.Format("{0}java", base.Configuration.AppSettings.Settings["FTTests.JavaPath"].Value), argumentsSelenium);

            //}
            else
            {
                log.Info(string.Format("Selenium Grid is being used or tester opted to have node/hub manually launched [0]", base.ServerHost));
                
            }


            log.Debug("Exit Setup");

        }

        [FixtureTearDown]
        public void TearDown() {

            log.Debug("Enter Local TearDown");
            if (local)
            {
                //Tear Down the locally run Selenium
                CloseProcess(_processSelenium, "Selenium");

                //Tear Down the locally run Selenium webdriver hub
                CloseProcess(_processSeleniumHub, "Selenium Webdriver Hub");

                //Tear Down the locally run Selenium webdriver node
                CloseProcess(_processSeleniumNode, "Selenium Webdriver Node");
            }
            log.Debug("Exit Local TearDown");

        }

        private void CloseProcess(Process processSelenium, string processName)
        {

            log.Info("Close Process: " + processName);
            
            //Tear Down the locally run selelinum webdriver node
            if (processSelenium != null)
            {

                try
                {

                    if (!processSelenium.HasExited)
                    {
                        processSelenium.CloseMainWindow();

                    }

                    if (!processSelenium.HasExited)
                    {
                        processSelenium.Kill();
                    }

                    processSelenium.Close();

                    //_processSeleniumHub.BeginOutputReadLine();
                    //_processSeleniumHub.BeginErrorReadLine();

                    //log.Debug("Hub Output: " + _processSeleniumHub.StandardOutput.ReadToEnd());
                    //log.Debug("Hub Error: " + _processSeleniumHub.StandardError.ReadToEnd());
                }
                catch (Exception e)
                {
                    log.Fatal(processName +  " Tear Down Error...Investigate\n" + e.Message);
                }

            }

        }

        /// <summary>
        /// Executes a command using cmd.exe (i.e. batch)
        /// </summary>
        /// <param name="command">Command to be executed</param>
        /// <returns>Instance of Process returned after command is executed</returns>
        static Process ExecuteCommand(string command)
        {

            log.Debug("Execute: " + command);

            //int exitCode;

            ProcessStartInfo processInfo;
            Process process;
            
            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = false;
            
            // *** Redirect the output ***
            //processInfo.RedirectStandardError = true;
            //processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo); 
           
            //Don't want this since this is indef
            //process.WaitForExit();

            // *** Read the streams ***
            //string output = process.StandardOutput.ReadToEnd();
            //string error = process.StandardError.ReadToEnd();
            
            //Need waitForExit
            //exitCode = process.ExitCode;

            return process;

        }
    }
}