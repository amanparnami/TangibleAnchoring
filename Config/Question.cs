using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class Question
    {
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private Answer[] answers;

        public Answer[] Answers
        {
            get { return answers; }
            set { answers = value; }
        }

        public Question(XmlSerializationHelpers.Question xmlData)
        {
            id = xmlData.Id;
            text = xmlData.Text;

            if (xmlData.Answers != null)
            {
                int numAnswers = xmlData.Answers.Length;
                answers = new Answer[numAnswers];
                for (int index = 0; index < numAnswers; index++)
                {
                    answers[index] = new Answer(xmlData.Answers[index]);
                }
            }
        }
    }
}
