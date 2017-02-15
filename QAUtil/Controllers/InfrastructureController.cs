using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.SqlClient;
using System.Text;
using System.Security.Principal;

using System.Management;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Web.Administration;
using Microsoft.Web.Administration;
using System.DirectoryServices;

namespace QAUtil.Controllers {
    [Authorize]
    public class InfrastructureController : Controller {
        //
        // GET: /Infrastructure/

        public ActionResult Index() {
            return View();
        }

        public ActionResult ApplicationLog() {
            return View();
        }

        public ActionResult InsertApplicationLogRecordForRDC(FormCollection form) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmPortal.dbo.APPLICATION_LOG (APPLICATION_ID, APPLICATION_LOG_STATUS_ID, DESCRIPTION, CREATED_TIME, CREATED_BY, CREATED_DATE) ");
            query.Append("VALUES(30, 1, 'RDC Initial Entry', '2011-01-01', 'RDCChecker', '2011-01-01')");
            Common.ExecuteDBQuery(form["dataSource"], query.ToString());

            return View("Index");
        }

        public ActionResult DeleteApplicationLogsForRDC(FormCollection form) {
            Common.ExecuteDBQuery(form["dataSource"], "DELETE FROM ChmPortal.dbo.APPLICATION_LOG WHERE APPLICATION_ID = 30");

            return View("Index");
        }

        #region Modules
        public ActionResult Modules(string dataSource, string churchId) {
            // If the input is an integer, assume that the user is providing a church id
            int church = int.MinValue;
            bool isInt = int.TryParse(churchId, out church);

            // If it is not, lookup the church id by using the church code
            if (!isInt) {
                church = Convert.ToInt32(Common.ExecuteDBQuery(dataSource, string.Format("SELECT TOP 1 CHURCH_ID FROM ChmChurch.dbo.CHURCH WHERE CHURCH_CODE = '{0}'", churchId)));
                ViewData["churchId"] = church;
            }
            else {
                ViewData["churchId"] = churchId;
            }

            ViewData["churchId"] = church;
            ViewData["dataSource"] = dataSource;
            var dataContext = new ModulesDataContext(Common.FetchDBConnectionString(dataSource));
            var modules = from m in dataContext.MODULEs
                          orderby m.MODULE_ID
                          join cm in dataContext.CHURCH_MODULEs on new { m.MODULE_ID, f1 = church } equals new { cm.MODULE_ID, f1 = cm.CHURCH_ID } into mod
                          from x in mod.DefaultIfEmpty().OrderBy(cm => cm.MODULE_ID)
                          select new QAUtil.Models.Modules {
                              ENABLED = x.MODULE_ID != null ? true : false,
                              MODULE_ID = m.MODULE_ID,
                              MODULE_NAME = m.MODULE_NAME,
                              CREATED_DATE = x.CREATED_DATE,
                              CREATED_BY_USER_ID = x.CREATED_BY_USER_ID,
                              LAST_UPDATED_DATE = x.LAST_UPDATED_DATE,
                              LAST_UPDATED_BY_USER_ID = x.LAST_UPDATED_BY_USER_ID,
                              CREATED_BY_LOGIN = x.CREATED_BY_LOGIN,
                              LAST_UPDATED_BY_LOGIN = x.LAST_UPDATED_BY_LOGIN
                          };

            //select ISNULL(CONVERT(BIT, c_m.church_id), 0) AS ENABLED, *
            //from module m with (nolock)
            //left join church_module c_m with (nolock)
            //on m.module_id = c_m.module_id and c_m.church_id = 15

            return View(modules);
        }

