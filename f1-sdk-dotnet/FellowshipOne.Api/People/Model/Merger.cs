using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellowshipOne.Api.Model;
using System.Xml;
using System.Xml.Serialization;

namespace FellowshipOne.Api.People.Model {
    [Serializable]
    [XmlRoot("merger")]
    public partial class Merger : APIModel {

        [XmlElement("winnerId")]
        public WinnerId WinnerId
        {
            get;
            set;
        }

        [XmlElement("loserId")]
        public LoserId LoserId
        {
            get;
            set;
        }

        [XmlIgnore]
        public DateTime? LastUpdatedDate {
            get {
                DateTime lastUpdatedDate = DateTime.MinValue;

                if (DateTime.TryParse(_lastUpdatedDateString, out lastUpdatedDate)) {
                    return lastUpdatedDate;
                }

                return null;
            }
            set {
                if (value.HasValue) {
                    _lastUpdatedDateString = value.Value.ToString();
                }
                else {
                    _lastUpdatedDateString = string.Empty;
                }
            }
        }

        private string _lastUpdatedDateString = string.Empty;
        [XmlElement("lastUpdatedDate")]
        public string LastUpdatedDateString {
            get {
                if (!string.IsNullOrEmpty(_lastUpdatedDateString)) {
                    DateTime dt = DateTime.Now;

                    if (DateTime.TryParse(_lastUpdatedDateString, out dt)) {
                        _lastUpdatedDateString = dt.ToString("s");
                    }
                }

                return _lastUpdatedDateString; 
            }
            set {
                if (value != null) {
                    _lastUpdatedDateString = value;
                }
            }
        }

    }

    [Serializable]
    [XmlRoot("results")]
    public class MergerCollection : Collection<Merger> {
        public  MergerCollection() { }
        public MergerCollection(List<Merger> merger) {
            if (merger != null) {
                this.AddRange(merger);
            }
        }
    }
}