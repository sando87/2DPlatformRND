using System;
using System.Collections.Generic;
using System.Reflection;

namespace PahlBit
{
    public struct FieldData
    {
        public string Name;
        public string Type;
        public string Value;
    }

    public static class ReflectionFieldExtractor
    {
        private static Dictionary<Type, FieldInfo[]> _cache = new Dictionary<Type, FieldInfo[]>();

        public static void GetFields(object target, List<FieldData> result)
        {
            result.Clear();
            Type type = target.GetType();

            FieldInfo[] fields = GetCachedFields(type);

            foreach (var field in fields)
            {
                object value = field.GetValue(target);

                result.Add(new FieldData
                {
                    Name = field.Name,
                    Type = GetFriendlyTypeName(field.FieldType),
                    Value = ConvertValueToString(value)
                });
            }
        }

        // ------------------------
        // 내부 유틸
        // ------------------------

        private static FieldInfo[] GetCachedFields(Type type)
        {
            if (_cache.TryGetValue(type, out var fields))
                return fields;

            fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            _cache[type] = fields;

            return fields;
        }

        private static string GetFriendlyTypeName(Type type)
        {
            if (type == typeof(float)) return "float";
            if (type == typeof(int)) return "int";
            if (type == typeof(bool)) return "bool";

            return type.Name;
        }

        private static string ConvertValueToString(object value)
        {
            if (value == null)
                return "null";

            if (value is float f)
                return f.ToString("0.##");

            if (value is int i)
                return i.ToString();

            if (value is bool b)
                return b.ToString();

            // ⚠️ Percent / PercentUp은 구조체 정의에 따라 다름 (확실하지 않음)
            // → ToString override 권장
            return value.ToString();
        }
    }
}
