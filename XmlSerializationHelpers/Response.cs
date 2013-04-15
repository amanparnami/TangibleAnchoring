using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class Response
    {
        private string questionId;
        /// <summary>
        /// Question QuestionId to which response was made.
        /// </summary>
        [XmlElement]
        public string QuestionId
        {
            get { return questionId; }
            set { questionId = value; }
        }

        private string answerId;
        /// <summary>
        /// Answer QuestionId that corresponds to response.
        /// </summary>
        [XmlElement]
        public string AnswerId
        {
            get { return answerId; }
            set { answerId = value; }
        }

        private string mediaName;
        /// <summary>
        /// Name of media file if a video was recorded else 0.
        /// </summary>
        [XmlElement]
        public string MediaName
        {
            get { return mediaName; }
            set { mediaName = value; }
        }
        
        
    }
}
