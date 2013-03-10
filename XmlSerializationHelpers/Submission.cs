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
        private string pollId;
        /// <summary>
        /// Gets the poll id associated with the submission.
        /// </summary>
        [XmlElement]
        public string PollId
        {
            get { return pollId; }
            set { pollId = value; }
        }

        private string userId;
        /// <summary>
        /// Gets the user/respondent id associated with the submission.
        /// </summary>
        [XmlElement]
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private double latitude;
        /// <summary>
        /// Gets the position latitude associated with the submission.
        /// </summary>
        [XmlElement]
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        private double longitude;
        /// <summary>
        /// Gets the position longitude associated with the submission.
        /// </summary>
        [XmlElement]
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        
        private int age;
        /// <summary>
        /// Gets the age of the respondent associated with the submission.
        /// </summary>
        [XmlElement]
        public int Age   
        {
            get { return age; }
            set { age = value; }
        }

        private Response[] responses;
        /// <summary>
        /// Gets the responses to questions associated with the submission.
        /// </summary>
        public Response[] Responses 
        { 
            get { return responses; }
            set { responses = value; }
        }
    }
}
