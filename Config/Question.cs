using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class Question
    {
        private string questionId;

        public string QuestionId
        {
            get { return questionId; }
            set { questionId = value; }
        }

        private string questionText;

        public string QuestionText
        {
            get { return questionText; }
            set { questionText = value; }
        }

        //private ARange[] blah;

        //public ARange[] AnswerRange
        //{
        //    get { return blah; }
        //    set { blah = value; }
        //}

        private string blah;

        public string Blah
        {
            get { return blah; }
            set { blah = value; }
        }


        private Answer[] answers;

        public Answer[] Answers
        {
            get { return answers; }
            set { answers = value; }
        }

        

        public Question(XmlSerializationHelpers.Question xmlData)
        {
            questionId = xmlData.QuestionId;
            questionText = xmlData.QuestionText;
            blah = xmlData.Blah;

            if (xmlData.Answers != null)
            {
                int numAnswers = xmlData.Answers.Length;
                answers = new Answer[numAnswers];
                for (int index = 0; index < numAnswers; index++)
                {
                    answers[index] = new Answer(xmlData.Answers[index]);
                }
            }


            //if (xmlData.AnswerRange != null)
            //{
            //    int numRange = xmlData.AnswerRange.Length;
            //    blah = new ARange[numRange];
            //    for (int index = 0; index < numRange; index++)
            //    {
            //        blah[index] = new ARange(xmlData.AnswerRange[index]);
            //    }
            //}
        }
    }
}
