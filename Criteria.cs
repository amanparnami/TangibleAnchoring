using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring
{
    public class Criteria
    {
        private string questionId;

        public string QuestionId
        {
            get { return questionId; }
            set { questionId = value; }
        }

        private string[] answerIds;

        public string[] AnswerIds
        {
            get { return answerIds; }
            set { answerIds = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="questionId">Question questionId to be checked</param>
        /// <param name="csvAnswerIds">Comma separated answer ids</param>
        public Criteria(string questionId, string csvAnswerIds)
        {
            this.questionId = questionId;

            int numAnswerIds = csvAnswerIds.Split(',').Length;
            answerIds = new string[numAnswerIds];
            string[] answerIdArr = csvAnswerIds.Split(',');
            for (int i = 0; i < numAnswerIds; i++)
            {
                answerIds[i] = answerIdArr[i].Trim();
            }
        }
    }
}
