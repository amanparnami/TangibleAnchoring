using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TangibleAnchoring.Config
{
    public class ARange
    {
        private string rStartValue;

        public string RStartValue
        {
            get { return rStartValue; }
            set { rStartValue = value; }
        }

        private string rEndValue;

        public string REndValue
        {
            get { return rEndValue; }
            set { rEndValue = value; }
        }

        private string rIncrement;

        public string RIncrement
        {
            get { return rIncrement; }
            set { rIncrement = value; }
        }

        public ARange(XmlSerializationHelpers.ARange xmlData)
        {
            rStartValue = xmlData.RStartValue;
            rEndValue = xmlData.REndValue;
            rIncrement = xmlData.RIncrement;
        }

    }
}
