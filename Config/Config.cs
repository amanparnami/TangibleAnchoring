﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class Config
    {
        private string submissionsFileUri;

        public string SubmissionsFileUri
        {
            get { return submissionsFileUri; }
            set { submissionsFileUri = value; }
        }

        private Tangible[] tangibles;

        public Tangible[] Tangibles
        {
            get { return tangibles; }
            set { tangibles = value; }
        }

        private Question[] questions;

        public Question[] Questions
        {
            get { return questions; }
            set { questions = value; }
        }

        private MediaDisplay display;

        public MediaDisplay Display
        {
            get { return display; }
            set { display = value; }
        }


        public Config(string filePath)
        {
            XmlSerializationHelpers.Config xmlData = XmlSerializationHelpers.Config.Deserialize(filePath);

            submissionsFileUri = xmlData.SubmissionsFileUri;

            if (xmlData.Tangibles != null)
            {
                int numTangibles = xmlData.Tangibles.Length;
                tangibles = new Tangible[numTangibles];
                for (int index = 0; index < numTangibles; ++index)
                {
                    tangibles[index] = new Tangible(xmlData.Tangibles[index]);
                }
            }

            if (xmlData.Questions != null)
            {
                int numQuestions = xmlData.Questions.Length;
                questions = new Question[numQuestions];
                for (int index = 0; index < numQuestions; ++index)
                {
                    questions[index] = new Question(xmlData.Questions[index]);
                }
            }

            if (xmlData.Display != null)
            {
                display = new MediaDisplay(xmlData.Display);
            }
        }

        public Question FindQuestionFromId(string qId)
        {
            foreach (Question ques in questions)
            {
                if (ques.QuestionId == qId)
                {
                    return ques;
                }
            }
            return null;
        }

        public Answer FindAnswerFromQuesIdAnsId(string qId, string aId)
        {
            foreach (Question ques in questions)
            {
                if (ques.QuestionId == qId)
                {
                    foreach (Answer ans in ques.Answers)
                    {
                        if (ans.AnswerId == aId)
                        {
                            return ans;
                        }
                    }
                }
            }
            return null;
        }

        public Tangible FindTangibleFromId(string tId)
        {
            foreach (Tangible tang in tangibles)
            {
                if (tang.TagId == tId)
                {
                    return tang;
                }
            }
            return null;
        }
    }
}