        public ActionResult UpdateModules(List<QAUtil.Models.Modules> grid, FormCollection form) {
            int churchId = Convert.ToInt32(form["church"]);
            ViewData["churchId"] = churchId;
            ViewData["dataSource"] = form["dataSource"];
            var dataContext = new ModulesDataContext(Common.FetchDBConnectionString(form["dataSource"]));
            foreach (var item in grid) {
                bool enabled = item.ENABLED;
                int module_id = item.MODULE_ID;

                // Enable or disable the module
                if (enabled) {
                    if (!dataContext.CHURCH_MODULEs.Any(c => c.MODULE_ID == item.MODULE_ID && c.CHURCH_ID == churchId)) {
                        CHURCH_MODULE cm = new CHURCH_MODULE() {
                            CHURCH_ID = churchId,
                            MODULE_ID = item.MODULE_ID,
                            CREATED_DATE = DateTime.Now,
                            LAST_UPDATED_DATE = DateTime.Now,
                            CREATED_BY_LOGIN = HttpContext.User.Identity.Name
                        };
                        dataContext.CHURCH_MODULEs.InsertOnSubmit(cm);
                        dataContext.SubmitChanges();
                    }
                }
                else {
                    if (dataContext.CHURCH_MODULEs.Any(c => c.MODULE_ID == item.MODULE_ID && c.CHURCH_ID == churchId)) {
                        dataContext.CHURCH_MODULEs.DeleteOnSubmit(dataContext.CHURCH_MODULEs.Where(cm => cm.MODULE_ID == item.MODULE_ID && cm.CHURCH_ID == churchId).Select(cm => cm).Single());
                        dataContext.SubmitChanges();
                    }
                }
            }
            return RedirectToAction("Modules", new { dataSource = ViewData["dataSource"], churchId = ViewData["churchId"] });
        }
        #endregion Modules

