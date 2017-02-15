using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FellowshipOne.Api.Model;

namespace FellowshipOne.Api.People.Model
{
    [Serializable]
    public class LoserId
    {

        [XmlAttribute("id")]
        public int id
        {
            get;
            set;
        }

        [XmlAttribute("uri")]
        public string uri
        {
            get;
            set;
        }

    }
}
