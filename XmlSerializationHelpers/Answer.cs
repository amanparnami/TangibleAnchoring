using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class Answer
    {
        private string id;
        [XmlElement]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string text;
        [XmlElement]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }
}
