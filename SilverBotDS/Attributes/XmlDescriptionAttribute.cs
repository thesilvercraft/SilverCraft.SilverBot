using System;

namespace SilverBotDS.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class XmlDescriptionAttribute : Attribute
    {
        public string description;

        public XmlDescriptionAttribute(string des) => description = des;
    }
    [AttributeUsage(AttributeTargets.Property)]
    internal class XmlCommentInsideAttribute : Attribute
    {
        public string comment;

        public XmlCommentInsideAttribute(string des) => comment = des;
    }
}