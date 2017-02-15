using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellowshipOne.Api.QueryObject;
using Restify.Attributes;

namespace FellowshipOne.Api.People.QueryObject {
    public class MergersQO : BaseQO {

        [QO("lastUpdatedDate")]
        public DateTime? LastUpdatedDate { get; set; }

        
    }
}
