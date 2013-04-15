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

        //private ARange[] answerRange;

        //public ARange[] AnswerRange
        //{
        //    get { return answerRange; }
        //    set { answerRange = value; }
        //}

        private string answerRange;

        public string AnswerRange
        {
            get { return answerRange; }
            set { answerRange = value; }
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
            answerRange = xmlData.AnswerRange;

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
            //    answerRange = new ARange[numRange];
            //    for (int index = 0; index < numRange; index++)
            //    {
            //        answerRange[index] = new ARange(xmlData.AnswerRange[index]);
            //    }
            //}
        }
    }
}
