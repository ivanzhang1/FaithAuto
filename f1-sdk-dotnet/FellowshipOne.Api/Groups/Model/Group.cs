using FellowshipOne.Api.Model;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace FellowshipOne.Api.Groups.Model {

    public class Group : APIModel {

        //public Group() {
        //    this.GroupType = new ParentNamedObject();
        //    this.ChurchCampus = new ParentNamedObject();
        //    this.TimeZone = new ParentNamedObject();
        //    this.CreatedByPerson = new ParentObject();
        //    this.LastUpdatedByPerson = new ParentObject();
        //}

        #region Properties

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("desciption")]
        public string Description { get; set; }

        //[XmlElement("startDate")]
        //public DateTime StartDate { get; set; }

        //[XmlElement("expirationDate")]
        //public DateTime ExpirationDate { get; set; }

        //[XmlElement("isOpen")]
        //public bool IsOpen { get; set; }

        //[XmlElement("isPublic")]
        //public bool IsPublic { get; set; }

        //[XmlElement("hasChildcare")]
        //public bool HasChildcare { get; set; }

        //[XmlElement("isSearchable")]
        //public bool IsSearchable { get; set; }

        //[XmlAnyElement("churchCampus")]
        //public ParentNamedObject ChurchCampus { get; set; }

        //[XmlElement("groupType")]
        //public ParentNamedObject GroupType { get; set; }

        //[XmlElement("groupURL")]
        //public string GroupURL { get; set; }

        //[XmlAnyElement("timeZone")]
        //public ParentNamedObject TimeZone { get; set; }

        //[XmlAnyElement("gender")]
        //public ParentNamedObject Gender { get; set; }

        //[XmlAnyElement("maritalStatus")]
        //public ParentNamedObject MaritalStatus { get; set; }

        //[XmlElement("startAgeRange")]
        //public DateTime StartAgeRange { get; set; }

        //[XmlElement("endAgeRange")]
        //public DateTime EndAgeRange { get; set; }

        //[XmlAnyElement("dateRangeType")]
        //public ParentNamedObject DateRangeType { get; set; }

        //[XmlAnyElement("leadersCount")]
        //public int LeadersCount { get; set; }

        //[XmlAnyElement("membersCount")]
        //public int MembersCount { get; set; }

        //[XmlAnyElement("openProspectsCount")]
        //public int OpenProspectsCount { get; set; }

        //[XmlAnyElement("event")]
        //public ParentNamedObject Event { get; set; }

        //[XmlAnyElement("location")]
        //public ParentNamedObject Location { get; set; }

        //[XmlElement("createdDate")]
        //public DateTime CreatedDate { get; set; }

        //[XmlElement("createdByPerson")]
        //public ParentObject CreatedByPerson { get; set; }

        //[XmlElement("lastUpdatedDate")]
        //public DateTime LastUpdatedDate { get; set; }

        //[XmlElement("lastUpdatedByPerson")]
        //public ParentObject LastUpdatedByPerson { get; set; }

        //[XmlAnyElement("isLocationPrivate")]
        //public bool IsLocationPrivate { get; set; }

        #endregion Properties
    }
}