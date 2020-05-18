namespace Common
{
    using System.Collections.Generic;
    using UnityEngine;

    internal class NumberFormatter
    {
        private static readonly IDictionary<int, string> _formattedValueCache = new Dictionary<int, string>();

        internal static string FormatValue(float value)
        {
            int intCastValue = Mathf.CeilToInt(value);
            return FormatValue(intCastValue);
        }

        internal static string FormatValue(int value)
        {
            if (!_formattedValueCache.TryGetValue(value, out string amountString))
            {
                amountString = $"{HandleLargeNumbers(value)}";
                _formattedValueCache.Add(value, amountString);
            }
            return amountString;
        }

        private static string HandleLargeNumbers(float possiblyLargeValue)
        {
            if (possiblyLargeValue > 9999999f)
            {
                return $"{possiblyLargeValue / 1000000f:F1}M";
            }

            if (possiblyLargeValue > 9999f)
            {
                return $"{possiblyLargeValue / 1000f:F1}K";
            }

            return $"{possiblyLargeValue:F0}";
        }

        /// <summary>
        /// Goes from Red at 0% to Green at 100%, passing through Yellow at 50%.
        /// </summary>
        /// <param name="value">The current value</param>
        /// <param name="max">The 100% value.</param>
        /// <param name="min">The 0% value.</param>
        /// <returns>The calculated color.</returns>
        internal static Color GetNumberColor(float value, float max, float min)
        {
            float mid = (min + max) / 2;

            if (value < min)
                return Color.white;

            if (value <= mid)
                return Color.Lerp(Color.red, Color.yellow, (value - min) / (mid - min));            
            
            if (value <= max)
                return Color.Lerp(Color.yellow, Color.green, (value - mid) / (max - mid));
            
            return Color.white;
        }
    }
}
