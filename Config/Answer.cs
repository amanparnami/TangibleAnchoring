using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class Answer
    {
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Answer(XmlSerializationHelpers.Answer xmlData)
        {
            id = xmlData.Id;
            text = xmlData.Text;
        }
    }
}
