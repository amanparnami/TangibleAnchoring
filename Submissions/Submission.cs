using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Submissions
{
    /// <summary>
    /// Represents the definition of a submission.
    /// </summary>
    public class Submission
    {
        private readonly string pollId;
        private readonly string userId;
        private readonly int age;
        private readonly double latitude;
        private readonly double longitude;
        private readonly Response[] responses;

        /// <summary>
        /// Gets the poll id associated with the submission.
        /// </summary>
        public string PollId { get { return pollId; } }

        /// <summary>
        /// Gets the user/respondent id associated with the submission.
        /// </summary>
        public string UserId { get { return userId; } }

        /// <summary>
        /// Gets the age of the respondent associated with the submission.
        /// </summary>
        public int Age { get { return age; } }

        /// <summary>
        /// Gets the position latitude associated with the submission.
        /// </summary>
        public double Latitude { get { return latitude; } }

        /// <summary>
        /// Gets the position longitude associated with the submission.
        /// </summary>
        public double Longitude { get { return longitude; } }

        /// <summary>
        /// Gets the responses to questions associated with the submission.
        /// </summary>
        public Submissions.Response[] Responses { get { return responses; } }

        internal Submission(XmlSerializationHelpers.Submission xmlSubmission)
        {
            pollId = xmlSubmission.PollId;
            userId = xmlSubmission.UserId;
            latitude = xmlSubmission.Latitude;
            longitude = xmlSubmission.Longitude;
            age = xmlSubmission.Age;

            if (xmlSubmission.Responses != null)
            {
                // corresponds to the number of responses listed per submission
                int numResponses = xmlSubmission.Responses.Length;
                responses = new Response[numResponses];

                for (int index = 0; index < numResponses; ++index)
                {
                    responses[index] = new Response(xmlSubmission.Responses[index]);
                }
            }
            
        }
    }
}
