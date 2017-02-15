using FellowshipOne.Api.QueryObject;
using Restify.Attributes;

namespace FellowshipOne.Api.Groups.QueryObject {
    public class GroupQO : BaseQO {
        [QO("searchFor")]
        public string Name { get; set; }

        [QO("isSearchable")]
        public bool IsSearchable { get; set; }

        [QO("genderID")]
        public string GenderID { get; set; }

        [QO("maritalStatusID")]
        public string MaritalStatusID { get; set; }

        [QO("churchCampusID")]
        public string ChurchCampusID { get; set; }
    }
}
