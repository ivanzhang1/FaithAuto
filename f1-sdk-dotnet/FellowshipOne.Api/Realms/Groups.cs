using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellowshipOne.Api.Realms {

    public class F1Groups {

        #region Properties

        private F1OAuthTicket _ticket { get; set; }

        private string _baseUrl { get; set; }

        private FellowshipOne.Api.Groups.Sets.GroupTypes _groupTypes;

        public FellowshipOne.Api.Groups.Sets.GroupTypes GroupTypes {
            get {
                if (_groupTypes == null) {
                    _groupTypes = new FellowshipOne.Api.Groups.Sets.GroupTypes(_ticket, _baseUrl);
                }
                return _groupTypes;
            }
        }

        private FellowshipOne.Api.Groups.Sets.Groups _groups;
        public FellowshipOne.Api.Groups.Sets.Groups Groups {
            get {
                if (_groups == null) {
                    _groups = new FellowshipOne.Api.Groups.Sets.Groups(_ticket, _baseUrl);
                }
                return _groups;
            }
        }

        #endregion Properties

        #region Constructor

        public F1Groups(F1OAuthTicket ticket, string baseUrl) {
            _ticket = ticket;
            _baseUrl = baseUrl;
        }

        #endregion Constructor
    }
}