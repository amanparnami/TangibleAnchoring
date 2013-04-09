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
        private string label;
        [XmlElement]
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        private string type;
        [XmlElement]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string questionId;
        [XmlElement]
        public string QuestionId
        {
            get { return questionId; }
            set { questionId = value; }
        }

        private string answerIds;
        [XmlElement]
        public string AnswerIds
        {
            get { return answerIds; }
            set { answerIds = value; }
        }

        
    }
}
