using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace QAUtil {
    public static class Common {
        /// <summary>
        /// Fetches the appropriate database connection string based on the selected environment.
        /// </summary>
        /// <param name="environment">The target environment.</param>
        /// <returns>A database connection string.</returns>
        public static string FetchDBConnectionString(string environment) {
            string connectionString = string.Empty;

            switch (environment) {
                case "DEV":
                    connectionString = "data source=DEVDB.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case "QA":
                    connectionString = "data source=QADB.DEV.CORP.LOCAL,65317;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case "STAGING":
                    connectionString = "data source=ftstage.staging.dallas-dc.ft.com,65316;initial catalog=ChmContribution;password=staging;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                default:
                    throw new ArgumentException("Not a valid environment.");
            }
            return connectionString;
        }

        /// <summary>
        /// Executes a provided database query against the specified environment.
        /// </summary>
        /// <param name="environment">The target environment.</param>
        /// <param name="query">The query to be executed.</param>
        /// <returns>An object representing the data returned from the query.</returns>
        public static object ExecuteDBQuery(string environment, string query) {
            object data;

            // Switch here for the environment, store the connection strings here, not in the aspx page!!
            string connectionString = string.Empty;

            switch (environment) {
                case "DEV":
                    connectionString = "data source=DEVDB.DEV.CORP.LOCAL,65316;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case "QA":
                    connectionString = "data source=QADB.DEV.CORP.LOCAL,65317;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                case "STAGING":
                    connectionString = "data source=ftstage.staging.dallas-dc.ft.com,65316;initial catalog=ChmPeople;password=staging;persist security info=True;user id=chm_system;packet size=4096";
                    break;
                default:
                    throw new ArgumentException("Not a valid environment.");
            }

            using (SqlConnection dbConnection = new SqlConnection(connectionString)) {
                using (SqlCommand cmd = new SqlCommand(query.ToString(), dbConnection)) {
                    dbConnection.Open();
                    data = cmd.ExecuteScalar();
                }
                dbConnection.Close();
            }
            return data;
        }

        /// <summary>
        /// Fetches a list of web servers based on the selected environment.
        /// </summary>
        /// <param name="environment">The target environment.</param>
        /// <returns>The list of web servers for the given environment.</returns>
        public static List<string> FetchWebServers(string environment) {
            List<string> webServers = new List<string>();

            switch (environment) {
                case "DEV":
                    webServers.Add("DWEB01.DEV.CORP.LOCAL");
                    webServers.Add("DWEB08.DEV.CORP.LOCAL");
                    break;
                case "QA":
                    webServers.Add("DWEB02.DEV.CORP.LOCAL");
                    webServers.Add("QWEB08.DEV.CORP.LOCAL");
                    break;
                case "STAGING":
                    webServers.Add("SWEB01.DALLAS-DC.FT.COM");
                    webServers.Add("SWEB03.DALLAS-DC.FT.COM");
                    break;
                default:
                    throw new ArgumentException("Not a valid environment.");
            }
            return webServers;
        }
    }
}