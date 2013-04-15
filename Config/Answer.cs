using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class Answer
    {
        private string answerId;

        public string AnswerId
        {
            get { return answerId; }
            set { answerId = value; }
        }

        private string answerText;

        public string AnswerText
        {
            get { return answerText; }
            set { answerText = value; }
        }

        public Answer(XmlSerializationHelpers.Answer xmlData)
        {
            answerId = xmlData.AnswerId;
            answerText = xmlData.AnswerText;
        }
    }
}
