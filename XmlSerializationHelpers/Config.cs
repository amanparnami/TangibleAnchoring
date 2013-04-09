using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class Config
    {
        [XmlElement]
        public string SubmissionsFileUri;

        [XmlElement]
        public Tangible[] Tangibles;

        [XmlElement]
        public Question[] Questions;

        /// <summary>
        /// Deserializes Config from an XML file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Config Deserialize(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return (Config)serializer.Deserialize(stream);
            }
        }
    }
}
