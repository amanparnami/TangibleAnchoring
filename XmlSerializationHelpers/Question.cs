using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class Question
    {
        [XmlElement]
        public string QuestionId;

        [XmlElement]
        public string QuestionText;

        [XmlElement]
        public string AnswerRange;

        //public ARange[] AnswerRange;

        public Answer[] Answers;
    }
}
