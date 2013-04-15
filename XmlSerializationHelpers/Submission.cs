using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class Submission
    {
         /// <summary>
        /// Gets the poll questionId associated with the submission.
        /// </summary>
        [XmlElement]
        public string PollId;
 
        /// <summary>
        /// Gets the user/respondent questionId associated with the submission.
        /// </summary>
        [XmlElement]
        public string UserId;
 
        /// <summary>
        /// Gets the position latitude associated with the submission.
        /// </summary>
        [XmlElement]
        public double Latitude;
 
        /// <summary>
        /// Gets the position longitude associated with the submission.
        /// </summary>
        [XmlElement]
        public double Longitude;
        
        /// <summary>
        /// Gets the age of the respondent associated with the submission.
        /// </summary>
        [XmlElement]
        public int Age;

        /// <summary>
        /// Gets the responses to questions associated with the submission.
        /// </summary>
        public Response[] Responses;
    }
}
