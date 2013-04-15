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
         /// <summary>
        /// Question QuestionId to which response was made.
        /// </summary>
        [XmlElement]
        public string QuestionId;
 
        /// <summary>
        /// Answer QuestionId that corresponds to response.
        /// </summary>
        [XmlElement]
        public string AnswerId;
 
        /// <summary>
        /// Name of media file if a video was recorded else 0.
        /// </summary>
        [XmlElement]
        public string MediaName;        
    }
}