        #region Services
        public ActionResult Services(string svcEnvironment) {
            ViewData["svcEnvironment"] = svcEnvironment;

            SelectQuery query = new SelectQuery("SELECT NAME, SYSTEMNAME, STATUS, STATE, STARTNAME, DESCRIPTION FROM Win32_Service WHERE STARTNAME LIKE '%svc_msmq%'");
            List<Models.Services> data = new List<Models.Services>();

            foreach (KeyValuePair<string, System.Management.ManagementScope> scope in this.CreateManagementScope(svcEnvironment)) {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope.Value, query)) {
                    foreach (ManagementObject service in searcher.Get()) {
                        data.Add(new Models.Services { SERVER = scope.Key, NAME = service["Name"].ToString(), STATUS = service["Status"].ToString(), STATE = service["State"].ToString(), STARTNAME = service["startname"].ToString(), DESCRIPTION = service["Description"] != null ? service["Description"].ToString() : "Not Specified" });
                    }
                }
            }

            // Return the view with the sorted list of services
            return View(data.OrderBy(service => service.NAME));
        }

        public ActionResult ToggleService(string svcEnvironment, string server, string service, string command) {
            SelectQuery query = new SelectQuery(string.Format("SELECT NAME FROM Win32_Service WHERE NAME = '{0}'", service));

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(this.CreateManagementScope(svcEnvironment)[server], query)) {
                foreach (ManagementObject svc in searcher.Get()) {
                    string cmd = command == "Start" ? "StartService" : "StopService";
                    ManagementBaseObject outParams = svc.InvokeMethod(cmd, null, null);
                }
            }

            return RedirectToAction("Services", new { svcEnvironment });
        }
        #endregion Services

        #region Application Pools
        public ActionResult ApplicationPools(string appPoolEnvironment) {
            ViewData["appPoolEnvironment"] = appPoolEnvironment;

            List<Models.ApplicationPool> appPools = new List<Models.ApplicationPool>();

            SelectQuery query = new SelectQuery("SELECT * FROM IIsApplicationPool");

            foreach (KeyValuePair<string, System.Management.ManagementScope> scope in this.CreateManagementScopeAppPool(appPoolEnvironment)) {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope.Value, query)) {
                    foreach (ManagementObject applicationPool in searcher.Get()) {
                        appPools.Add(new Models.ApplicationPool { SERVER = scope.Key, NAME = applicationPool["Name"].ToString() });
                    }
                }
            }

            return View(appPools);
        }

        public ActionResult RecycleApplicationPool(string appPoolEnvironment, string server, string applicationPool) {
            SelectQuery query = new SelectQuery(string.Format("SELECT NAME FROM IIsApplicationPool WHERE NAME = '{0}'", applicationPool));

            
            ManagementObject appPool = new ManagementObject(this.CreateManagementScopeAppPool(appPoolEnvironment)[server], new ManagementPath(string.Format("IIsApplicationPool.Name='{0}'", applicationPool)), null);
            ManagementBaseObject outParams = appPool.InvokeMethod("Recycle", null, null);

            return RedirectToAction("ApplicationPools", new { appPoolEnvironment });
        }
        #endregion Application Pools

        public ActionResult Selenium() {
            // Make a request to the selenium grid console
            HttpWebRequest console = (HttpWebRequest)WebRequest.Create("http://qats01.dev.corp.local:4444/console");
            HttpWebResponse response = (HttpWebResponse)console.GetResponse();

            XmlDocument xDoc = new XmlDocument();

            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                if (response.ContentLength > 0) {
                    xDoc.Load(sr);
                }
            }
            response.Close();

            string environments = xDoc.SelectSingleNode("//div[@class='section']").InnerXml;
            string rcAvail = xDoc.SelectSingleNode("//div[@class='section' and position()=3]").InnerXml;
            string rcActive = xDoc.SelectSingleNode("//div[@class='section' and position()=4]").InnerXml;

            ViewData["Environments"] = environments;
            ViewData["rcAvail"] = rcAvail;
            ViewData["rcActive"] = rcActive;
            
            return View();

            //http://qats01.dev.corp.local:4444/registration-manager/unregister?host=10.121.40.74&port=5558&environment=*firefox
        }

        public ActionResult UnregisterRemoteControl(FormCollection form) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format("http://qats01.dev.corp.local:4444/registration-manager/unregister?host={0}&port={1}&environment=*firefox", form["host"], form["port"]));
            req.GetResponse();

            return RedirectToAction("Selenium");
        }

        #region Private Methods
        private string[] FetchServicesHostName(string environment) {
            string[] hostNames;

            switch (environment) {
                case "DEV":
                    hostNames = new string[] { "DAPP02.DEV.CORP.LOCAL", "DRPT01.DEV.CORP.LOCAL" };
                    break;
                case "QA":
                    hostNames = new string[] { "DAPP03.DEV.CORP.LOCAL", "QARPT01.DEV.CORP.LOCAL" };
                    break;
                case "STAGING":
                    hostNames = new string[] { "SAPP03.DALLAS-DC.FT.COM", "SWEB03.DALLAS-DC.FT.COM", "SRPT01.DALLAS-DC.FT.COM", "SRPT02.DALLAS-DC.FT.COM" };
                    break;
                default:
                    throw new ArgumentException("Not a valid environment.");
            }
            return hostNames;
        }

        private Dictionary<string, ManagementScope> CreateManagementScope(string environment) {
            ConnectionOptions creds = new ConnectionOptions();
            creds.Username = "dev\\appdeploy";
            creds.Password = "Git:Rocks!";

            string[] hostNames = this.FetchServicesHostName(environment);

            Dictionary<string, ManagementScope> scopes = new Dictionary<string, ManagementScope>();

            foreach (string hostName in hostNames) {
                scopes.Add(hostName, new ManagementScope(string.Format("\\\\{0}\\root\\cimv2", hostName), creds));
            }

            return scopes;
        }

        private Dictionary<string, ManagementScope> CreateManagementScopeAppPool(string environment) {
            ConnectionOptions creds = new ConnectionOptions();
            creds.Username = "dev\\appdeploy";
            creds.Password = "Git:Rocks!";

            List<string> webServers = Common.FetchWebServers(environment);

            Dictionary<string, ManagementScope> scopes = new Dictionary<string, ManagementScope>();

            foreach (string hostName in webServers) {
                ManagementScope scope = new ManagementScope(string.Format("\\\\{0}\\root\\MicrosoftIISv2", hostName), creds);
                scope.Path.NamespacePath = "root\\MicrosoftIISv2";
                scope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
                scope.Options.Impersonation = ImpersonationLevel.Impersonate;
                scope.Options.EnablePrivileges = true;

                scopes.Add(hostName, scope);
            }

            return scopes;
        }
        #endregion Private Methods
    }
}