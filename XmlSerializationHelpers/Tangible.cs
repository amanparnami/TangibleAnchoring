using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TangibleAnchoring.XmlSerializationHelpers
{
    public class Tangible
    {
        private string type;

        [XmlElement]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string tagId;
        [XmlElement]
        public string TagId
        {
            get { return tagId; }
            set { tagId = value; }
        }

        private string name;
        [XmlElement]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Facet[] rotation;
        
        public Facet[] Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

    }
}
