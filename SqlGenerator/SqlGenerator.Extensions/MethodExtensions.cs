using System;
using System.Collections;
using System.Text;

namespace SqlGenerator.Extensions
{
    public static class MethodExtensions
    {
        public static string ToString(this Exception e, string separator)
        {
            var result = "";

            if (e != null)
            {
                var current = e;

                while (current != null)
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = e.Message;
                    }
                    else
                    {
                        result += separator + current.Message;
                    }

                    current = current.InnerException;
                }
            }
            return result;
        }
        public static string Join(this IEnumerable e, string separator)
        {
            var sb = new StringBuilder();

            foreach (var item in e)
            {
                sb.Append((sb.Length == 0 ? "": separator) + item);
            }

            return sb.ToString();
        }
    }
}
