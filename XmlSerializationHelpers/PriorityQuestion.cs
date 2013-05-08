using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class PriorityQuestion
    {
        [XmlElement]
        public string QuestionId;

        [XmlElement]
        public string LeftAnswers;

        [XmlElement]
        public string RightAnswers;
    }
}
