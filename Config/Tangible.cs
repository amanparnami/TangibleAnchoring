using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class Tangible
    {
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string tagId;

        public string TagId
        {
            get { return tagId; }
            set { tagId = value; }
        }

        private string name;

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

        public Tangible(XmlSerializationHelpers.Tangible xmlTangible)
        {
            type = xmlTangible.Type;
            tagId = xmlTangible.TagId;
            name = xmlTangible.Name;

            
            if (xmlTangible.Rotation != null)
            {
                int numFacets = xmlTangible.Rotation.Length;
                rotation = new Facet[numFacets];
                for (int index = 0; index < numFacets; ++index)
                {
                    rotation[index] = new Facet(xmlTangible.Rotation[index]);
                }
            }


        }

    }
}
