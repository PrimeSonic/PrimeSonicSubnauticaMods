namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal static class StringHelper
    {
        internal static string WithFirstUpper(this string value)
        {
            if (char.IsUpper(value[0]))
                return value;
            else
                return $"{char.ToUpperInvariant(value[0])}{value.Substring(1)}";
        }

    }
}
