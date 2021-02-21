using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Attributes
{
    internal class XmlDescriptionAttribute : Attribute
    {
        public string description;

        public XmlDescriptionAttribute(string des)
        {
            description = des;
        }
    }
}