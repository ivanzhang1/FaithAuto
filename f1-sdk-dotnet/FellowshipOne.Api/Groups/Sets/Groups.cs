using Restify;

namespace FellowshipOne.Api.Groups.Sets {

    public class Groups : ApiSet<Model.Group> {
        private readonly string _baseUrl = string.Empty;
        private const string CHILD_LIST_URL = "/groups/v1/grouptypes/{0}/groups";
        private const string GET_URL = "/groups/v1/groups/{0}";
        private const string SEARCH_URL = "/groups/v1/groups/search";
        private const string CREATE_URL = "/groups/v1/groups";

        public Groups(OAuthTicket ticket, string baseUrl)
            : base(ticket, baseUrl, ContentType.XML) {
            _baseUrl = baseUrl;
        }

        protected override string GetChildListUrl {
            get {
                return CHILD_LIST_URL;
            }
        }

        protected override string GetUrl { get { return GET_URL; } }

        protected override string SearchUrl { get { return SEARCH_URL; } }

        protected override string CreateUrl { get { return CREATE_URL; } }
    }
}