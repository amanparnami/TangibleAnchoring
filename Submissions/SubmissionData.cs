using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Submissions
{
    public class SubmissionData
    {
        private readonly Submission[] submissions;

        public Submission[] Submissions
        {
            get { return submissions; }
        }

        public SubmissionData(string filePath)
        {
            XmlSerializationHelpers.SubmissionData xmlData = XmlSerializationHelpers.SubmissionData.Deserialize(filePath);
            int numSubmissions = xmlData.Submissions.Length;
            submissions = new Submission[numSubmissions];
            if (xmlData.Submissions != null)
            {
                for (int index = 0; index < numSubmissions; ++index)
                {
                    submissions[index] = new Submission(xmlData.Submissions[index]);
                }
            }
        }
    }
}
