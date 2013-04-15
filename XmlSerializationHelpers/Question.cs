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
        private string questionId;
        [XmlElement]
        public string QuestionId
        {
            get { return questionId; }
            set { questionId = value; }
        }

        private string questionText;
        [XmlElement]
        public string QuestionText
        {
            get { return questionText; }
            set { questionText = value; }
        }

        private string blah;
        [XmlElement]
        public string Blah
        {
            get { return blah; }
            set { blah = value; }
        }


        //public ARange[] AnswerRange;

        public Answer[] Answers;
        
        
       
    }
}
