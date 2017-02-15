namespace FellowshipOne.Api {

    public class RestClient : Restify.Client {

        #region Properties

        private F1OAuthTicket _ticket { get; set; }

        #region API Sets

        public FellowshipOne.Api.Realms.Person PeopleRealm;
        public FellowshipOne.Api.Realms.Giving GivingRealm;
        public FellowshipOne.Api.Realms.F1Groups GroupsRealm;
        public FellowshipOne.Api.Realms.F1Activities ActivitiesRealm;

        #endregion API Sets

        #endregion Properties

        #region Constructor

        public RestClient(F1OAuthTicket ticket)
            : base(ticket) {
            _ticket = ticket;
            PeopleRealm = new Realms.Person(ticket, ticket.BaseURL);
            GivingRealm = new Realms.Giving(ticket, ticket.BaseURL);
            GroupsRealm = new Realms.F1Groups(ticket, ticket.BaseURL);
            ActivitiesRealm = new Realms.F1Activities(ticket, ticket.BaseURL);
        }

        #endregion Constructor
    }
}