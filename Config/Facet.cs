using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class Facet
    {
        private string label;

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string questionId;

        public string QuestionId
        {
            get { return questionId; }
            set { questionId = value; }
        }

        private string answerIds;

        public string AnswerIds
        {
            get { return answerIds; }
            set { answerIds = value; }
        }

        public Facet(XmlSerializationHelpers.Facet xmlData)
        {
            label = xmlData.Label;
            type = xmlData.Type;
            questionId = xmlData.QuestionId;
            answerIds = xmlData.AnswerIds;
        }
    }
}
