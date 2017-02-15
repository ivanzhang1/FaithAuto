using Restify;

namespace FellowshipOne.Api.Groups.Sets {

    public class GroupTypes : ApiSet<Model.GroupType> {
        private readonly string _baseUrl = string.Empty;
        private const string LIST_URL = "/groups/v1/grouptypes";
        private const string GET_URL = "/groups/v1/grouptypes/{0}";
        private const string SEARCH_URL = "/groups/v1/grouptypes/search";

        public GroupTypes(OAuthTicket ticket, string baseUrl)
            : base(ticket, baseUrl, ContentType.XML) {
            _baseUrl = baseUrl;
        }

        protected override string ListUrl { get { return LIST_URL; } }

        protected override string GetUrl { get { return GET_URL; } }

        protected override string SearchUrl { get { return SEARCH_URL; } }
    }
}
