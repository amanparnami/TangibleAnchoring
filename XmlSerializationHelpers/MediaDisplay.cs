using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class MediaDisplay
    {
        [XmlElement]
        public string Priority;

        public PriorityQuestion Viewpoint;

        public PriorityQuestion[] PriorityQuestions;
    }
}
