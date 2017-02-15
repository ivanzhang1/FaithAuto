using FellowshipOne.Api;
using MbUnit.Framework;
using System;
using System.Configuration;

namespace API {

    [TestFixture]
    internal class Base {
        internal RestClient RestClient;
        internal F1OAuthTicket Ticket;

        private ExeConfigurationFileMap _appConfigFileMap = new ExeConfigurationFileMap();

        private Configuration _config;
        private string _environment;
        private string _consumerKey;
        private string _consumerSecret;
        private string _churchcode;
        private string _username;
        private string _password;
        private string _accessToken;
        private string _accessTokenSecret;
        private string _baseUrl;
        private string _authUrl;

        protected FTTests.SQL _sql;
        private string _dbConnectionString;

        public void BuildBaseUrl() {
            
            switch (_environment) {
                case "local":
                case "LOCAL":
                    this._baseUrl = string.Format("http://{0}.fellowshiponeapi.local/", _churchcode);
                    this._dbConnectionString = @"data source=localhost;initial catalog=ChmContribution;password=Fris121co;persist security info=True;user id=chm_system;packet size=4096";
                    return;

                case "LV_QA":
                case "lv_qa":
                    this._baseUrl = string.Format("https://{0}.{1}.fellowshiponeapi.com/", _churchcode, "qa");
                    this._dbConnectionString = string.Format("data source=transdb.{0}.fellowshipone.com;initial catalog=ChmContribution;persist security info=True;integrated security=True;packet size=4096", "qa");
                    return;

                case "int":
                case "INT":
                case "int2":
                case "INT2":
                case "int3":
                case "INT3":
                case "qa":
                case "QA":
                case "staging":
                case "STAGING":
                    this._baseUrl = string.Format("https://{0}.{1}.fellowshiponeapi.com/", _churchcode, _environment);
                    this._dbConnectionString = string.Format("data source=transdb.{0}.fellowshipone.com;initial catalog=ChmContribution;persist security info=True;integrated security=True;packet size=4096", _environment);
                    return;

                case "production":
                    this._baseUrl = string.Format("https://{0}.fellowshiponeapi.com/", _churchcode);
                    this._dbConnectionString = "";
                    return;

                default:
                    throw new Exception("The environment is not set correctly.  Check the config file");
            }
        }

        public enum LoginType {
            PortalUser = 1,
            WeblinkUser = 2
        }

        [FixtureSetUp]
        public void Setup() {
            _appConfigFileMap.ExeConfigFilename = @"..\..\..\Common\bin\Debug\Common.dll.config"; //"C:\\dev\\tests\\Common\\bin\\Debug\\Common.dll.config";
            _config = ConfigurationManager.OpenMappedExeConfiguration(_appConfigFileMap, ConfigurationUserLevel.None);

            this._environment = _config.AppSettings.Settings["FTTests.Environment"].Value;
            this._consumerKey = _config.AppSettings.Settings["FTTests.APIConsumerKey"].Value;
            this._consumerSecret = _config.AppSettings.Settings["FTTests.APIConsumerSecret"].Value;
            this._username = _config.AppSettings.Settings["FTTests.APIUsername"].Value;
            this._password = _config.AppSettings.Settings["FTTests.APIPassword"].Value;
            this._churchcode = _config.AppSettings.Settings["FTTests.APIChurchCode"].Value;
            this._authUrl = string.Format("v1/{0}/accesstoken", LoginType.PortalUser);
            this._accessToken = "";
            this._accessTokenSecret = "";
            BuildBaseUrl();

            this.Ticket = new F1OAuthTicket {
                ConsumerKey = _consumerKey,
                ConsumerSecret = _consumerSecret,
                ChurchCode = _churchcode,
                AccessToken = _accessToken,
                AccessTokenSecret = _accessTokenSecret,
                BaseURL = _baseUrl,
                AuthURL = _authUrl
            };

            RestClient = new RestClient(Ticket);

            var oauth = RestClient.AuthorizeWithCredentials(Ticket, _username, _password, Ticket.BaseURL, Ticket.AuthURL);

            Ticket.AccessToken = oauth.AccessToken;
            Ticket.AccessTokenSecret = oauth.AccessTokenSecret;

            // Create a new SQL class
            if (!"".Equals(this._dbConnectionString))
            {
                this._sql = new FTTests.SQL(this._dbConnectionString);

            }
        }
    }
}