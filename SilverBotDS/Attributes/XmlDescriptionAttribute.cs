using System;

namespace SilverBotDS.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class XmlDescriptionAttribute : Attribute
{
    public string Description;

    public XmlDescriptionAttribute(string des)
    {
        Description = des;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class XmlCommentInsideAttribute : Attribute
{
    public string Comment;

    public XmlCommentInsideAttribute(string des)
    {
        Comment = des;
    }
}