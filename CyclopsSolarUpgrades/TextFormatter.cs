namespace CyclopsSolarUpgrades
{
    using System.Collections.Generic;
    using UnityEngine;

    internal static class TextFormatter
    {
        private static readonly IDictionary<int, string> _formattedSunCache = new Dictionary<int, string>();
        private static readonly IDictionary<int, string> _formattedAmountCache = new Dictionary<int, string>();

        internal static string FormatSolarPercentage(float solarPercent)
        {
            int value = Mathf.CeilToInt(solarPercent);
            if (!_formattedSunCache.TryGetValue(value, out string sunString))
            {
                sunString = $"{value}%Θ";
                _formattedSunCache.Add(value, sunString);
            }

            return sunString;
        }

        internal static string FormatBatteryCharge(float remainingCharge)
        {
            int value = Mathf.CeilToInt(remainingCharge);
            if (!_formattedAmountCache.TryGetValue(value, out string amountString))
            {
                amountString = $"{HandleLargeNumbers(value)}";
                _formattedAmountCache.Add(value, amountString);
            }
            return amountString;
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

            return $"{Mathf.CeilToInt(possiblyLargeValue)}";
        }
    }
}
