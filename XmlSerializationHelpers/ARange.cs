using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class ARange
    {
        [XmlElement]
        public string RStartValue;

        [XmlElement]
        public string REndValue;

        [XmlElement]
        public string RIncrement;
    }
}
