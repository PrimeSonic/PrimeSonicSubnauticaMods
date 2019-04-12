namespace MoreCyclopsUpgrades.Caching
{
    using System.Collections.Generic;
    using UnityEngine;

    internal static class NumberFormatter
    {
        private static readonly IDictionary<int, string> _formattedTemperatureCache = new Dictionary<int, string>();
        private static readonly IDictionary<int, string> _formattedSunCache = new Dictionary<int, string>();
        private static readonly IDictionary<int, string> _formattedPercentCache = new Dictionary<int, string>();
        private static readonly IDictionary<int, string> _formattedAmountCache = new Dictionary<int, string>();
        
        internal static string FormatNumber(int value, NumberFormat format)
        {
            switch (format)
            {
                case NumberFormat.Temperature:
                    string temperatureString;
                    if (!_formattedTemperatureCache.TryGetValue(value, out temperatureString))                        
                    {
                        temperatureString = $"{value}°C";
                        _formattedTemperatureCache.Add(value, temperatureString);
                    }
                    return temperatureString;
                case NumberFormat.Sun:
                    string sunString;
                    if (!_formattedSunCache.TryGetValue(value, out sunString))
                    {
                        sunString = $"{value}%Θ";
                        _formattedSunCache.Add(value, sunString);
                    }
                    return sunString;
                case NumberFormat.Amount:
                    string amountString;
                    if (!_formattedAmountCache.TryGetValue(value, out amountString))
                    {
                        amountString = $"{HandleLargeNumbers(value)}";
                        _formattedAmountCache.Add(value, amountString);
                    }
                    return amountString;
                case NumberFormat.Percent:
                    string percentString;
                    if (!_formattedPercentCache.TryGetValue(value, out percentString))
                    {
                        percentString = $"{value}%";
                        _formattedPercentCache.Add(value, percentString);
                    }
                    return percentString;
                default:
                    return Mathf.FloorToInt(value).ToString();
            }
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
