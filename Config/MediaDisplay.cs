using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class MediaDisplay
    {
        private string priority;

        public string Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        private PriorityQuestion viewpoint;

        public PriorityQuestion Viewpoint
        {
            get { return viewpoint; }
            set { viewpoint = value; }
        }

        private PriorityQuestion[] priorityQuestions;

        public PriorityQuestion[] PriorityQuestions
        {
            get { return priorityQuestions; }
            set { priorityQuestions = value; }
        }

        public MediaDisplay(XmlSerializationHelpers.MediaDisplay xmlData) 
        {
            priority = xmlData.Priority;

            viewpoint = new PriorityQuestion(xmlData.Viewpoint);

            if (xmlData.PriorityQuestions != null)
            {
                int numPriorityQuestions = xmlData.PriorityQuestions.Length;
                priorityQuestions = new PriorityQuestion[numPriorityQuestions];
                for (int index = 0; index < numPriorityQuestions; ++index)
                {
                    priorityQuestions[index] = new PriorityQuestion(xmlData.PriorityQuestions[index]);
                }
            }
        }

        public string FindSideFromQuesIdAnsId(string qId, string aId)
        {
            if (priority == "Questions")
            {
                foreach (PriorityQuestion pQues in priorityQuestions)
                {
                    if (pQues.QuestionId == qId)
                    {
                        if (pQues.LeftAnswers.Contains(aId))
                        {
                            return "Left";
                        }
                        else { return "Right"; }
                    }
                }
            }
            else 
            {
                if (viewpoint.QuestionId == qId)
                {
                    if (viewpoint.LeftAnswers.Contains(aId))
                    {
                        return "Left";
                    }
                    else { return "Right"; }
                }
            }
            
            return "n/a";
        }

    }
}
