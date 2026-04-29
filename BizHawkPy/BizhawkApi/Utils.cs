using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace BizHawkPy.BizhawkApi;


internal static class Utils
{
    public static T Parse<T>(string[] args, int index)
    {
        if (args.Length <= index)
            throw new ArgumentException($"Missing required argument at index {index}");
        var raw = args[index];

        // 空 or null → デフォルト値
        if (string.IsNullOrWhiteSpace(raw))
            return default!;

        // int? → int と?を除いた型を取得
        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        // ===== Dictionary =====
        if (type.IsGenericType &&
                (
                    (type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)) ||
                    (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                )
            )
        {
            return (T)ConvertDict(type, raw);
        }

        // ===== 配列 or List系まとめて処理 =====
        if (type.IsArray || (type.IsGenericType && type.GetGenericArguments().Length == 1))
        {
            return (T)ConvertList(type, raw);
        }

        // ===== 単体 =====
        return (T)ConvertValue(type, raw);
    }

    private static object ConvertJson(Type type, string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return type.IsValueType ? Activator.CreateInstance(type)! : null!;

        var result = JsonConvert.DeserializeObject(raw, type);
        if (result == null)
            throw new JsonException($"Deserialize failed: {raw}");

        return result;
    }

    private static object ConvertDict(Type type, string raw)
    {
        return ConvertJson(type, raw);
    }

    private static object ConvertList(Type type, string raw)
    {
        var result = ConvertJson(type, raw);

        // 配列指定なのにListで返るケースを防ぐ
        if (type.IsArray && result is System.Collections.IEnumerable enumerable)
        {
            var elementType = type.GetElementType()!;
            var list = enumerable.Cast<object?>().ToList();

            var array = Array.CreateInstance(elementType, list.Count);
            for (int i = 0; i < list.Count; i++)
                array.SetValue(list[i], i);

            return array;
        }

        return result;
    }
    private static object ConvertValue(Type type, string raw)
    {

        object result = type switch
        {
            Type t when t == typeof(object) => raw,

            Type t when t == typeof(int) => int.Parse(raw),
            Type t when t == typeof(uint) => uint.Parse(raw),

            Type t when t == typeof(short) => short.Parse(raw),
            Type t when t == typeof(ushort) => ushort.Parse(raw),

            Type t when t == typeof(long) => long.Parse(raw),
            Type t when t == typeof(ulong) => ulong.Parse(raw),

            Type t when t == typeof(byte) => byte.Parse(raw),
            Type t when t == typeof(sbyte) => sbyte.Parse(raw),

            Type t when t == typeof(float) => float.Parse(raw, CultureInfo.InvariantCulture),
            Type t when t == typeof(double) => double.Parse(raw, CultureInfo.InvariantCulture),
            Type t when t == typeof(decimal) => decimal.Parse(raw, CultureInfo.InvariantCulture),
            Type t when t == typeof(bool) => raw.ToLower() switch
            {
                "1" => true,
                "0" => false,
                _ => throw new FormatException($"Invalid bool: {raw}")
            },
            Type t when t == typeof(string) => raw,
            Type t when t.IsEnum => Enum.Parse(t, raw, true),
            _ => throw new NotSupportedException($"Unsupported type: {type.Name}")
        };
        return result;
    }

    /// <summary>
    /// luacolor を System.Drawing.Color に変換する
    /// Why: Lua側の柔軟な表現をC#で安全に扱うため
    /// </summary>
    public static Color? ToColor(string? value)
    {
        if (value is null)
            return null;

        value = value.Trim();

        // --- 10進数（例: "4294902015"）---
        if (uint.TryParse(value, out var u))
            return FromArgbUInt(u);

        // --- 16進数（0xAARRGGBB）---
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
            uint.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out u))
            return FromArgbUInt(u);

        // --- #形式 (#RRGGBB / #AARRGGBB) ---
        if (value.StartsWith("#"))
            return ParseString(value);

        // --- 色名（"red" 等） ---
        return ParseString(value);
    }

    /// <summary>
    /// 0xAARRGGBB → Color
    /// </summary>
    private static Color FromArgbUInt(uint argb)
    {
        byte a = (byte)((argb >> 24) & 0xFF);
        byte r = (byte)((argb >> 16) & 0xFF);
        byte g = (byte)((argb >> 8) & 0xFF);
        byte b = (byte)(argb & 0xFF);

        return Color.FromArgb(a, r, g, b);
    }

    /// <summary>
    /// "#RRGGBB" / "#AARRGGBB" / 色名
    /// </summary>
    private static Color ParseString(string s)
    {
        if (s.StartsWith("#"))
        {
            string hex = s.Substring(1);

            return hex.Length switch
            {
                6 => Color.FromArgb(
                    255,
                    int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                    int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                    int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber)
                ),

                8 => Color.FromArgb(
                    int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                    int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                    int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                    int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber)
                ),

                _ => throw new FormatException($"Invalid hex color: {s}")
            };
        }

        // CSS/X11 color name
        return Color.FromName(s);
    }

    public static System.Drawing.Point[] ToPointArray(object? obj)
    {
        // Why: Pythonの list[tuple[int,int]] → C#の Point[] に変換するため
        if (obj is null) return Array.Empty<System.Drawing.Point>();

        if (obj is JToken token)
        {
            if (token.Type != JTokenType.Array)
                throw new ArgumentException("Invalid points format");

            return token
                .Select(item =>
                {
                    var arr = item as JArray
                              ?? throw new ArgumentException("Invalid point element");

                    return new System.Drawing.Point(
                        arr[0]!.Value<int>(),
                        arr[1]!.Value<int>()
                    );
                })
                .ToArray();
        }

        if (obj is IEnumerable<object> enumerable)
        {
            return enumerable
                .Select(item =>
                {
                    if (item is IEnumerable<object> pair)
                    {
                        var p = pair.ToArray();
                        return new System.Drawing.Point(Convert.ToInt32(p[0]), Convert.ToInt32(p[1]));
                    }

                    throw new ArgumentException("Invalid point element");
                })
                .ToArray();
        }

        throw new ArgumentException("Invalid points format");
    }
}
