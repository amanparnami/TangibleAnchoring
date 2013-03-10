using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Submissions
{
    /// <summary>
    /// Represents the definition of a response within a submission.
    /// </summary>
    public class Response
    {
        private readonly string questionId;
        private readonly string answerId;
        private readonly string mediaName;

        /// <summary>
        /// Question Id to which response was made.
        /// </summary>
        public string QuestionId
        {
            get { return questionId; }
        }

        /// <summary>
        /// Answer Id that corresponds to response.
        /// </summary>
        public string AnswerId
        {
            get { return answerId; }
        }

        /// <summary>
        /// Name of media file if a video was recorded else 0.
        /// </summary>
        public string MediaName
        {
            get { return mediaName; }
        }

        public Response(XmlSerializationHelpers.Response xmlResponse)
        {
            questionId = xmlResponse.QuestionId;
            answerId = xmlResponse.AnswerId;
            mediaName = xmlResponse.MediaName;
        }

    }
}
