using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class Facet
    {
        [XmlElement]
        public string Label;

        [XmlElement]
        public string Type;

        [XmlElement]
        public string QuestionId;

        [XmlElement]
        public string AnswerIds;

        [XmlElement]
        public string FacetRange;

    }
}
