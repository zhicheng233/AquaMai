using System;
using System.IO;
using System.Reflection;
using AquaMai.Attributes;
using Tomlet;
using Tomlet.Models;

namespace AquaMai;

public static class ConfigGenerator
{
    public static void GenerateConfig()
    {
        var defaultConfig = new Config();
        foreach (var lang in (string[]) ["en", "zh"])
        {
            File.WriteAllText($"AquaMai.{lang}.toml", SerializeConfigWithComments(defaultConfig, lang));
        }
    }

    public static string SerializeConfigWithComments(Config config, string lang)
    {
        var tomlDoc = TomletMain.DocumentFrom(config);
        MakeComments(tomlDoc, typeof(Config), lang);
        return tomlDoc.SerializedValue;
    }

    private static void MakeComments(TomlTable table, Type configType, string lang)
    {
        foreach (var property in configType.GetProperties())
        {
            var value = table.GetValue(property.Name);
            var comment = property.GetCustomAttribute<ConfigCommentAttribute>();
            if (comment != null)
            {
                value.Comments.PrecedingComment = lang switch
                {
                    "en" => comment.CommentEn,
                    "zh" => comment.CommentZh,
                    _ => throw new ArgumentException($"Unsupported language: {lang}")
                };
            }

            if (value is TomlTable subTable)
            {
                MakeComments(subTable, property.PropertyType, lang);
            }
        }
    }
}
