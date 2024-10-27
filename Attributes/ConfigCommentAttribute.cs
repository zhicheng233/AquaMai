using System;

namespace AquaMai.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigCommentAttribute(string en = null, string zh = null) : Attribute
{
    public string CommentEn { get; } = en;
    public string CommentZh { get; } = zh;
}
