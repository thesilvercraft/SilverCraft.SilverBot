using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class CategoryAttribute: Attribute
    {
        public string[] Category;
        public CategoryAttribute(params string[] thing)
        {
            Category = thing;
        }
    }
}
