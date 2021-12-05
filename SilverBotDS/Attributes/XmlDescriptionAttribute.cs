using System;

namespace SilverBotDS.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XmlDescriptionAttribute : Attribute
    {
        public string description;

        public XmlDescriptionAttribute(string des) => description = des;
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class XmlCommentInsideAttribute : Attribute
    {
        public string comment;

        public XmlCommentInsideAttribute(string des) => comment = des;
    }
}