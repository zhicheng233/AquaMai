using System;
using AquaMai.Config.Interfaces;

namespace AquaMai.Config.Attributes;

public record ConfigComment(string CommentEn, string CommentZh) : IConfigComment
{
    public string GetLocalized(string lang) => lang switch
    {
        "en" => CommentEn ?? "",
        "zh" => CommentZh ?? "",
        _ => throw new ArgumentException($"Unsupported language: {lang}")
    };
}
