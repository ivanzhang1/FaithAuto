using System.Collections.Generic;
using Restify;

namespace FellowshipOne.Api.People.Sets {
    public class Mergers : ApiSet<Model.Merger> {
        private const string MERGER_LIST_URL = "/v1/people/mergers/search";

        public Mergers(OAuthTicket ticket, string baseUrl) : base(ticket, baseUrl, ContentType.XML) { }

        protected override string SearchUrl { get { return MERGER_LIST_URL; } }

    }
}
