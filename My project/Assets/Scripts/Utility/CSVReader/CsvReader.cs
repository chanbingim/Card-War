using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CsvReader
{
    struct CsvReuslt
    {
        public string[] Headers;
        public Dictionary<string, List<string>> datas;
    }

    public static IReadOnlyList<T> ReadCSV<T>(TextAsset asset) where T : new ()
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset));

        if (string.IsNullOrEmpty(asset.text))
            return Array.Empty<T>();

        string[] rows = asset.text.Split('\n');

        List<T> result = new List<T>();
        string Col = rows[0].TrimEnd('\r');

        var members = CreateTuple<T>(Col.Split(','));
        for (int lineIndex = 1; lineIndex < rows.Length; lineIndex++)
        {
            Col = rows[lineIndex].TrimEnd('\r');
            var values = Col.Split(",");
            if (values.Length == 0 || string.IsNullOrWhiteSpace(values[0]))
                continue;

            T classInstance = new T();

            for (int memberIndex = 0; memberIndex < members.Count; memberIndex++)
            {
                //튜플 구조분해
                var (member, type) = members[memberIndex];

                if (member == null) continue;

                var raw = values[memberIndex].Trim();
                var convertedRaw = ConvertTo(raw, type);

                if (member is FieldInfo fInfo)
                    fInfo.SetValue(classInstance, convertedRaw);
                else if (member is PropertyInfo proInfo)
                    proInfo.SetValue(classInstance, convertedRaw);
            }
            result.Add(classInstance);
        }
        return result;
    }

    private static object ConvertTo(string value, Type type)
    {
        if (type == typeof(int)) return int.Parse(value);
        if (type == typeof(float)) return float.Parse(value);
        if (type == typeof(double)) return double.Parse(value);
        if (type == typeof(bool)) return bool.Parse(value.ToLower());
        if (type == typeof(string)) return value;
        if (type.IsEnum) return Enum.Parse(type, value);

        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return Convert.ChangeType(value, underlying);
        }

        return Convert.ChangeType(value, type);
    }

    private static List<(MemberInfo member, Type type)> CreateTuple<T>(string[] headers)
    {
        var members = new List<(MemberInfo member, Type type)>();
        foreach (var header in headers)
        {
            //필드 : public int ID
            var field = typeof(T).GetField(header);
            if (field != null)
            {
                //튜플구조
                members.Add((field, field.FieldType));
                continue;
            }

            //프로퍼티 : public int ID { get; set; }
            var property = typeof(T).GetProperty(header);
            if (property != null && property.CanWrite)
            {
                //튜플구조
                members.Add((property, property.PropertyType));
                continue;
            }

            members.Add((null, null));
        }

        return members.Count == 0 ? null : members;
    }


}

