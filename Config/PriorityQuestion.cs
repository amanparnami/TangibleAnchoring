using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class PriorityQuestion
    {
        private string questionId;

        public string QuestionId
        {
            get { return questionId; }
            set { questionId = value; }
        }

        private List<string> leftAnswers;

        public List<string> LeftAnswers
        {
            get { return leftAnswers; }
            set { leftAnswers = value; }
        }

        private List<string> rightAnswers;

        public List<string> RightAnswers
        {
            get { return rightAnswers; }
            set { rightAnswers = value; }
        }

        public PriorityQuestion(XmlSerializationHelpers.PriorityQuestion xmlData) 
        {
            questionId = xmlData.QuestionId;

            if (xmlData.LeftAnswers != null)
            {
                int numLeftAnswers = xmlData.LeftAnswers.Split(',').Length;
                leftAnswers = new List<string>();
                string[] leftAnswersArr = xmlData.LeftAnswers.Split(',');
                
                for (int index = 0; index < numLeftAnswers; index++)
                {
                    leftAnswers.Add(leftAnswersArr[index]);
                }
            }

            if (xmlData.RightAnswers != null)
            {
                int numRightAnswers = xmlData.RightAnswers.Split(',').Length;
                rightAnswers = new List<string>();
                string[] rightAnswersArr = xmlData.RightAnswers.Split(',');

                for (int index = 0; index < numRightAnswers; index++)
                {
                    rightAnswers.Add(rightAnswersArr[index]);
                }
            }
        }
    }
}
