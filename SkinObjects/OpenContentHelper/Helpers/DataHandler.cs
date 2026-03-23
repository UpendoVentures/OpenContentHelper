/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Upendo.SkinObjects.OpenContentHelper.Helpers
{
    public class DataHandler
    {
        public static string GetString(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
                return null;

            try
            {
                var prop = obj.GetType().GetProperty(
                    propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                );

                if (prop == null)
                    return null;

                var value = prop.GetValue(obj, null);
                return value != null ? value.ToString() : null;
            }
            catch
            {
                return null;
            }
        }

        public static IReadOnlyList<string> GetStringList(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
                return null;

            try
            {
                var prop = obj.GetType().GetProperty(
                    propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                );

                if (prop == null)
                    return null;

                var value = prop.GetValue(obj, null);
                if (value == null)
                    return null;

                // IEnumerable (but not string)
                if (value is IEnumerable enumerable && !(value is string))
                {
                    var list = new List<string>();
                    foreach (var item in enumerable)
                    {
                        if (item != null)
                        {
                            var s = item.ToString();
                            if (!string.IsNullOrWhiteSpace(s))
                                list.Add(s);
                        }
                    }
                    return list.Count > 0 ? list : null;
                }

                // Single string (or comma-separated fallback)
                var str = value.ToString();
                if (string.IsNullOrWhiteSpace(str))
                    return null;

                if (str.Contains(","))
                {
                    var parts = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var list = new List<string>();
                    foreach (var p in parts)
                    {
                        var trimmed = p.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmed))
                            list.Add(trimmed);
                    }
                    return list.Count > 0 ? list : null;
                }

                return new[] { str };
            }
            catch
            {
                return null;
            }
        }
        
        public static string ToStringSafe(object o)
        {
            if (o == null || o == DBNull.Value) return string.Empty;
            return Convert.ToString(o);
        }

        public static int ToInt(object o, int def)
        {
            if (o == null || o == DBNull.Value) return def;
            try { return Convert.ToInt32(o); } catch { return def; }
        }

        public static byte ToByte(object o, byte def)
        {
            if (o == null || o == DBNull.Value) return def;
            try { return Convert.ToByte(o); } catch { return def; }
        }

        public static bool ToBool(object o, bool def)
        {
            if (o == null || o == DBNull.Value) return def;

            try
            {
                // handles bit
                if (o is bool) return (bool)o;

                // handles 0/1 in many numeric forms
                int n = Convert.ToInt32(o);
                return n != 0;
            }
            catch
            {
                return def;
            }
        }
    }
}