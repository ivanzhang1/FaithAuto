using FellowshipOne.Api.Model;
using System.Xml.Serialization;

namespace FellowshipOne.Api.Groups.Model {

    public class GroupType : APIModel {
        
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("isWebEnabled")]
        public string IsWebEnabled { get; set; }

        [XmlElement("isSearchable")]
        public string IsSearchable { get; set; }
    }
}