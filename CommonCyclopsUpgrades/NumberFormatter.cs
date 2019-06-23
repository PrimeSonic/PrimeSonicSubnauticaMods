namespace CommonCyclopsUpgrades
{
    using System.Collections.Generic;
    using UnityEngine;

    internal class NumberFormatter
    {
        private static readonly IDictionary<int, string> _formattedSunCache = new Dictionary<int, string>();
        private static readonly IDictionary<int, string> _formattedTemperatureCache = new Dictionary<int, string>();
        private static readonly IDictionary<int, string> _formattedAmountCache = new Dictionary<int, string>();

        internal static string FormatTemperature(float temperature)
        {
            return FormatTemperature(Mathf.CeilToInt(temperature));
        }

        internal static string FormatTemperature(int temperature)
        {            
            if (!_formattedTemperatureCache.TryGetValue(temperature, out string temperatureString))
            {
                temperatureString = $"{temperature}°C";
                _formattedTemperatureCache.Add(temperature, temperatureString);
            }
            return temperatureString;
        }

        internal static string FormatSolarPercentage(float solarPercent)
        {
            return FormatSolarPercentage(Mathf.CeilToInt(solarPercent));
        }

        internal static string FormatSolarPercentage(int solarPercent)
        {            
            if (!_formattedSunCache.TryGetValue(solarPercent, out string sunString))
            {
                sunString = $"{solarPercent}%Θ";
                _formattedSunCache.Add(solarPercent, sunString);
            }

            return sunString;
        }

        internal static string FormatValue(float value)
        {
            return FormatValue(Mathf.CeilToInt(value));
        }

        internal static string FormatValue(int value)
        {
            if (!_formattedAmountCache.TryGetValue(value, out string amountString))
            {
                amountString = $"{HandleLargeNumbers(value)}";
                _formattedAmountCache.Add(value, amountString);
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

            return $"{Mathf.CeilToInt(possiblyLargeValue)}";
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
