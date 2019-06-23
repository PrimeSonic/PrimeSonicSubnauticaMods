namespace CommonCyclopsUpgrades
{
    using System.Collections.Generic;
    using UnityEngine;

    internal class NumberFormatter
    {        
        private static readonly IDictionary<int, string> _formattedValueCache = new Dictionary<int, string>();

        internal static string FormatValue(float value)
        {
            return FormatValue(Mathf.CeilToInt(value));
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

        private static string HandleLargeNumbers(int possiblyLargeValue)
        {
            if (possiblyLargeValue > 9999999)
            {
                return $"{possiblyLargeValue / 1000000f:F1}M";
            }

            if (possiblyLargeValue > 9999)
            {
                return $"{possiblyLargeValue / 1000f:F1}K";
            }

            return $"{possiblyLargeValue}";
        }

        internal static Color GetNumberColor(float value, float max, float min)
        {
            if (value > max)
                return Color.white;

            if (value <= min)
                return Color.red;

            const float greenHue = 120f / 360f;
            float percentOfMax = (value - min) / (max - min);

            const float saturation = 1f;
            const float lightness = 0.8f;

            return Color.HSVToRGB(percentOfMax * greenHue, saturation, lightness);
        }
    }
}
