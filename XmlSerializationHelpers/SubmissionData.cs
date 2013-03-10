using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class SubmissionData
    {
        /// <summary>
        /// The submissions.
        /// </summary>
        public Submission[] Submissions;

        /// <summary>
        /// Deserializes an Submission from an XML file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SubmissionData Deserialize(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SubmissionData));
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return (SubmissionData)serializer.Deserialize(stream);
            }
        }
    }
}
