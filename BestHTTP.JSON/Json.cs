using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BestHTTP.JSON
{
    public class Json
    {
        // Decode JSON string into object
        public static object Decode(string json)
        {
            bool success = true;
            return json != null ? Decode(json.ToCharArray(), ref success) : null;
        }

        // Internal decode with reference to success state
        public static object Decode(string json, ref bool success)
        {
            success = true;
            return json != null ? Decode(json.ToCharArray(), ref success) : null;
        }

        // Convert object to JSON string
        public static string Encode(object json)
        {
            StringBuilder sb = new StringBuilder(2000);
            return SerializeValue(json, sb) ? sb.ToString() : null;
        }

        // Core parser for the JSON value
        protected static object Decode(char[] json, ref bool success)
        {
            int index = 0;
            return ParseValue(json, ref index, ref success);
        }

        // Parse a JSON object (key-value pairs)
        protected static Dictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
        {
            Dictionary<string, object> dict = new();
            NextToken(json, ref index); // Skip '{'
            bool done = false;

            while (!done)
            {
                int token = LookAhead(json, index);
                if (token == 0)
                {
                    success = false;
                    return null;
                }
                else if (token == 6) // Skip commas
                {
                    NextToken(json, ref index);
                    continue;
                }
                else if (token == 2) // End object
                {
                    NextToken(json, ref index);
                    return dict;
                }

                string key = ParseString(json, ref index, ref success);
                if (!success) return null;

                if (NextToken(json, ref index) != 5) // Expect colon
                {
                    success = false;
                    return null;
                }

                dict[key] = ParseValue(json, ref index, ref success);
                if (!success) return null;
            }

            return dict;
        }

        // Parse a JSON array (list of values)
        protected static List<object> ParseArray(char[] json, ref int index, ref bool success)
        {
            List<object> list = new();
            NextToken(json, ref index); // Skip '['
            bool done = false;

            while (!done)
            {
                int token = LookAhead(json, index);
                if (token == 0)
                {
                    success = false;
                    return null;
                }
                else if (token == 6) // Skip commas
                {
                    NextToken(json, ref index);
                    continue;
                }
                else if (token == 4) // End array
                {
                    NextToken(json, ref index);
                    break;
                }

                list.Add(ParseValue(json, ref index, ref success));
                if (!success) return null;
            }

            return list;
        }

        // Parse an individual JSON value
        protected static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case 7: return ParseString(json, ref index, ref success);
                case 8: return ParseNumber(json, ref index, ref success);
                case 1: return ParseObject(json, ref index, ref success);
                case 3: return ParseArray(json, ref index, ref success);
                case 9: NextToken(json, ref index); return true;
                case 10: NextToken(json, ref index); return false;
                case 11: NextToken(json, ref index); return null;
                default:
                    success = false;
                    return null;
            }
        }

        // Parse a string value
        protected static string ParseString(char[] json, ref int index, ref bool success)
        {
            StringBuilder sb = new();
            EatWhitespace(json, ref index);
            char c = json[index++]; // Skip starting quote

            while (index < json.Length)
            {
                c = json[index++];
                if (c == '"') return sb.ToString();
                if (c == '\\')
                {
                    if (index == json.Length) break;
                    c = json[index++];
                    sb.Append(c switch
                    {
                        '"' => '"',
                        '\\' => '\\',
                        '/' => '/',
                        'b' => '\b',
                        'f' => '\f',
                        'n' => '\n',
                        'r' => '\r',
                        't' => '\t',
                        'u' => ParseUnicode(json, ref index, ref success),
                        _ => c
                    });
                }
                else sb.Append(c);
            }

            success = false;
            return null;
        }

        // Parse a Unicode escape sequence (e.g., \u1234)
        private static char ParseUnicode(char[] json, ref int index, ref bool success)
        {
            if (json.Length - index < 4)
            {
                success = false;
                return '\0';
            }

            if (uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
            {
                index += 4;
                return (char)result;
            }

            success = false;
            return '\0';
        }

        // Parse a numeric value (int or double)
        protected static double ParseNumber(char[] json, ref int index, ref bool success)
        {
            EatWhitespace(json, ref index);
            int lastIndex = GetLastIndexOfNumber(json, index);
            string numberStr = new string(json, index, lastIndex - index + 1);

            success = double.TryParse(numberStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
            index = lastIndex + 1;
            return result;
        }

        // Get the last index of a number
        protected static int GetLastIndexOfNumber(char[] json, int index)
        {
            while (index < json.Length && "0123456789+-.eE".IndexOf(json[index]) != -1) index++;
            return index - 1;
        }

        // Skip whitespace characters
        protected static void EatWhitespace(char[] json, ref int index)
        {
            while (index < json.Length && " \t\n\r".IndexOf(json[index]) != -1) index++;
        }

        // Check next token without advancing index
        protected static int LookAhead(char[] json, int index)
        {
            int tempIndex = index;
            return NextToken(json, ref tempIndex);
        }

        // Fetch the next token and advance the index
        protected static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);
            if (index == json.Length) return 0;

            char c = json[index++];
            return c switch
            {
                '{' => 1,
                '}' => 2,
                '[' => 3,
                ']' => 4,
                ',' => 6,
                '"' => 7,
                ':' => 5,
                '-' or >= '0' and <= '9' => 8,
                _ when Matches(json, index - 1, "true") => 9,
                _ when Matches(json, index - 1, "false") => 10,
                _ when Matches(json, index - 1, "null") => 11,
                _ => 0
            };
        }

        // Helper method to match literal strings
        private static bool Matches(char[] json, int index, string literal)
        {
            if (json.Length - index < literal.Length) return false;
            for (int i = 0; i < literal.Length; i++)
            {
                if (json[index + i] != literal[i]) return false;
            }
            index += literal.Length;
            return true;
        }

        // Serialize an object to JSON
        protected static bool SerializeValue(object value, StringBuilder builder)
        {
            return value switch
            {
                string str => SerializeString(str, builder),
                IDictionary dict => SerializeObject(dict, builder),
                IList list => SerializeArray(list, builder),
                bool b => builder.Append(b ? "true" : "false").ToString() != null,
                null => builder.Append("null").ToString() != null,
                _ when value is ValueType => SerializeNumber(Convert.ToDouble(value), builder),
                _ => false
            };
        }

        // Serialize a dictionary (object)
        protected static bool SerializeObject(IDictionary obj, StringBuilder builder)
        {
            builder.Append("{");
            bool first = true;
            foreach (DictionaryEntry entry in obj)
            {
                if (!first) builder.Append(", ");
                SerializeString(entry.Key.ToString(), builder);
                builder.Append(":");
                SerializeValue(entry.Value, builder);
                first = false;
            }
            builder.Append("}");
            return true;
        }

        // Serialize a list (array)
        protected static bool SerializeArray(IList array, StringBuilder builder)
        {
            builder.Append("[");
            bool first = true;
            foreach (object item in array)
            {
                if (!first) builder.Append(", ");
                SerializeValue(item, builder);
                first = false;
            }
            builder.Append("]");
            return true;
        }

        // Serialize a string value
        protected static bool SerializeString(string str, StringBuilder builder)
        {
            builder.Append("\"");
            foreach (char c in str)
            {
                switch (c)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default:
                        int charValue = c;
                        if (charValue >= 32 && charValue <= 126)
                        {
                            builder.Append(c);
                        }
                        else
                        {
                            builder.Append("\\u" + charValue.ToString("x4"));
                        }
                        break;
                }
            }
            builder.Append("\"");
            return true;
        }

        // Serialize a number
        protected static bool SerializeNumber(double number, StringBuilder builder)
        {
            builder.Append(number.ToString(CultureInfo.InvariantCulture));
            return true;
        }
    }
}
